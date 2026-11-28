import { useCallback, useEffect, useMemo, useState } from "react";
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
import Dropdown from "../components/Dropdown";
import TransactionCard from "../components/TransactionCard";
import { IoMdAdd } from "react-icons/io";
import { IoChevronBack, IoChevronForward } from "react-icons/io5";
import { AiOutlineTransaction } from "react-icons/ai";
import PieCharts from "../components/PieChart";
import PeriodicAggregateCard from "../components/PeriodicAggregateCard";
import ScrollReveal from "../components/ScrollReveal";
import SwitchButton from "../components/SwitchButton";
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
  const monthAggregates = useAppSelector(
    (state) => state.financialAggregates.monthAggregates,
  );

  const globalDebt = globalAggregates?.find(
    (ag) => ag.name.toLowerCase() === "GlobalActiveDebt".toLowerCase(),
  )?.amount.amount;
  const globalInvestment = globalAggregates?.find(
    (ag) => ag.name.toLowerCase() === "GlobalTotalInvestment".toLowerCase(),
  )?.amount.amount;
  const globalCashFlow = globalAggregates?.find(
    (ag) => ag.name.toLowerCase() === "GlobalNetCashFlow".toLowerCase(),
  )?.amount.amount;
  const globalSavings = globalAggregates?.find(
    (ag) => ag.name.toLowerCase() === "GlobalTotalSavings".toLowerCase(),
  )?.amount.amount;

  const totalIncome =
    monthAggregates?.aggregates.find(
      (ag) => ag.name.toLowerCase() === "TotalIncome".toLowerCase(),
    )?.amount.amount ?? 0;
  const incomeVariation =
    monthAggregates?.aggregates.find(
      (ag) => ag.name.toLowerCase() === "TotalIncome".toLowerCase(),
    )?.variation ?? 0;

  const totalExpense =
    monthAggregates?.aggregates.find(
      (ag) => ag.name.toLowerCase() === "TotalExpense".toLowerCase(),
    )?.amount.amount ?? 0;
  const expenseVariation =
    monthAggregates?.aggregates.find(
      (ag) => ag.name.toLowerCase() === "TotalExpense".toLowerCase(),
    )?.variation ?? 0;
  const expenseRatio =
    monthAggregates?.aggregates.find(
      (ag) => ag.name.toLowerCase() === "ExpensesRatio".toLowerCase(),
    )?.amount.amount ?? 0;

  const totalInvestment =
    monthAggregates?.aggregates.find(
      (ag) => ag.name.toLowerCase() === "TotalInvestment".toLowerCase(),
    )?.amount.amount ?? 0;
  const investmentVariation =
    monthAggregates?.aggregates.find(
      (ag) => ag.name.toLowerCase() === "TotalInvestment".toLowerCase(),
    )?.variation ?? 0;
  const investmentRatio =
    monthAggregates?.aggregates.find(
      (ag) => ag.name.toLowerCase() === "InvestmentRatio".toLowerCase(),
    )?.amount.amount ?? 0;

  const debt =
    monthAggregates?.aggregates.find(
      (ag) => ag.name.toLowerCase() === "TotalContractedLoans".toLowerCase(),
    )?.amount.amount ?? 0;
  const debtVariation =
    monthAggregates?.aggregates.find(
      (ag) => ag.name.toLowerCase() === "TotalContractedLoans".toLowerCase(),
    )?.variation ?? 0;
  const debtRatio =
    monthAggregates?.aggregates.find(
      (ag) => ag.name.toLowerCase() === "DebtRatio".toLowerCase(),
    )?.amount.amount ?? 0;

  const savings =
    monthAggregates?.aggregates.find(
      (ag) => ag.name.toLowerCase() === "TotalSavings".toLowerCase(),
    )?.amount.amount ?? 0;

  const pieChartData = [
    { name: "Expenses", value: totalExpense, color: "#f3081c" },
    { name: "Investments", value: totalInvestment, color: "#10f308" },
    { name: "Debt", value: debt, color: "#f3ef08" },
    { name: "Savings", value: savings, color: "#8108f3" },
  ];

  const fetchUserInformations = useCallback(async () => {
    if (userInformations !== null) {
      return;
    }
    try {
      const fetchedUserInformations = await userService.getUserAsync();
      dispatch(setUserInformations(fetchedUserInformations));
    } catch (error) {
      toast.error(
        error instanceof Error
          ? error.message
          : "Failed to fetch user informations",
      );
    }
  }, [dispatch, userInformations]);

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
    fetchUserInformations();
    fetchGlobalAggregates();
  }, [fetchGlobalAggregates, fetchUserInformations]);

  const createdAt = userInformations?.createdAt;

  const months: MonthItem[] = useMemo(() => {
    return createdAt ? generateMonths(createdAt) : [];
  }, [createdAt]);

  const [selectedMonth, setSelectedMonth] = useState<MonthItem | undefined>(
    undefined,
  );
  const [isTransactionsLoading, setIsTransactionsLoading] = useState(false);
  const [refreshTransactions, setRefreshTransactions] = useState(0);
  const [dateDropdown, setDateDropdown] = useState(false);
  const [page, setPage] = useState(1);
  const [pageChange, setPageChange] = useState(false);
  const [transactionDetails, setTransactionDetails] = useState("");
  const [updateTransaction, setUpdateTransaction] = useState("");
  const [showDate, setShowDate] = useState(false);
  const [editingTransaction, setEditingTransaction] =
    useState<GetTransactionResponse | null>(null);

  const [description, setDescription] = useState("");
  const [isEssential, setIsEssential] = useState(false);

  if (months.length > 0 && !selectedMonth) {
    setSelectedMonth(months[0]);
  }

  const symbol = userInformations?.userCurrencies.find(
    (c) => c.isDefault === true,
  )?.currency.symbol;

  const fetchMonthAggregates = useCallback(
    async (date: Date) => {
      try {
        const aggregates = await financialAggregatesService.getByMonth(date);
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

  const startUpdateTransaction = (id: string) => {
    const found = transactionsState.transactions?.items.find(
      (t) => t.id === id,
    );
    if (!found) {
      return;
    }

    setEditingTransaction(found);
    setUpdateTransaction(id);
    setDescription(found.description ?? "");
    setIsEssential(found.isEssential ?? false);
  };

  const pageSize = 10;

  const monthRef = useClickOutside<HTMLDivElement>(() =>
    setDateDropdown(false),
  );

  const handleUpdate = async () => {
    if (!editingTransaction) {
      return;
    }

    const requestBody = {
      description:
        description.trim().length > 0 &&
        description !== editingTransaction.description
          ? description
          : undefined,
      isEssential:
        isEssential !== editingTransaction.isEssential
          ? isEssential
          : undefined,
    };

    try {
      await financialTrascationService.updateAsync(
        editingTransaction.id,
        requestBody,
      );
      setRefreshTransactions((prev) => prev + 1);
      setUpdateTransaction("");
      setEditingTransaction(null);
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
    if (!selectedMonth) {
      return;
    }
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

  if (!userInformations || !selectedMonth) {
    return (
      <div className="flex min-h-dvh w-full justify-center items-center">
        <Spinner />
      </div>
    );
  }

  return (
    <div className="relative min-h-dvh h-full min-w-dvw text-text-primary">
      <div className="flex justify-around w-full my-10 px-6">
        <GlobalAggregates
          debtValue={globalDebt}
          investmentValue={globalInvestment}
          cashFlowValue={globalCashFlow}
          savingValue={globalSavings}
          symbol={symbol!}
        />
      </div>

      <div className="flex w-full md:w-1/2 lg:w-5/12 md:mx-auto justify-between items-center mb-4 p-4 md:p-0">
        <div className={`${updateTransaction === "" ? "flex" : "hidden"}`}>
          <SwitchButton
            handleClick={() => setShowDate((prev) => !prev)}
            showDate={showDate}
          />
        </div>
        <div
          ref={monthRef}
          className={`${updateTransaction === "" ? "flex" : "hidden"} w-1/3`}
        >
          <Dropdown<MonthItem>
            selectedValue={selectedMonth}
            options={months}
            onSelect={(selected) => handleMonthClick(selected)}
          />
        </div>
        <div
          className={`h-8 aspect-square group bg-btn-standard hover:bg-btn-standard-hover rounded-sm ${updateTransaction === "" ? "flex" : "hidden"}`}
        >
          <Link
            to="new"
            className="flex w-full h-full justify-center items-center rounded-sm shadow-card"
          >
            <IoMdAdd className="size-7 text-btn-standard-text" />
          </Link>
          <span className="absolute opacity-0 md:group-hover:opacity-100 -top-6 -right-40 px-2.5 py-1 text-xs font-medium text-text-primary bg-bg-surface border-thin border-border-subtle rounded-sm shadow-subtle">
            Add new Transaction
          </span>
        </div>
      </div>

      <div className="relative w-full px-2 flex justify-around">
        <div
          className={`hidden w-[20%] h-full items-between mt-10 ${updateTransaction !== "" ? "md:hidden" : "md:flex md:flex-col"}`}
        >
          <PeriodicAggregateCard
            title="Monthly Income"
            aggregateValue={totalIncome}
            aggregateVariation={incomeVariation}
            symbol={symbol!}
            isBaseElement={true}
            isAnActif={true}
          />
          <PeriodicAggregateCard
            title="Monthly Investment"
            aggregateValue={totalInvestment}
            aggregateRatio={investmentRatio}
            aggregateVariation={investmentVariation}
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
            className={`p-4 bg-bg-secondary/70 border-thin border-border-subtle rounded-md shadow-subtle ${updateTransaction !== "" ? "w-full md:w-2/3" : "w-full md:w-1/2 lg:w-5/12"}`}
          >
            {transactionsState.transactions?.items.map((t, index, arr) => (
              <div
                key={t.id}
                className={`${updateTransaction !== "" && updateTransaction !== t.id ? "hidden" : "block mx-auto"}`}
              >
                {showDate && (
                  <div
                    className={`
                    ${t.transactionDate === arr[index - 1]?.transactionDate || updateTransaction !== "" ? "hidden" : "block"} text-xs font-semibold uppercase tracking-wider text-text-secondary mt-3 mb-1 px-1`}
                  >
                    {t.transactionDate.toString()}
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
            ))}

            <div
              className={`${pageChange && updateTransaction === "" ? "flex justify-center items-center gap-2 mt-4" : "hidden"}`}
            >
              <button
                disabled={page === 1}
                onClick={() => setPage((prev) => prev - 1)}
                className="px-2.5 py-1.5 mr-1 rounded-md border-thin border-border-subtle bg-bg-surface disabled:opacity-30 cursor-pointer hover:bg-bg-muted"
              >
                <IoChevronBack className="stroke-text-primary" />
              </button>

              <span className="px-2.5 py-1 text-sm font-medium text-text-secondary rounded-md">
                {page} / {totalPages}
              </span>

              <button
                disabled={page === totalPages}
                onClick={() => setPage((prev) => prev + 1)}
                className="px-2.5 py-1.5 ml-1 rounded-md border-thin border-border-subtle bg-bg-surface disabled:opacity-30 cursor-pointer hover:bg-bg-muted"
              >
                <IoChevronForward className="stroke-text-primary" />
              </button>
            </div>
          </div>
        ) : (
          <div className="flex flex-col justify-center items-center w-full h-fit mx-auto mt-10 md:w-1/2 lg:w-5/12 p-6 bg-bg-surface border-thin border-border-subtle rounded-md shadow-card text-center">
            <div className="flex justify-center m-4 p-3 rounded-full bg-bg-muted">
              <AiOutlineTransaction className="size-9 text-text-secondary" />
            </div>
            <h3 className="text-xl font-bold my-2 mx-auto text-text-primary">
              A new month, a clearer financial picture.
            </h3>
            <h3 className="py-1.5 text-sm font-medium text-text-secondary">
              No transactions recorded for this month yet.
            </h3>
            <h3 className="text-sm pt-3 text-text-primary">
              Keep your finances on track by recording your income, expenses,
              savings, investments, and debts.
            </h3>
            <h3 className="text-sm pb-4 text-text-primary">
              Finance_It will help you understand your cash flow and monitor
              your financial health as the month unfolds.
            </h3>
            <div className="my-3 px-4 py-2.5 bg-btn-standard hover:bg-btn-standard-hover text-btn-standard-text font-bold rounded-sm shadow-card">
              <Link to="newFinancialTransaction">
                <span className="text-lg mr-1">+</span>
                Add a transaction
              </Link>
            </div>
            <p className="text-xs text-text-muted mt-2">
              Small steps today. Better financial insights tomorrow.
            </p>
          </div>
        )}

        <div
          className={`hidden w-[20%] h-full mt-10 justify-between ${updateTransaction !== "" ? "md:hidden" : "md:flex md:flex-col"}`}
        >
          <PeriodicAggregateCard
            title="Monthly Expense"
            aggregateValue={totalExpense}
            aggregateRatio={expenseRatio}
            aggregateVariation={expenseVariation}
            symbol={symbol!}
            isBaseElement={false}
            isAnActif={false}
          />
          <PeriodicAggregateCard
            title="Monthly Debt"
            aggregateValue={debt}
            aggregateRatio={debtRatio}
            aggregateVariation={debtVariation}
            symbol={symbol!}
            isBaseElement={false}
            isAnActif={false}
          />
        </div>
      </div>

      <div className="flex flex-col justify-center md:hidden px-6">
        <PeriodicAggregateCard
          title="Monthly Income"
          aggregateValue={totalIncome}
          aggregateVariation={incomeVariation}
          symbol={symbol!}
          isBaseElement={true}
          isAnActif={true}
        />
        <PeriodicAggregateCard
          title="Monthly Investment"
          aggregateValue={totalInvestment}
          aggregateRatio={investmentRatio}
          aggregateVariation={investmentVariation}
          symbol={symbol!}
          isBaseElement={false}
          isAnActif={true}
        />
        <PeriodicAggregateCard
          title="Monthly Expense"
          aggregateValue={totalExpense}
          aggregateRatio={expenseRatio}
          aggregateVariation={expenseVariation}
          symbol={symbol!}
          isBaseElement={false}
          isAnActif={false}
        />
        <PeriodicAggregateCard
          title="Monthly Debt"
          aggregateValue={debt}
          aggregateRatio={debtRatio}
          aggregateVariation={debtVariation}
          symbol={symbol!}
          isBaseElement={false}
          isAnActif={false}
        />
      </div>

      <ScrollReveal>
        <div
          className={`w-full h-fit my-6 px-6 md:px-12 ${updateTransaction !== "" || totalIncome === 0 ? "md:hidden" : "md:block"}`}
        >
          <div className="w-full h-fit px-10 py-6 border-thin border-border-subtle rounded-md bg-bg-surface shadow-card">
            <h3 className="text-xl font-bold mt-2 mb-4 text-center text-text-primary">
              Outflow Breakdown
            </h3>
            <div className="w-full flex flex-col md:flex-row justify-between items-center">
              <div className="w-2/3 md:w-1/3 flex flex-col items-center">
                <PieCharts data={pieChartData} />
              </div>

              <div className="w-full md:w-1/4 mt-4 p-4 rounded-md bg-bg-secondary border-thin border-border-subtle text-sm space-y-2">
                {pieChartData.map((pie) => (
                  <div
                    key={pie.name}
                    className="flex justify-between items-center"
                  >
                    <span
                      style={{ color: pie.color }}
                      className="font-semibold"
                    >
                      ● {pie.name}
                    </span>
                    <span className="font-medium text-text-secondary">
                      {symbol}
                      {pie.value.toFixed(2)}
                    </span>
                  </div>
                ))}
              </div>
            </div>
          </div>
        </div>
      </ScrollReveal>
    </div>
  );
}

export default FinancialTransactions;
