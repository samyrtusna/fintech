import { useCallback, useEffect, useState } from "react";
import generateMonths from "../helpers/generateMonths";
import { Link } from "react-router-dom";
import { toast } from "sonner";
import {
  type GetTransactionResponse,
  type MonthItem,
  type TransactionsFilter,
} from "../types/financialTransactionTypes";
import { useAppDispatch, useAppSelector } from "../state/stateHooks";
import useClickOutside from "../hooks/useClickOutside";
import financialTrascationService from "../API/Services/financialTrascationService";
import { setFinancialTransactions } from "../state/slices/financialTransactionSlice";
import Spinner from "../components/Spinner";
import GlobalAggregates from "../components/GlobalAggregates";
import financialAggregatesService from "../API/Services/financialAggregatesService";
import {
  setGlobalAggregates,
  setMonthAggregates,
} from "../state/slices/financialAggregatesSlice";
import userService from "../API/Services/userService";
import { setUserInformations } from "../state/slices/authSlice";
import Dropdown from "../components/Dropdown";
import TransactionCard from "../components/TransactionCard";
import { IoMdAdd } from "react-icons/io";
import { IoChevronBack, IoChevronForward } from "react-icons/io5";
import { AiOutlineTransaction } from "react-icons/ai";
import PieChartWithCustomizedLabel from "../components/PieChart";
import PeriodicAggregateCard from "../components/PeriodicAggregateCard";

