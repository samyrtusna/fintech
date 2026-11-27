import { useAppSelector } from "../state/stateHooks";
import { useCallback, useEffect, useMemo, useState } from "react";
import { useDispatch } from "react-redux";
import { Link } from "react-router-dom";
import { toast } from "sonner";
import { MdAccountBalanceWallet } from "react-icons/md";
import { GoGoal } from "react-icons/go";
import { IoMdAdd } from "react-icons/io";

import financialAggregatesService from "../API/Services/financialAggregatesService";
import budgetService from "../API/Services/budgetService";
import goalService from "../API/Services/goalService";

import { setMonthAggregates } from "../state/slices/financialAggregatesSlice";
import { setBudget } from "../state/slices/budgetSlice";
import { setGoal } from "../state/slices/goalSlice";

import { type MonthItem } from "../types/financialTransactionTypes";
import generateMonths from "../helpers/generateMonths";
import Dropdown from "../components/Dropdown";
import PieChatCard from "../components/PieChatCard";
import FormatData from "../helpers/lineChartDataformatter";
import { mergeSeriesData } from "../helpers/mergeSeriesData";
import LinesChart from "../components/LinesChart";
import userService from "../API/Services/userService";
import { setUserInformations } from "../state/slices/authSlice";
import Spinner from "../components/Spinner";

