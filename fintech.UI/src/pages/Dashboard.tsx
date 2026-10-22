import { useAppSelector } from "../state/stateHooks";
import { useCallback, useEffect, useState } from "react";
import { useDispatch } from "react-redux";
import { toast } from "sonner";
import financialAggregatesService from "../API/Services/financialAggregatesService";
import { setMonthAggregates } from "../state/slices/financialAggregatesSlice";
import { type MonthItem } from "../types/financialTransactionTypes";
import generateMonths from "../helpers/generateMonths";
import Dropdown from "../components/Dropdown";
import PieChatCard from "../components/PieChatCard";

import FormatData from "../helpers/lineChartDataformatter";
import { mergeSeriesData } from "../helpers/mergeSeriesData";
import LinesChart from "../components/LinesChart";

const Dashboard = () => {
  const userInformations = useAppSelector(
    (state) => state.authUser.userInformations,
  );

  const aggregates = useAppSelector(
    (state) => state.financialAggregates.monthAggregates,
  );

  const symbol = useAppSelector(
    (state) => state.authUser.userInformations?.userCurrencies,
  )?.find((c) => c.isDefault === true)?.currencySymbol;

  const dispatch = useDispatch();

  const months = generateMonths(userInformations!.createdAt);

  const [selectedMonth, setSelectedMonth] = useState<MonthItem>(months[0]);

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

  useEffect(() => {
    fetchMonthAggregates(selectedMonth.value);
  }, [fetchMonthAggregates, selectedMonth.value]);

  // Key metrics
  const totalIncome = aggregates?.totals.find(
    (t) => t.name === "Income",
  )?.amount;
  const incomeVariation = aggregates?.variations.incomeVariation;

  const totalExpense = aggregates?.totals.find(
    (t) => t.name === "Expense",
  )?.amount;
  const expenseVariation = aggregates?.variations.expensesVariation;

  // const totalInvestment = aggregates?.totals.find(
  //   (t) => t.name === "Investment",
  // )?.amount;
  // const investmentVariation = aggregates?.variations.investmentVariation;

  const netCashFlow = aggregates?.aggregates.netCashFlow;
  // const netCashFlowVariation = aggregates?.variations.netCashFlowVariation;

  const netBalance = aggregates?.aggregates.netBalance;
  // const netBalanceVariation = aggregates?.variations.netBalanceVariation;

  // const totalInflow = aggregates?.aggregates.totalInflow;
  // const totalOutflow = aggregates?.aggregates.totalOutflow;

  // const totalEssentialExpenses = aggregates?.totals.find(
  //   (t) => t.name === "Important Expenses",
  // )?.amount;

  const incomeByCategory = aggregates?.totals.filter(
    (t) => t.type === "Income",
  );

  const expenseByCategory = aggregates?.totals.filter(
    (t) => t.type === "Expense" && t.name !== "Important Expenses",
  );

  // Ratios
  const expensesRatio = aggregates?.aggregates.expensesRatio;
  const investmentRatio = aggregates?.aggregates.investmentRatio;
  const debtRatio = aggregates?.aggregates.debtRatio;
  const savingsRate = aggregates?.aggregates.savingsRate;
  const essentialExpensesRatio = aggregates?.aggregates.essentialExpensesRatio;
  // const debtPaymentRatio = aggregates?.aggregates.debtPaymentRatio;

  // Aggregate Variations By Month

  const incomeVariationsData =
    aggregates?.aggregatesVariationsByMonth.filter(
      (ag) => ag.type === "Income" && ag.categoryName === null,
    ) ?? [];

  const salaryVariationsData =
    aggregates?.aggregatesVariationsByMonth.filter(
      (ag) => ag.categoryName === "Salary",
    ) ?? [];

  const expenseVariationsData =
    aggregates?.aggregatesVariationsByMonth.filter(
      (ag) => ag.type === "Expense" && ag.categoryName === null,
    ) ?? [];

  const incomeLineChartData = FormatData(incomeVariationsData);
  const salaryLineChartData = FormatData(salaryVariationsData);
  const expenseLineChartData = FormatData(expenseVariationsData);

  const incomeSalaryData = mergeSeriesData([
    incomeLineChartData,
    salaryLineChartData,
  ]);

  const expenseData = mergeSeriesData([expenseLineChartData]);

  const incomeLineConfigs = [
    { dataKey: "Income", name: "Income", stroke: "#036ffc" },
    { dataKey: "Salary", name: "Salary", stroke: "#03fc0b" },
  ];

  const expenseLineConfigs = [
    {
      dataKey: "Expense",
      name: "Expense",
      stroke: "#cc081c",
    },
  ];

  const handleMonthClick = (month: MonthItem) => {
    setSelectedMonth(month);
  };

  return (
    <div className="w-full mx-auto p-4 md:p-6 space-y-6 bg-bg-primary text-text-primary">
      <div className="flex flex-col md:flex-row justify-between items-start md:items-center gap-4 bg-form-bg p-6 border border-border-subtle rounded-md shadow-card">
        <div>
          <h1 className="text-2xl font-bold">Monthly Financial Overview</h1>
          <div className="flex">
            <p className="text-sm text-text-surface mt-2 mr-1">
              Performance summary for{" "}
            </p>
            <div className="w-[40%] pt-1">
              <Dropdown<MonthItem>
                selectedValue={selectedMonth}
                options={months}
                onSelect={(selected) => handleMonthClick(selected)}
              />
            </div>
          </div>
        </div>
        <div className="flex gap-4">
          <div className="px-4 py-2 bg-bg-muted border border-border-subtle rounded-sm">
            <span className="text-sm text-text-secondary block">
              Net Balance
            </span>
            <span className="text-lg font-bold text-green-500">
              {symbol}
              {netBalance?.toFixed(2)}
            </span>
          </div>
          <div className="px-4 py-2 bg-bg-muted border border-border-subtle rounded-sm">
            <span className="text-sm text-text-secondary block">
              Net Cash Flow
            </span>
            <span className="text-lg font-bold text-blue-500">
              {symbol}
              {netCashFlow?.toFixed(2)}
            </span>
          </div>
        </div>
      </div>

      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
        <div className="p-4 bg-red-500 border border-border-subtle rounded-sm">
          <span className="text-sm text-gray-50">Expenses Ratio</span>
          <div className="text-xl font-bold mt-1">
            {expensesRatio?.toFixed(2)}%
          </div>
          <div className="w-full bg-bg-primary h-1.5 rounded-full mt-3 overflow-hidden">
            <div
              className="bg-yellow-300 h-full rounded-full"
              style={{ width: `${Math.min(expensesRatio!, 100)}%` }}
            ></div>
          </div>
        </div>

        <div className="p-4 bg-green-500 border border-border-subtle rounded-sm">
          <span className="text-sm text-gray-50">Investment Ratio</span>
          <div className="text-xl font-bold mt-1">
            {investmentRatio?.toFixed(2)}%
          </div>
          <div className="w-full bg-bg-primary h-1.5 rounded-full mt-3 overflow-hidden">
            <div
              className="bg-blue-800 h-full rounded-full"
              style={{ width: `${Math.min(investmentRatio!, 100)}%` }}
            ></div>
          </div>
        </div>

        <div className="p-4 bg-blue-500 border border-border-subtle rounded-sm">
          <span className="text-sm text-gray-50">
            {savingsRate !== 0 ? "Savings Ratio" : "Important Expense Ratio"}
          </span>
          <div className="text-xl font-bold mt-1">
            {savingsRate && savingsRate !== 0
              ? savingsRate.toFixed(2)
              : essentialExpensesRatio?.toFixed(2)}
            %
          </div>
          <div className="w-full bg-bg-primary h-1.5 rounded-full mt-3 overflow-hidden">
            <div
              className="bg-red-500 h-full rounded-full"
              style={{
                width: `${savingsRate && savingsRate !== 0 ? Math.min(savingsRate, 100) : Math.min(essentialExpensesRatio!, 100)}%`,
              }}
            ></div>
          </div>
        </div>

        <div className="p-4 bg-orange-500 border border-border-subtle rounded-sm">
          <span className="text-sm text-gray-50">Debt Ratio</span>
          <div className="text-xl font-bold mt-1">{debtRatio?.toFixed(2)}%</div>
          <div className="w-full bg-bg-primary h-1.5 rounded-full mt-3 overflow-hidden">
            <div
              className="bg-green-500 h-full rounded-full"
              style={{ width: `${Math.min(debtRatio!, 100)}%` }}
            ></div>
          </div>
        </div>
      </div>
      <div className="w-full flex flex-col md:flex-row gap-4">
        <div className="w-full md:w-1/2 flex flex-col gap-4">
          <div className="w-full p-2 md:p-5 bg-bg-secondary  border border-border-subtle rounded-md shadow-card flex flex-col justify-between">
            <PieChatCard
              dataList={incomeByCategory!}
              symbol={symbol!}
              total={totalIncome!}
              variation={incomeVariation!}
              aggregate="income"
            />
          </div>
          <div className="w-full p-2 md:p-5 bg-bg-contrast  border border-border-subtle rounded-md shadow-card flex flex-col justify-between">
            <LinesChart
              data={incomeSalaryData}
              lines={incomeLineConfigs}
              xAxisDataKey="label"
            />
          </div>
        </div>

        <div className="w-full md:w-1/2 flex flex-col gap-4">
          <div className="w-full  p-2 md:p-5 bg-bg-secondary  border border-border-subtle rounded-md shadow-card flex flex-col justify-between">
            <PieChatCard
              dataList={expenseByCategory!}
              symbol={symbol!}
              total={totalExpense!}
              variation={expenseVariation!}
              aggregate="expense"
            />
          </div>
          <div className="w-full  p-2 md:p-5 bg-bg-contrast  border border-border-subtle rounded-md shadow-card flex flex-col justify-between">
            <LinesChart
              data={expenseData}
              lines={expenseLineConfigs}
              xAxisDataKey="label"
            />
          </div>
        </div>
      </div>

      {/* Investment Card */}

      {/* Financial Ratios & Health Indicators */}
    </div>
  );
};

export default Dashboard;
