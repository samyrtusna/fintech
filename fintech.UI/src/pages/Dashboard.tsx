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

  const monthAggregates = useAppSelector(
    (state) => state.financialAggregates.monthAggregates,
  );

  const symbol = useAppSelector(
    (state) => state.authUser.userInformations?.userCurrencies,
  )?.find((c) => c.isDefault === true)?.currency.symbol; //refactor currency symbol origin

  const dispatch = useDispatch();

  const months = generateMonths(userInformations!.createdAt);

  const [selectedMonth, setSelectedMonth] = useState<MonthItem>(months[0]);

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

  useEffect(() => {
    fetchMonthAggregates(selectedMonth.value);
  }, [fetchMonthAggregates, selectedMonth.value]);

  // Key metrics
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

  // const totalInvestment =
  //   monthAggregates?.aggregates.find(
  //     (ag) => ag.name.toLowerCase() === "TotalInvestment".toLowerCase(),
  //   )?.amount.amount ?? 0;
  // const investmentVariation =
  //   monthAggregates?.aggregates.find(
  //     (ag) => ag.name.toLowerCase() === "TotalInvestment".toLowerCase(),
  //   )?.variation ?? 0;

  const netCashFlow =
    monthAggregates?.aggregates.find(
      (ag) => ag.name.toLowerCase() === "NetCashFlow".toLowerCase(),
    )?.amount.amount ?? 0;
  // const netCashFlowVariation =
  //   monthAggregates?.aggregates.find(
  //     (ag) => ag.name.toLowerCase() === "NetCashFlow".toLowerCase(),
  //   )?.variation ?? 0;

  const netBalance =
    monthAggregates?.aggregates.find(
      (ag) => ag.name.toLowerCase() === "NetBalance".toLowerCase(),
    )?.amount.amount ?? 0;
  // const netBalanceVariation =
  //   monthAggregates?.aggregates.find(
  //     (ag) => ag.name.toLowerCase() === "NetBalance".toLowerCase(),
  //   )?.variation ?? 0;

  // const totalInflow =
  //   monthAggregates?.aggregates.find(
  //     (ag) => ag.name.toLowerCase() === "TotalInflow".toLowerCase(),
  //   )?.amount.amount ?? 0;
  // const totalOutflow =
  //   monthAggregates?.aggregates.find(
  //     (ag) => ag.name.toLowerCase() === "TotalOutflow".toLowerCase(),
  //   )?.amount.amount ?? 0;

  // const totalEssentialExpenses = aggregates?.totals.find(
  //   (t) => t.name === "Important Expenses",
  // )?.amount;

  const incomeByCategory = monthAggregates?.aggregates.filter(
    (t) => t.type === "Income",
  );

  const expenseByCategory = monthAggregates?.aggregates.filter(
    (t) => t.type === "Expense",
  );

  // Ratios
  const expensesRatio =
    monthAggregates?.aggregates.find(
      (ag) => ag.name.toLowerCase() === "ExpensesRatio".toLowerCase(),
    )?.amount.amount ?? 0;
  const investmentRatio =
    monthAggregates?.aggregates.find(
      (ag) => ag.name.toLowerCase() === "InvestmentRatio".toLowerCase(),
    )?.amount.amount ?? 0;
  const debtRatio =
    monthAggregates?.aggregates.find(
      (ag) => ag.name.toLowerCase() === "DebtRatio".toLowerCase(),
    )?.amount.amount ?? 0;
  const savingsRate =
    monthAggregates?.aggregates.find(
      (ag) => ag.name.toLowerCase() === "SavingsRate".toLowerCase(),
    )?.amount.amount ?? 0;
  const essentialExpensesRatio =
    monthAggregates?.aggregates.find(
      (ag) => ag.name.toLowerCase() === "EssentialExpenseRatio".toLowerCase(),
    )?.amount.amount ?? 0;
  // const debtPaymentRatio = aggregates?.aggregates.debtPaymentRatio;

  // Aggregate Variations By Month

  const incomeVariationsData = monthAggregates?.periodicVariations?.find(
    (ag) => ag.name.toLowerCase() === "Income Variations".toLowerCase(),
  );

  const salaryVariationsData = monthAggregates?.periodicVariations?.find(
    (ag) => ag.name.toLowerCase() === "Salary Variations".toLowerCase(),
  );

  const expenseVariationsData = monthAggregates?.periodicVariations?.find(
    (ag) => ag.name.toLowerCase() === "Expense Variations".toLowerCase(),
  );

  // const investmentVarationsData = monthAggregates?.periodicVariations?.find(
  //   (ag) => ag.name.toLowerCase() === "Investment Variations".toLowerCase(),
  // );

  const incomeLineChartData = incomeVariationsData
    ? FormatData(incomeVariationsData)
    : null;
  const salaryLineChartData = salaryVariationsData
    ? FormatData(salaryVariationsData)
    : null;
  const expenseLineChartData = expenseVariationsData
    ? FormatData(expenseVariationsData)
    : null;

  const incomeSalaryData = mergeSeriesData([
    incomeLineChartData,
    salaryLineChartData,
  ]);

  const expenseData = mergeSeriesData([expenseLineChartData]);

  const incomeLineConfigs = [
    { dataKey: "Income Variations", name: "Income", stroke: "#036ffc" },
    { dataKey: "Salary Variations", name: "Salary", stroke: "#03fc0b" },
  ];

  const expenseLineConfigs = [
    {
      dataKey: "Expense Variations",
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