const Dashboard = () => {
  const dispatch = useDispatch();
  const userInformations = useAppSelector(
    (state) => state.authUser.userInformations,
  );
  const monthAggregates = useAppSelector(
    (state) => state.financialAggregates.monthAggregates,
  );
  const budgets = useAppSelector((state) => state.budgets.budgets);
  const goals = useAppSelector((state) => state.goals.goals);

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

  useEffect(() => {
    fetchUserInformations();
  }, [fetchUserInformations]);

  const symbol =
    userInformations?.userCurrencies?.find((c) => c.isDefault === true)
      ?.currency.symbol ?? "";

  const createdAt = userInformations?.createdAt;

  const months: MonthItem[] = useMemo(() => {
    return createdAt ? generateMonths(createdAt) : [];
  }, [createdAt]);
  const [selectedMonth, setSelectedMonth] = useState<MonthItem | undefined>(
    undefined,
  );

  useEffect(() => {
    if (months.length > 0 && !selectedMonth) {
      setSelectedMonth(months[0]);
    }
  }, [months, selectedMonth]);

  const fetchMonthData = useCallback(
    async (monthItem: MonthItem) => {
      const monthIso = monthItem.value.toISOString();
      await Promise.allSettled([
        financialAggregatesService
          .getByMonth(monthItem.value)
          .then((aggregates) => dispatch(setMonthAggregates(aggregates)))
          .catch((error) =>
            toast.error(
              error instanceof Error
                ? error.message
                : "Failed to fetch month aggregates",
            ),
          ),
        budgetService
          .getByMonthAsync({ month: monthIso })
          .then((fetchedBudgets) => dispatch(setBudget(fetchedBudgets)))
          .catch((error) =>
            toast.error(
              error instanceof Error
                ? error.message
                : "Failed to fetch month budgets",
            ),
          ),
        goalService
          .getByMonthAsync({ month: monthIso })
          .then((fetchedGoals) => dispatch(setGoal(fetchedGoals)))
          .catch((error) =>
            toast.error(
              error instanceof Error
                ? error.message
                : "Failed to fetch month goals",
            ),
          ),
      ]);
    },
    [dispatch],
  );

  useEffect(() => {
    if (!selectedMonth) {
      return;
    }
    fetchMonthData(selectedMonth);
  }, [fetchMonthData, selectedMonth]);

  // Key metrics
  const getAggregateAmount = (name: string) =>
    monthAggregates?.aggregates.find(
      (ag) => ag.name.toLowerCase() === name.toLowerCase(),
    )?.amount.amount ?? 0;

  const getAggregateVariation = (name: string) =>
    monthAggregates?.aggregates.find(
      (ag) => ag.name.toLowerCase() === name.toLowerCase(),
    )?.variation ?? 0;

  const totalIncome = getAggregateAmount("TotalIncome");
  const incomeVariation = getAggregateVariation("TotalIncome");

  const totalExpense = getAggregateAmount("TotalExpense");
  const expenseVariation = getAggregateVariation("TotalExpense");

  const incomeByCategory =
    monthAggregates?.aggregates.filter((t) => t.type === "Income") ?? [];
  const expenseByCategory =
    monthAggregates?.aggregates.filter((t) => t.type === "Expense") ?? [];

  // Ratios
  const expensesRatio = getAggregateAmount("ExpensesRatio");
  const investmentRatio = getAggregateAmount("InvestmentRatio");
  const debtRatio = getAggregateAmount("DebtRatio");
  const savingsRate = getAggregateAmount("SavingsRate");
  const essentialExpensesRatio = getAggregateAmount("EssentialExpenseRatio");

  // Budgets & Goals slices
  const globalBudget = budgets?.find((b) => b.type === "Global");
  const categoryBudgets = budgets?.filter((b) => b.type !== "Global") ?? [];
  const activeGoals = goals ?? [];
  const completedGoalsCount = activeGoals.filter(
    (g) => g.goalProgress >= 100,
  ).length;

  // Line chart variations
  const incomeVariationsData = monthAggregates?.periodicVariations?.find(
    (ag) => ag.name.toLowerCase() === "Income Variations".toLowerCase(),
  );
  const salaryVariationsData = monthAggregates?.periodicVariations?.find(
    (ag) => ag.name.toLowerCase() === "Salary Variations".toLowerCase(),
  );
  const expenseVariationsData = monthAggregates?.periodicVariations?.find(
    (ag) => ag.name.toLowerCase() === "Expense Variations".toLowerCase(),
  );

  const incomeSalaryData = mergeSeriesData([
    incomeVariationsData ? FormatData(incomeVariationsData) : null,
    salaryVariationsData ? FormatData(salaryVariationsData) : null,
  ]);
  const expenseData = mergeSeriesData([
    expenseVariationsData ? FormatData(expenseVariationsData) : null,
  ]);

  const incomeLineConfigs = [
    { dataKey: "Income Variations", name: "Income", stroke: "#036ffc" },
    { dataKey: "Salary Variations", name: "Salary", stroke: "#03fc0b" },
  ];
  const expenseLineConfigs = [
    { dataKey: "Expense Variations", name: "Expense", stroke: "#cc081c" },
  ];

  const getBudgetBarColor = (pct: number) => {
    if (pct >= 100) return "bg-red-500";
    if (pct >= 80) return "bg-amber-500";
    return "bg-btn-standard";
  };

  if (!userInformations || !selectedMonth) {
    return (
      <div className="flex min-h-dvh w-full justify-center items-center">
        <Spinner />
      </div>
    );
  }
  return (
    <div className="w-full mx-auto p-4 md:p-6 space-y-6 text-text-primary">
      {/* Header */}
      <div className="flex flex-col md:flex-row md:justify-between items-start md:items-center gap-4 bg-form-bg p-6 border-thin border-border-subtle rounded-md shadow-card">
        <div>
          <h1 className="text-xl md:text-2xl font-bold text-text-primary">
            Financial Overview
          </h1>
          <p className="text-xs md:text-sm text-text-secondary mt-1">
            Ratios, budget guardrails, goal progress, and cash-flow trends.
          </p>
        </div>

        <div className="w-full md:w-auto flex flex-col sm:flex-row sm:items-center gap-3">
          <div className="flex items-center gap-2">
            <span className="text-sm text-text-secondary whitespace-nowrap">
              Summary for
            </span>
            <div className="w-40">
              <Dropdown<MonthItem>
                selectedValue={selectedMonth}
                options={months}
                onSelect={(selected) => setSelectedMonth(selected)}
              />
            </div>
          </div>
        </div>
      </div>

      {/* Ratios Grid */}
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
        <div className="p-4 bg-red-500 text-white border-thin border-red-600/30 rounded-md shadow-card">
          <span className="text-sm font-medium text-white/90">
            Expenses Ratio
          </span>
          <div className="text-xl font-bold mt-1">
            {expensesRatio.toFixed(2)}%
          </div>
          <div className="w-full bg-black/25 h-1.5 rounded-full mt-3 overflow-hidden">
            <div
              className="bg-yellow-300 h-full rounded-full"
              style={{ width: `${Math.min(expensesRatio, 100)}%` }}
            />
          </div>
        </div>

        <div className="p-4 bg-emerald-600 dark:bg-emerald-500 text-white border-thin border-emerald-700/30 rounded-md shadow-card">
          <span className="text-sm font-medium text-white/90">
            Investment Ratio
          </span>
          <div className="text-xl font-bold mt-1">
            {investmentRatio.toFixed(2)}%
          </div>
          <div className="w-full bg-black/25 h-1.5 rounded-full mt-3 overflow-hidden">
            <div
              className="bg-blue-950 h-full rounded-full"
              style={{ width: `${Math.min(investmentRatio, 100)}%` }}
            />
          </div>
        </div>

        <div className="p-4 bg-btn-standard text-btn-standard-text border-thin border-border-subtle rounded-md shadow-card">
          <span className="text-sm font-medium opacity-90">
            {savingsRate !== 0 ? "Savings Ratio" : "Important Expense Ratio"}
          </span>
          <div className="text-xl font-bold mt-1">
            {(savingsRate !== 0 ? savingsRate : essentialExpensesRatio).toFixed(
              2,
            )}
            %
          </div>
          <div className="w-full bg-black/20 h-1.5 rounded-full mt-3 overflow-hidden">
            <div
              className="bg-emerald-200 h-full rounded-full"
              style={{
                width: `${Math.min(
                  savingsRate !== 0 ? savingsRate : essentialExpensesRatio,
                  100,
                )}%`,
              }}
            />
          </div>
        </div>

        <div className="p-4 bg-orange-500 text-white border-thin border-orange-600/30 rounded-md shadow-card">
          <span className="text-sm font-medium text-white/90">Debt Ratio</span>
          <div className="text-xl font-bold mt-1">{debtRatio.toFixed(2)}%</div>
          <div className="w-full bg-black/25 h-1.5 rounded-full mt-3 overflow-hidden">
            <div
              className="bg-white h-full rounded-full"
              style={{ width: `${Math.min(debtRatio, 100)}%` }}
            />
          </div>
        </div>
      </div>

      {/* Budgets & Goals Section */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Budgets Card */}
        <div className="p-5 bg-bg-surface border-thin border-border-subtle rounded-md shadow-card flex flex-col justify-between space-y-4">
          <div className="flex items-center justify-between">
            <div className="flex items-center gap-2.5">
              <div className="p-2 bg-bg-muted border-thin border-border-subtle rounded-sm">
                <MdAccountBalanceWallet className="size-5 text-text-secondary" />
              </div>
              <div>
                <h2 className="text-lg font-bold text-text-primary">Budgets</h2>
                <p className="text-xs text-text-secondary">
                  Spending limits & consumption for {selectedMonth.shortLabel}
                </p>
              </div>
            </div>
            <Link
              to="/budgets"
              className="text-xs font-semibold text-text-secondary hover:underline"
            >
              Manage all
            </Link>
          </div>

          {!budgets || budgets.length === 0 ? (
            <div className="p-6 bg-bg-secondary border-thin border-border-subtle rounded-md text-center space-y-3">
              <p className="text-sm text-text-secondary">
                No budgets set for this month yet.
              </p>
              <Link
                to="/budget/new"
                className="inline-flex items-center gap-1 px-4 py-2 text-xs font-bold rounded-sm bg-btn-standard hover:bg-btn-standard-hover text-btn-standard-text shadow-subtle"
              >
                <IoMdAdd className="size-4" />
                Create a budget
              </Link>
            </div>
          ) : (
            <div className="space-y-4">
              {globalBudget && (
                <div className="p-3.5 rounded-md bg-bg-secondary border-thin border-border-subtle space-y-2">
                  <div className="flex justify-between items-center text-sm">
                    <span className="font-bold text-text-primary">
                      Global Budget
                    </span>
                    <span className="font-bold text-text-secondary">
                      {globalBudget.budgetConsumptionPercentage.toFixed(0)}%
                    </span>
                  </div>
                  <div className="w-full bg-bg-muted h-2 rounded-full overflow-hidden">
                    <div
                      className={`${getBudgetBarColor(globalBudget.budgetConsumptionPercentage)} h-full rounded-full transition-all`}
                      style={{
                        width: `${Math.min(globalBudget.budgetConsumptionPercentage, 100)}%`,
                      }}
                    />
                  </div>
                  <div className="flex justify-between text-xs text-text-secondary">
                    <span>
                      Spent: {globalBudget.amount.currency.symbol}
                      {Number(globalBudget.budgetConsumption).toFixed(2)}
                    </span>
                    <span>
                      Limit: {globalBudget.amount.currency.symbol}
                      {globalBudget.amount.amount.toFixed(2)}
                    </span>
                  </div>
                </div>
              )}

              {categoryBudgets.length > 0 && (
                <div className="space-y-2.5">
                  <span className="text-xs font-semibold text-text-muted uppercase tracking-wide">
                    Category Budgets ({categoryBudgets.length})
                  </span>
                  {categoryBudgets.slice(0, 3).map((b) => (
                    <div
                      key={b.id}
                      className="p-3 rounded-md bg-bg-muted/70 border-thin border-border-subtle space-y-1.5"
                    >
                      <div className="flex justify-between text-sm">
                        <span className="font-semibold text-text-primary">
                          {b.categoryName}
                        </span>
                        <span className="font-semibold text-text-secondary">
                          {b.budgetConsumptionPercentage.toFixed(0)}%
                        </span>
                      </div>
                      <div className="w-full bg-bg-secondary h-1.5 rounded-full overflow-hidden">
                        <div
                          className={`${getBudgetBarColor(b.budgetConsumptionPercentage)} h-full rounded-full`}
                          style={{
                            width: `${Math.min(b.budgetConsumptionPercentage, 100)}%`,
                          }}
                        />
                      </div>
                      <div className="flex justify-between text-xs text-text-muted">
                        <span>
                          {b.amount.currency.symbol}
                          {Number(b.budgetConsumption).toFixed(2)} used
                        </span>
                        <span>
                          {b.amount.currency.symbol}
                          {b.amount.amount.toFixed(2)}
                        </span>
                      </div>
                    </div>
                  ))}
                </div>
              )}
            </div>
          )}
        </div>

        {/* Goals Card */}
        <div className="p-5 bg-bg-surface border-thin border-border-subtle rounded-md shadow-card flex flex-col justify-between space-y-4">
          <div className="flex items-center justify-between">
            <div className="flex items-center gap-2.5">
              <div className="p-2 bg-bg-muted border-thin border-border-subtle rounded-sm">
                <GoGoal className="size-5 text-emerald-600 dark:text-emerald-400" />
              </div>
              <div>
                <h2 className="text-lg font-bold text-text-primary">
                  Financial Goals
                </h2>
                <p className="text-xs text-text-secondary">
                  {activeGoals.length > 0
                    ? `${completedGoalsCount} of ${activeGoals.length} targets achieved`
                    : `Targets for ${selectedMonth.shortLabel}`}
                </p>
              </div>
            </div>
            <Link
              to="/goals"
              className="text-xs font-semibold text-text-secondary hover:underline"
            >
              View all
            </Link>
          </div>

          {activeGoals.length === 0 ? (
            <div className="p-6 bg-bg-secondary border-thin border-border-subtle rounded-md text-center space-y-3">
              <p className="text-sm text-text-secondary">
                No monthly goals set yet. Create a savings, investment, or debt
                target to track progress.
              </p>
              <Link
                to="/goal/new"
                className="inline-flex items-center gap-1 px-4 py-2 text-xs font-bold rounded-sm bg-emerald-600 hover:bg-emerald-700 text-white shadow-subtle"
              >
                <IoMdAdd className="size-4" />
                Create a goal
              </Link>
            </div>
          ) : (
            <div className="space-y-3">
              {activeGoals.slice(0, 4).map((g) => {
                const progress = Math.min(g.goalProgress, 100);
                const isAchieved = g.goalProgress >= 100;
                return (
                  <div
                    key={g.id}
                    className="p-3 rounded-md bg-bg-secondary border-thin border-border-subtle space-y-1.5"
                  >
                    <div className="flex justify-between items-center text-sm">
                      <span className="font-bold text-text-primary truncate">
                        {g.title}
                      </span>
                      <span
                        className={`text-xs font-bold px-2 py-0.5 rounded-full ${
                          isAchieved
                            ? "bg-emerald-500/15 text-emerald-600 dark:text-emerald-400"
                            : "bg-bg-muted text-text-secondary"
                        }`}
                      >
                        {g.goalProgress.toFixed(0)}%
                      </span>
                    </div>
                    <div className="w-full bg-bg-muted h-2 rounded-full overflow-hidden">
                      <div
                        className={`h-full rounded-full ${
                          isAchieved ? "bg-emerald-500" : "bg-btn-standard"
                        }`}
                        style={{ width: `${progress}%` }}
                      />
                    </div>
                    <div className="flex justify-between text-xs text-text-secondary">
                      <span>{g.category?.name ?? g.targetType}</span>
                      <span>
                        Target: {g.targetAmount.currency.symbol}
                        {g.targetAmount.amount.toFixed(2)}
                      </span>
                    </div>
                  </div>
                );
              })}
            </div>
          )}
        </div>
      </div>

      {/* Charts Row */}
      <div className="w-full flex flex-col md:flex-row gap-4">
        <div className="w-full md:w-1/2 flex flex-col gap-4">
          <div className="w-full p-3 md:p-5 bg-bg-surface border-thin border-border-subtle rounded-md shadow-card flex flex-col justify-between">
            <PieChatCard
              dataList={incomeByCategory}
              symbol={symbol}
              total={totalIncome}
              variation={incomeVariation}
              aggregate="income"
            />
          </div>
          <div className="w-full p-3 md:p-5 bg-bg-surface border-thin border-border-subtle rounded-md shadow-card flex flex-col justify-between">
            <LinesChart
              data={incomeSalaryData}
              lines={incomeLineConfigs}
              xAxisDataKey="label"
            />
          </div>
        </div>

        <div className="w-full md:w-1/2 flex flex-col gap-4">
          <div className="w-full p-3 md:p-5 bg-bg-surface border-thin border-border-subtle rounded-md shadow-card flex flex-col justify-between">
            <PieChatCard
              dataList={expenseByCategory}
              symbol={symbol}
              total={totalExpense}
              variation={expenseVariation}
              aggregate="expense"
            />
          </div>
          <div className="w-full p-3 md:p-5 bg-bg-surface border-thin border-border-subtle rounded-md shadow-card flex flex-col justify-between">
            <LinesChart
              data={expenseData}
              lines={expenseLineConfigs}
              xAxisDataKey="label"
            />
          </div>
        </div>
      </div>
    </div>
  );
};

export default Dashboard;
