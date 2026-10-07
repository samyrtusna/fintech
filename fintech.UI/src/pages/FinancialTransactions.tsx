import { useCallback, useEffect, useState } from "react";
import generateMonths from "../helpers/generateMonths";
import { Link } from "react-router-dom";
import { ChevronLeft, ChevronRight, PenLine, Plus, X } from "lucide-react";
import { toast } from "sonner";
import {
  type GetTransactionResponse,
  type MonthItem,
  type TransactionsFilter,
} from "../types/financialTransactionTypes";
import { useAppDispatch, useAppSelector } from "../state/stateHooks";
import useClickOutside from "../hooks/useClickOutside";
import FormatAmount from "../helpers/amountFormatter";
import financialTrascationService from "../API/Services/financialTrascationService";
import { setFinancialTransactions } from "../state/slices/financialTransactionSlice";
import Spinner from "../components/Spinner";
import GlobalAggregates from "../components/GlobalAggregates";
import financialAggregatesService from "../API/Services/financialAggregatesService";
import { setGlobalAggregates } from "../state/slices/financialAggregatesSlice";
import Button from "../components/Button";
import userService from "../API/Services/userService";
import { setUserInformations } from "../state/slices/authSlice";

function FinancialTransactions() {
  const dispatch = useAppDispatch();

  const userInformations = useAppSelector(
    (state) => state.authUser.userInformations,
  );
  const transactionsState = useAppSelector((state) => state.transactions);
  const globalAggregates = useAppSelector(
    (state) => state.financialAggregates?.globalAggregates,
  );

  const fetchUserInformations = useCallback(async () => {
    if (userInformations !== null) {
      return;
    }
    try {
      const user = await userService.getUserAsync();
      dispatch(setUserInformations(user));
    } catch (error) {
      toast.error(
        error instanceof Error
          ? error.message
          : "Failed to fetch user information",
      );
    }
  }, [userInformations, dispatch]);

  const fetchGlobalAggregates = useCallback(async () => {
    try {
      const fetchedAggregates =
        await financialAggregatesService.getGlobalAggregates();
      dispatch(setGlobalAggregates(fetchedAggregates));
    } catch (error) {
      toast.error(
        error instanceof Error
          ? error.message
          : "Failed to fetch globalAggregates",
      );
    }
  }, [dispatch]);

  useEffect(() => {
    fetchGlobalAggregates();
    fetchUserInformations();
  }, [fetchGlobalAggregates, fetchUserInformations]);

  const months = generateMonths(userInformations!.createdAt);

  const [selectedMonth, setSelectedMonth] = useState<MonthItem | null>(
    months[0],
  );
  const [isTransactionsLoading, setIsTransactionsLoading] = useState(false);
  const [refreshTransactions, setRefreshTransactions] = useState(0);
  const [dateDropdown, setDateDropdown] = useState(false);
  const [page, setPage] = useState(1);
  const [pageChange, setPageChange] = useState(false);
  const [transactionDetails, setTransactionDetails] = useState(-1);
  const [updateTransaction, setUpdateTransaction] = useState(-1);
  const [showDate, setShowDate] = useState(true);

  const [description, setDescription] = useState("");
  const [isEssential, setIsEssential] = useState(false);

  let selectedTransaction;

  const startUpdateTransaction = (index: number) => {
    selectedTransaction = transactionsState.transactions?.items[index];

    if (!selectedTransaction) {
      return;
    }

    setUpdateTransaction(index);
    setDescription(selectedTransaction.description ?? "");
    setIsEssential(selectedTransaction.isEssential ?? false);
  };

  const pageSize = 10;

  const monthRef = useClickOutside<HTMLDivElement>(() =>
    setDateDropdown(false),
  );

  const handleUpdate = async () => {
    const requestBody = {
      description:
        description!.length > 1 &&
        description !== selectedTransaction!.description
          ? description
          : undefined,
      isEssential:
        isEssential !== selectedTransaction!.isEssential
          ? isEssential
          : undefined,
    };
    try {
      await financialTrascationService.updateAsync(
        selectedTransaction!.id,
        requestBody,
      );
      setRefreshTransactions((prev) => prev + 1);
      setUpdateTransaction(-1);
    } catch (error) {
      toast.error(
        error instanceof Error ? error.message : "Failed to update transaction",
      );
    }
  };

  const handleDelete = async (t: GetTransactionResponse) => {
    try {
      await financialTrascationService.deleteAsync(t.id);

      setTransactionDetails(-1);
      setUpdateTransaction(-1);

      setRefreshTransactions((prev) => prev + 1);
    } catch (error) {
      toast.error(
        error instanceof Error ? error.message : "Failed to delete transaction",
      );
    }
  };

  const totalPages =
    transactionsState.transactions == null
      ? 0
      : Math.ceil(
          transactionsState.transactions.totalCount /
            transactionsState.transactions.pageSize,
        );

  useEffect(() => {
    const filter: TransactionsFilter = {
      date: selectedMonth!.value,
      page,
      pageSize,
    };

    const fetchTransactions = async () => {
      try {
        setIsTransactionsLoading(true);
        const fetchedTransactions =
          await financialTrascationService.getByFilterAsync(filter);

        dispatch(setFinancialTransactions(fetchedTransactions));
        if (fetchedTransactions.totalCount >= 10) {
          setPageChange(true);
        } else {
          setPageChange(false);
        }
      } catch (error) {
        toast.error(
          error instanceof Error
            ? error.message
            : "Failed to fetch transactions",
        );
      } finally {
        setIsTransactionsLoading(false);
      }
    };

    fetchTransactions();
  }, [selectedMonth, refreshTransactions, page, dispatch]);

  const handleMonthClick = (month: MonthItem) => {
    setSelectedMonth(month);
    setPage(1);
    setDateDropdown(!dateDropdown);
    setTransactionDetails(-1);
    setUpdateTransaction(-1);
  };

  return (
    <div className="relative min-h-dvh h-full min-w-dvw ">
      <div className="flex justify-around my-12">
        <GlobalAggregates
          debtValue={globalAggregates?.totalActiveDebt}
          investmentValue={globalAggregates?.totalInvestment}
          cashFlowValue={globalAggregates?.totalCashFlow}
          savingValue={globalAggregates?.totalSavings}
          symbol={
            transactionsState.transactions?.items[0]?.baseCurrencySymbol ?? ""
          }
        />
      </div>

      <div className="flex w-full justify-center">
        <div
          ref={monthRef}
          className="relative"
        >
          <div>
            <button
              onClick={() => {
                setDateDropdown((prev) => !prev);
              }}
              className={`w-30 py-1 px-2 border-thin border-border-subtle bg-bg-muted hover:bg-bg-secondary cursor-pointer ${dateDropdown ? "rounded-t-sm" : "rounded-sm"}`}
            >
              {selectedMonth?.key}
            </button>
            {dateDropdown && (
              <div className="absolute top-8 w-30 flex flex-col z-50">
                {months
                  .filter((month) => month.key !== selectedMonth?.key)
                  .map((month, index, arr) => (
                    <button
                      key={month.key}
                      onClick={() => handleMonthClick(month)}
                      className={`py-1 px-2 bg-bg-muted hover:bg-bg-secondary border-x-thin border-x-border-subtle cursor-pointer
                  ${index === arr.length - 1 ? "rounded-b-sm border-b-thin border-b-border-subtle" : ""}`}
                    >
                      {month.key}
                    </button>
                  ))}
              </div>
            )}
          </div>
        </div>
      </div>
      <div className="relative w-full px-2 flex justify-center mt-6">
        {isTransactionsLoading ? (
          <Spinner />
        ) : (
          <div className="w-full p-4">
            {transactionsState.transactions?.items.map((t, index, arr) => (
              <div
                key={t.id}
                className={`${updateTransaction !== -1 && updateTransaction !== index ? "hidden" : updateTransaction === index ? "w-full mx-auto md:w-2/3" : "w-full mx-auto md:w-1/2 lg:w-5/12"}`}
              >
                {showDate && (
                  <div
                    className={`
                    ${t.transactionDate === arr[index - 1]?.transactionDate || updateTransaction !== -1 ? "hidden" : "block"} text-sm`}
                  >
                    {t.transactionDate}
                  </div>
                )}
                <div
                  className={`my-2 border-thin border-border-subtle rounded-md bg-bg-muted w-full transition-all duration-200 ${
                    updateTransaction === index
                      ? "shadow-lg p-2"
                      : "cursor-pointer"
                  }`}
                >
                  <div
                    className={`min-w-full ${transactionDetails === index ? "flex" : "hidden"} justify-end items-baseline px-5 py-3`}
                  >
                    <button
                      onClick={() => {
                        setUpdateTransaction(-1);
                      }}
                      className={`h-8 aspect-square rounded-full p-1 hover:bg-bg-secondary cursor-pointer ${updateTransaction === index ? "block" : "hidden"}`}
                    >
                      <X className="stroke-2" />
                    </button>
                    <button
                      onClick={() => {
                        startUpdateTransaction(index);
                      }}
                      className={`h-8 aspect-square rounded-full p-1 hover:bg-bg-secondary cursor-pointer ${updateTransaction === index ? "hidden" : "block"}`}
                    >
                      <PenLine className="stroke-1" />
                    </button>
                  </div>
                  <div
                    onClick={() =>
                      updateTransaction !== index && setTransactionDetails(-1)
                    }
                    className={`min-w-full ${transactionDetails === index ? "flex" : "hidden"} justify-between items-baseline px-5 py-3`}
                  >
                    <div className="w-full md:w-1/3">
                      <h3
                        className={`my-1 ${updateTransaction === index ? "block" : "hidden"}`}
                      >
                        Type
                      </h3>
                      <h3
                        className={`${updateTransaction === index ? "w-full px-2 py-1 border-thin border-border-subtle rounded-sm bg-bg-contrast" : "mx-0"}`}
                      >
                        {t.type}
                      </h3>
                    </div>
                  </div>
                  <div
                    onClick={() =>
                      transactionDetails !== index
                        ? setTransactionDetails(index)
                        : updateTransaction !== index &&
                          setTransactionDetails(-1)
                    }
                    className={`min-w-full justify-between items-baseline px-5 py-3 ${updateTransaction === index ? "flex-col md:flex md:flex-row" : "flex"}`}
                  >
                    <div
                      className={`w-full pb-6 md:pb-0 ${updateTransaction === index ? "md:w-1/3" : "md:w-fit"}`}
                    >
                      <h3
                        className={`my-1 ${updateTransaction === index ? "block" : "hidden"}`}
                      >
                        Category
                      </h3>
                      <h3
                        className={`${updateTransaction === index ? "w-full px-2 py-1 border-thin border-border-subtle rounded-sm bg-bg-contrast" : "mx-0"}`}
                      >
                        {t.categoryName}
                      </h3>
                    </div>
                    <div className="w-full md:w-1/3">
                      <h3
                        className={`my-1 ${updateTransaction === index ? "block" : "hidden"}`}
                      >
                        Amount
                      </h3>
                      <div
                        className={`flex ${updateTransaction === index ? "w-full px-2 py-1 border-thin border-border-subtle rounded-sm bg-bg-contrast" : "float-end"}`}
                      >
                        <h3 className="font-bold text-green-500 ">
                          {t.currencySymbol}
                        </h3>{" "}
                        <h3 className="font-bold ml-1">
                          {FormatAmount(t.amount)}
                        </h3>
                      </div>
                    </div>
                  </div>
                  <div
                    onClick={() =>
                      updateTransaction !== index && setTransactionDetails(-1)
                    }
                    className={`min-w-full ${transactionDetails === index ? "block" : "hidden"}`}
                  >
                    <div
                      className={`min-w-full justify-between items-baseline px-5 py-3 ${updateTransaction === index ? "flex-col md:flex md:flex-row" : "flex"}`}
                    >
                      <div
                        className={`w-full pb-6 md:pb-0 ${updateTransaction === index ? "md:w-1/3" : "md:w-fit"}`}
                      >
                        <h3
                          className={`${updateTransaction === index ? "block" : "hidden"}`}
                        >
                          Description
                        </h3>
                        <input
                          type="text"
                          placeholder={t!.description}
                          onChange={(e) => setDescription(e.target.value)}
                          className={`placeholder:text-text-primary ${updateTransaction === index ? "w-full px-2 py-1 border-thin border-border-subtle rounded-sm bg-bg-contrast outline-0 " : "mx-0"}`}
                        />
                      </div>
                      <div className="w-full md:w-1/3">
                        <h3
                          className={`my-1 ${updateTransaction === index ? "block" : "hidden"}`}
                        >
                          Base Amount
                        </h3>
                        <div
                          className={`flex ${updateTransaction === index ? "w-full px-2 py-1 border-thin border-border-subtle rounded-sm bg-bg-contrast font-bold" : "float-end"}`}
                        >
                          {t.baseAmount !== t.amount ? (
                            <>
                              {" "}
                              <h3 className="font-bold text-green-500">
                                {t.baseCurrencySymbol}
                              </h3>{" "}
                              <h3 className="ml-1">
                                {FormatAmount(t.baseAmount)}
                              </h3>
                            </>
                          ) : updateTransaction === index ? (
                            FormatAmount(t.amount)
                          ) : (
                            ""
                          )}
                        </div>
                      </div>
                    </div>
                    <div
                      className={`min-w-full justify-between items-baseline px-5 py-3 ${updateTransaction === index ? "flex-col md:flex md:flex-row" : "flex"}`}
                    >
                      <div className="w-full pb-6 md:w-1/3 md:pb-0">
                        <h3
                          className={`${updateTransaction === index ? "block" : "hidden"}`}
                        >
                          Transaction Date
                        </h3>
                        <h3
                          className={`${updateTransaction === index ? "w-full px-2 py-1 border-thin border-border-subtle rounded-sm bg-bg-contrast" : "mx-0"}`}
                        >
                          {t.transactionDate}
                        </h3>
                      </div>

                      <div className="w-full md:w-1/3">
                        <h3
                          className={`my-1 ${updateTransaction === index ? "block" : "hidden"}`}
                        >
                          Exchange Rate
                        </h3>
                        <h3
                          className={`${updateTransaction === index ? "w-full px-2 py-1 border-thin border-border-subtle rounded-sm bg-bg-contrast" : "float-end"}`}
                        >
                          {t.exchangeRate !== 1
                            ? t.exchangeRate
                            : updateTransaction === index
                              ? 1
                              : ""}
                        </h3>
                      </div>
                    </div>
                    <div
                      className={`min-w-full  px-5 py-3 ${updateTransaction === index ? "flex-col md:flex md:flex-row md:justify-between md:items-end" : "block"}`}
                    >
                      <div className="w-full md:w-1/3">
                        <h3
                          className={`my-1 ${updateTransaction === index ? "block" : "hidden"}`}
                        >
                          Importance
                        </h3>
                        <h3
                          onClick={() => setIsEssential((prev) => !prev)}
                          className={`${updateTransaction === index ? "w-full px-2 py-1 border-thin border-border-subtle rounded-sm bg-bg-contrast cursor-pointer" : "mx-0"}`}
                        >
                          {updateTransaction === index
                            ? isEssential
                              ? "Important"
                              : "Not Important"
                            : ""}
                        </h3>
                      </div>
                      <div
                        className={`w-full md:w-1/3 ${updateTransaction === index ? "flex-col md:flex md:flex-row justify-between" : "hidden"} `}
                      >
                        <div className="w-full mt-2 md:w-[45%]">
                          <Button
                            label="Delete"
                            type="button"
                            background="bg-btn-danger"
                            hoverBg="hover:bg-btn-danger-hover"
                            textColor="btn-danger-text"
                            handleClick={() => handleDelete(t)}
                            disabled={
                              new Date(t.transactionDate).getFullYear <
                              new Date().getFullYear
                            }
                          />
                        </div>
                        <div className="w-full mt-2 md:w-[45%]">
                          <Button
                            label="Update"
                            type="button"
                            background="bg-btn-standard"
                            hoverBg="hover:bg-btn-standard-hover"
                            textColor="btn-standard-text"
                            handleClick={handleUpdate}
                            disabled={
                              description === t.description &&
                              isEssential === t.isEssential
                            }
                          />
                        </div>
                      </div>
                      <div
                        className={`w-50 h-1 mx-auto rounded-full my-2 ${t.isEssential ? "bg-lime-500" : "bg-red-500"} ${updateTransaction === index ? "hidden" : "block"}`}
                      ></div>
                    </div>
                  </div>
                </div>
              </div>

              // TODO: user can handle category importance
            ))}
            <div
              className={` h-10 aspect-square group bg-blue-500 hover:bg-blue-600 rounded-sm ${updateTransaction === -1 ? "absolute -top-8 right-6 md:right-[27%] lg:right-[31%]" : "hidden"}`}
            >
              <Link to="newFinancialTransaction">
                <Plus className="stroke-1 stroke-white w-full h-full rounded-sm shadow-card" />
              </Link>
              <span className="absolute opacity-0 md:group-hover:opacity-100 -top-5 -right-40 px-2 py-1 bg-bg-muted border-thin border-border-subtle rounded-sm">
                Add new Transaction
              </span>
            </div>
            <div
              className={`flex ${updateTransaction ? "absolute -top-8 left-6 md:left-[27%] lg:left-[31%]" : "hidden"}`}
            >
              <div className="flex justify-between h-8 w-16 rounded-full bg-bg-contrast p-1 border-thin border-border-subtle">
                <div
                  onClick={() => setShowDate((prev) => !prev)}
                  className={`h-full aspect-square rounded-full bg-btn-disabled cursor-pointer ${showDate ? "opacity-0" : "opacity-100"}`}
                ></div>
                <div
                  onClick={() => setShowDate((prev) => !prev)}
                  className={`h-full aspect-square rounded-full bg-bg-surface cursor-pointer ${showDate ? "opacity-100" : "opacity-0"}`}
                ></div>
              </div>
            </div>
          </div>
        )}
      </div>
      <div className={`${pageChange ? "flex justify-center gap-2" : "hidden"}`}>
        <button
          disabled={page === 1}
          onClick={() => setPage((prev) => prev - 1)}
          className="px-2 py-1 mr-1 rounded-md disabled:opacity-0 cursor-pointer hover:bg-bg-muted "
        >
          <ChevronLeft className="stroke-text-primary" />
        </button>

        <span className="px-2 pt-1 rounded-md">
          {page} / {totalPages}
        </span>

        <button
          disabled={page === totalPages}
          onClick={() => setPage((prev) => prev + 1)}
          className="px-2 py-1 ml-1 rounded-md disabled:opacity-0 cursor-pointer hover:bg-bg-muted "
        >
          <ChevronRight className="stroke-text-primary" />
        </button>
      </div>
    </div>
  );
}

export default FinancialTransactions;