function FinancialTransactions() {
  const dispatch = useAppDispatch();

  const userInformations = useAppSelector(
    (state) => state.authUser.userInformations,
  );
  const transactionsState = useAppSelector((state) => state.transactions);
  const globalAggregates = useAppSelector(
    (state) => state.financialAggregates?.globalAggregates,
  );
  const monthAggregates = useAppSelector(
    (state) => state.financialAggregates.monthAggregates,
  );

  const totalIncome =
    monthAggregates?.find((ag) => ag.name === "TotalIncome")?.value ?? 0;
  const incomeVariation =
    monthAggregates?.find((ag) => ag.name === "TotalIncome")?.variation ?? 0;
  const totalExpense =
    monthAggregates?.find((ag) => ag.name === "TotalExpense")?.value ?? 0;
  const expenseVariation =
    monthAggregates?.find((ag) => ag.name === "TotalExpense")?.variation ?? 0;
  const expenseRatio =
    monthAggregates?.find((ag) => ag.name === "ExpensesRatio")?.value ?? 0;

  const totalInvestment =
    monthAggregates?.find((ag) => ag.name === "TotalInvestment")?.value ?? 0;
  const investmentVariation =
    monthAggregates?.find((ag) => ag.name === "TotalInvestment")?.variation ??
    0;
  const investmentRatio =
    monthAggregates?.find((ag) => ag.name === "InvestmentRatio")?.value ?? 0;
  const debt =
    monthAggregates?.find((ag) => ag.name === "TotalContractedLoans")?.value ??
    0;
  const debtVariation =
    monthAggregates?.find((ag) => ag.name === "TotalContractedLoans")
      ?.variation ?? 0;
  const debtRatio =
    monthAggregates?.find((ag) => ag.name === "DebtRatio")?.value ?? 0;

  const totalOthers = totalIncome - (totalExpense + totalInvestment + debt);

  const pieChartData = [
    { name: "Expenses", value: totalExpense },
    { name: "Investments", value: totalInvestment },
    { name: "Debt", value: debt },
    { name: "Other", value: totalOthers },
  ];

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
    if (globalAggregates !== null) {
      return;
    }
    try {
      const fetchedAggregates =
        await financialAggregatesService.getGlobalAggregates();
      dispatch(setGlobalAggregates(fetchedAggregates));
    } catch (error) {
      toast.error(
        error instanceof Error
          ? error.message
          : "Failed to fetch global aggregates",
      );
    }
  }, [dispatch, globalAggregates]);

  useEffect(() => {
    fetchGlobalAggregates();
    fetchUserInformations();
  }, [fetchGlobalAggregates, fetchUserInformations]);

  const months = generateMonths(userInformations!.createdAt);

  const [selectedMonth, setSelectedMonth] = useState<MonthItem>(months[0]);
  const [isTransactionsLoading, setIsTransactionsLoading] = useState(false);
  const [refreshTransactions, setRefreshTransactions] = useState(0);
  const [dateDropdown, setDateDropdown] = useState(false);
  const [page, setPage] = useState(1);
  const [pageChange, setPageChange] = useState(false);
  const [transactionDetails, setTransactionDetails] = useState("");
  const [updateTransaction, setUpdateTransaction] = useState("");
  const [showDate, setShowDate] = useState(true);

  const [description, setDescription] = useState("");
  const [isEssential, setIsEssential] = useState(false);
  const symbol = userInformations?.userCurrencies.find(
    (c) => c.isDefault === true,
  )?.currencySymbol;

  const fetchMonthAggregates = useCallback(
    async (date: Date) => {
      try {
        const aggregates = await financialAggregatesService.getByFilter({
          period: "Monthly",
          date: date,
        });
        dispatch(setMonthAggregates(aggregates));
      } catch (error) {
        toast.error(
          error instanceof Error
            ? error.message
            : "Failed to fetch month aggregates",
        );
      }
    },
    [dispatch],
  );

  let selectedTransaction;

  const startUpdateTransaction = (id: string) => {
    selectedTransaction = transactionsState.transactions?.items.find(
      (t) => t.id === id,
    );

    if (!selectedTransaction) {
      return;
    }

    setUpdateTransaction(id);
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
      setUpdateTransaction("");
    } catch (error) {
      toast.error(
        error instanceof Error ? error.message : "Failed to update transaction",
      );
    }
  };

  const handleDelete = async (t: GetTransactionResponse) => {
    try {
      await financialTrascationService.deleteAsync(t.id);

      setTransactionDetails("");
      setUpdateTransaction("");

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
    fetchMonthAggregates(selectedMonth.value);
  }, [
    selectedMonth,
    refreshTransactions,
    fetchMonthAggregates,
    page,
    dispatch,
  ]);

  const handleMonthClick = (month: MonthItem) => {
    setSelectedMonth(month);
    setPage(1);
    setDateDropdown(!dateDropdown);
    setTransactionDetails("");
    setUpdateTransaction("");
  };

  return (
    <div className="relative min-h-dvh h-full min-w-dvw ">
      <div className="flex justify-around w-full my-12 px-6">
        <GlobalAggregates
          debtValue={globalAggregates?.totalActiveDebt}
          investmentValue={globalAggregates?.totalInvestment}
          cashFlowValue={globalAggregates?.totalCashFlow}
          savingValue={globalAggregates?.totalSavings}
          symbol={symbol!}
        />
      </div>

      <div className="flex w-full justify-center">
        <div
          ref={monthRef}
          className="relative w-1/3 md:w-1/6"
        >
          <Dropdown<MonthItem>
            selectedValue={selectedMonth}
            options={months}
            onSelect={(selected) => handleMonthClick(selected)}
          />
        </div>
      </div>
      <div className="relative w-full px-2 flex justify-around">
        <div
          className={`hidden  w-[20%] h-full items-between  mt-10  ${updateTransaction !== "" ? "md:hidden" : "md:flex md:flex-col"}`}
        >
          <PeriodicAggregateCard
            title="Monthly Income"
            AggregateName="Total income"
            aggregateValue={totalIncome}
            aggregateVariation={incomeVariation}
            condition={incomeVariation > 10}
            symbol={symbol!}
            isBaseElement={true}
            isAnActif={true}
          />
          <PeriodicAggregateCard
            title="Monthly Investment"
            AggregateName="Total invested"
            aggregateValue={totalInvestment}
            aggregateVariation={investmentVariation}
            aggregateRatio={investmentRatio}
            condition={investmentRatio > 10}
            symbol={symbol!}
            isBaseElement={false}
            isAnActif={true}
          />
        </div>

        {isTransactionsLoading ? (
          <Spinner />
        ) : transactionsState.transactions !== null &&
          transactionsState.transactions.items?.length > 0 ? (
          <div
            className={`p-4 ${updateTransaction !== "" ? "w-full md:w-2/3" : "w-full  md:w-1/2 lg:w-5/12"}`}
          >
            {transactionsState.transactions?.items.map((t, index, arr) => (
              <div
                key={t.id}
                className={`${updateTransaction !== "" && updateTransaction !== t.id ? "hidden" : "block mx-auto"}`}
              >
                {showDate && (
                  <div
                    className={`
                    ${t.transactionDate === arr[index - 1]?.transactionDate || updateTransaction !== "" ? "hidden" : "block"} text-sm`}
                  >
                    {t.transactionDate}
                  </div>
                )}
                <TransactionCard
                  transaction={t}
                  startUpdateTransaction={() => {
                    startUpdateTransaction(t.id);
                  }}
                  handleDescription={(e) => setDescription(e.target.value)}
                  handleIsEssential={() => setIsEssential((prev) => !prev)}
                  isEssential={isEssential}
                  handleDelete={() => handleDelete(t)}
                  handleUpdate={handleUpdate}
                  description={description}
                  transactionDetails={transactionDetails}
                  setTransactionDetails={setTransactionDetails}
                  updateTransaction={updateTransaction}
                  setUpdateTransaction={setUpdateTransaction}
                />
              </div>

              // TODO: user can handle category importance
            ))}
            <div
              className={`${pageChange && updateTransaction === "" ? "flex justify-center gap-2" : "hidden"}`}
            >
              <button
                disabled={page === 1}
                onClick={() => setPage((prev) => prev - 1)}
                className="px-2 py-1 mr-1 rounded-md disabled:opacity-0 cursor-pointer hover:bg-bg-muted "
              >
                <IoChevronBack className="stroke-text-primary" />
              </button>

              <span className="px-2 pt-1 rounded-md">
                {page} / {totalPages}
              </span>

              <button
                disabled={page === totalPages}
                onClick={() => setPage((prev) => prev + 1)}
                className="px-2 py-1 ml-1 rounded-md disabled:opacity-0 cursor-pointer hover:bg-bg-muted "
              >
                <IoChevronForward className="stroke-text-primary" />
              </button>
            </div>
            <div
              className={` h-10 aspect-square group bg-blue-500 hover:bg-blue-600 rounded-sm ${updateTransaction === "" ? "absolute -top-8 right-6 md:right-[27%] lg:right-[30%]" : "hidden"}`}
            >
              <Link
                to="newFinancialTransaction"
                className="flex w-full h-full justify-center items-center rounded-sm shadow-card"
              >
                <IoMdAdd className="size-10 text-bg-contrast" />
              </Link>
              <span className="absolute opacity-0 md:group-hover:opacity-100 -top-5 -right-40 px-2 py-1 bg-bg-muted border-thin border-border-subtle rounded-sm">
                Add new Transaction
              </span>
            </div>
            <div
              className={`flex ${updateTransaction === "" ? "absolute -top-8 left-6 md:left-[27%] lg:left-[30%]" : "hidden"}`}
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
        ) : (
          <div className="flex flex-col justify-center items-center w-full h-fit mx-auto mt-10 md:w-1/2 lg:w-5/12 p-5 border-thin border-border-subtle rounded-md">
            <div className="flex justify-center m-4">
              <AiOutlineTransaction className="size-10 text-green-500" />
            </div>
            <h3 className="text-xl font-semibold my-2 mx-auto">
              A new month, a clearer financial picture.
            </h3>
            <h3 className="py-2">
              No transactions recorded for this month yet.
            </h3>
            <h3 className="text-lg pt-4">
              Keep your finances on track by recording your income, expenses,
              savings, investments, and debts.
            </h3>
            <h3 className="text-lg pb-4">
              {" "}
              Finance_It will help you understand your cash flow and monitor
              your financial health as the month unfolds.
            </h3>
            <div className="my-3 p-3 bg-green-500 text-gray-50 font-bold rounded-sm shadow-card">
              <Link to="newFinancialTransaction">
                <span className="text-lg mr-1">+</span>
                Add a transaction
              </Link>
            </div>
            <p className="text-sm">
              Small steps today. Better financial insights tomorrow.
            </p>
          </div>
        )}
        <div
          className={`hidden  w-[20%] h-fit mt-10  ${updateTransaction !== "" ? "md:hidden" : "md:block"}`}
        >
          <PeriodicAggregateCard
            title="Monthly Expense"
            AggregateName="Total spent"
            aggregateValue={totalExpense}
            aggregateVariation={expenseVariation}
            aggregateRatio={expenseRatio}
            condition={expenseRatio < 20}
            symbol={symbol!}
            isBaseElement={false}
            isAnActif={false}
          />
          <PeriodicAggregateCard
            title="Monthly Debt"
            AggregateName="Contracted debt"
            aggregateValue={debt}
            aggregateVariation={debtVariation}
            aggregateRatio={debtRatio}
            condition={debtRatio < 10}
            symbol={symbol!}
            isBaseElement={false}
            isAnActif={false}
          />
        </div>
      </div>
      <div className="flex flex-col justify-center md:hidden px-6">
        <PeriodicAggregateCard
          title="Monthly Income"
          AggregateName="Total income"
          aggregateValue={totalIncome}
          aggregateVariation={incomeVariation}
          condition={incomeVariation > 10}
          symbol={symbol!}
          isBaseElement={true}
          isAnActif={true}
        />
        <PeriodicAggregateCard
          title="Monthly Investment"
          AggregateName="Total invested"
          aggregateValue={totalInvestment}
          aggregateVariation={investmentVariation}
          aggregateRatio={investmentRatio}
          condition={investmentRatio > 10}
          symbol={symbol!}
          isBaseElement={false}
          isAnActif={true}
        />
        <PeriodicAggregateCard
          title="Monthly Expense"
          AggregateName="Total spent"
          aggregateValue={totalExpense}
          aggregateVariation={expenseVariation}
          aggregateRatio={expenseRatio}
          condition={expenseRatio < 20}
          symbol={symbol!}
          isBaseElement={false}
          isAnActif={false}
        />
        <PeriodicAggregateCard
          title="Monthly Debt"
          AggregateName="Contracted debt"
          aggregateValue={debt}
          aggregateVariation={debtVariation}
          aggregateRatio={debtRatio}
          condition={debtRatio < 10}
          symbol={symbol!}
          isBaseElement={false}
          isAnActif={false}
        />
      </div>

      <div
        className={`w-full h-fit my-5 px-6 md:px-12  ${updateTransaction !== "" || totalIncome === 0 ? "md:hidden" : "md:block"}`}
      >
        <div className="w-full h-fit px-10 border-thin border-border-subtle rounded-md bg-bg-contrast shadow-card ">
          <h3 className="text-xl font-bold mt-5 text-center">
            Income Breakdown
          </h3>
          <div className="w-full flex flex-col md:flex-row justify-between items-center">
            <div className="w-2/3 md:w-1/3 flex flex-col items-center">
              <PieChartWithCustomizedLabel data={pieChartData} />
            </div>

            <div className="w-1/2 md:w-1/4 mt-4 text-sm space-y-1">
              <div className="flex justify-between items-center">
                <span className="text-red-500 font-semibold">● Expenses</span>
                <span>
                  {symbol}
                  {totalExpense.toFixed(2)}
                </span>
              </div>
              <div className="flex justify-between items-center">
                <span className="text-green-500 font-semibold">
                  ● Investments
                </span>
                <span>
                  {symbol}
                  {totalInvestment.toFixed(2)}
                </span>
              </div>
              <div className="flex justify-between items-center">
                <span className="text-yellow-500 font-semibold">● Debt</span>
                <span>
                  {symbol}
                  {debt.toFixed(2)}
                </span>
              </div>
              <div className="flex justify-between items-center">
                <span className="text-blue-500 font-semibold">● Other</span>
                <span>
                  {symbol}
                  {totalOthers.toFixed(2)}
                </span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

export default FinancialTransactions;
// TODO:add an interface to allow a user to set it's currencies
// TODO:add a a div at the bottom to give advices
