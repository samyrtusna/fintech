import { MdTrendingUp, MdTrendingDown } from "react-icons/md";
import { useAppSelector } from "../state/stateHooks";

const Dashboard = () => {
  const aggregates = useAppSelector(
    (state) => state.financialAggregates.monthAggregates,
  );

  const symbol = useAppSelector(
    (state) => state.authUser.userInformations?.userCurrencies,
  )?.find((c) => c.isDefault === true)?.currencySymbol;

  const getAggValue = (name: string) =>
    aggregates!.find((ag) => ag.name === name)?.value ?? 0;
  const getAggVariation = (name: string) =>
    aggregates!.find((ag) => ag.name === name)?.variation ?? null;

  // Key metrics
  const totalIncome = getAggValue("TotalIncome");
  const incomeVar = getAggVariation("TotalIncome");

  const totalExpense = getAggValue("TotalExpense");
  const expenseVar = getAggVariation("TotalExpense");

  const totalInvestment = getAggValue("TotalInvestment");
  const investmentVar = getAggVariation("TotalInvestment");

  const netCashFlow = getAggValue("NetCashFlow");
  //   const netCashFlowVar = getAggVariation("NetCashFlow");

  const netBalance = getAggValue("NetBalance");
  //   const netBalanceVar = getAggVariation("NetBalance");

  // Ratios
  const expensesRatio = getAggValue("ExpensesRatio");
  const investmentRatio = getAggValue("InvestmentRatio");
  const debtRatio = getAggValue("DebtRatio");
  const savingsRate = getAggValue("SavingsRate");

  return (
    <div className="w-full max-w-7xl mx-auto p-4 md:p-6 space-y-6 bg-bg-primary text-text-primary">
      {/* Header Summary / Month Info */}
      <div className="flex flex-col md:flex-row justify-between items-start md:items-center gap-4 bg-form-bg p-6 border border-border-subtle rounded-md shadow-card">
        <div>
          <h1 className="text-2xl font-bold">Monthly Financial Overview</h1>
          <p className="text-sm text-text-secondary mt-1">
            Performance summary for{" "}
            {new Date(aggregates![0]?.date).toLocaleDateString("en-US", {
              month: "long",
              year: "numeric",
            })}
          </p>
        </div>
        <div className="flex gap-4">
          <div className="px-4 py-2 bg-bg-muted border border-border-subtle rounded-sm">
            <span className="text-xs text-text-secondary block">
              Net Balance
            </span>
            <span className="text-lg font-bold text-green-500">
              {symbol}
              {netBalance.toFixed(2)}
            </span>
          </div>
          <div className="px-4 py-2 bg-bg-muted border border-border-subtle rounded-sm">
            <span className="text-xs text-text-secondary block">
              Net Cash Flow
            </span>
            <span className="text-lg font-bold text-blue-500">
              {symbol}
              {netCashFlow.toFixed(2)}
            </span>
          </div>
        </div>
      </div>

      {/* Primary KPI Grid */}
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
        {/* Income Card */}
        <div className="p-5 bg-form-bg border border-border-subtle rounded-md shadow-card flex flex-col justify-between">
          <div>
            <span className="text-xs font-semibold text-text-secondary uppercase tracking-wider">
              Total Income
            </span>
            <div className="text-3xl font-extrabold mt-2">
              {symbol}
              {totalIncome.toFixed(2)}
            </div>
          </div>
          {incomeVar !== null && (
            <div
              className={`mt-4 flex items-center gap-1 text-xs font-medium ${incomeVar >= 0 ? "text-green-500" : "text-red-500"}`}
            >
              {incomeVar >= 0 ? (
                <MdTrendingUp className="size-4" />
              ) : (
                <MdTrendingDown className="size-4" />
              )}
              <span>
                {incomeVar > 0 ? `+${incomeVar}%` : `${incomeVar}%`} vs last
                month
              </span>
            </div>
          )}
        </div>

        {/* Expense Card */}
        <div className="p-5 bg-form-bg border border-border-subtle rounded-md shadow-card flex flex-col justify-between">
          <div>
            <span className="text-xs font-semibold text-text-secondary uppercase tracking-wider">
              Total Expenses
            </span>
            <div className="text-3xl font-extrabold mt-2">
              {symbol}
              {totalExpense.toFixed(2)}
            </div>
          </div>
          {expenseVar !== null && (
            <div
              className={`mt-4 flex items-center gap-1 text-xs font-medium ${expenseVar <= 0 ? "text-green-500" : "text-red-500"}`}
            >
              {expenseVar <= 0 ? (
                <MdTrendingDown className="size-4" />
              ) : (
                <MdTrendingUp className="size-4" />
              )}
              <span>
                {expenseVar > 0 ? `+${expenseVar}%` : `${expenseVar}%`} vs last
                month
              </span>
            </div>
          )}
        </div>

        {/* Investment Card */}
        <div className="p-5 bg-form-bg border border-border-subtle rounded-md shadow-card flex flex-col justify-between">
          <div>
            <span className="text-xs font-semibold text-text-secondary uppercase tracking-wider">
              Total Investments
            </span>
            <div className="text-3xl font-extrabold mt-2">
              {symbol}
              {totalInvestment.toFixed(2)}
            </div>
          </div>
          {investmentVar !== null && (
            <div
              className={`mt-4 flex items-center gap-1 text-xs font-medium ${investmentVar >= 0 ? "text-green-500" : "text-red-500"}`}
            >
              {investmentVar >= 0 ? (
                <MdTrendingUp className="size-4" />
              ) : (
                <MdTrendingDown className="size-4" />
              )}
              <span>{investmentVar}% vs last month</span>
            </div>
          )}
        </div>
      </div>

      {/* Financial Ratios & Health Indicators */}
      <div className="bg-form-bg p-6 border border-border-subtle rounded-md shadow-card">
        <h3 className="text-lg font-bold mb-4">Key Financial Ratios</h3>

        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
          <div className="p-4 bg-bg-muted border border-border-subtle rounded-sm">
            <span className="text-xs text-text-secondary">Expenses Ratio</span>
            <div className="text-xl font-bold mt-1">
              {expensesRatio.toFixed(2)}%
            </div>
            <div className="w-full bg-bg-primary h-1.5 rounded-full mt-3 overflow-hidden">
              <div
                className="bg-blue-500 h-full rounded-full"
                style={{ width: `${Math.min(expensesRatio, 100)}%` }}
              ></div>
            </div>
          </div>

          <div className="p-4 bg-bg-muted border border-border-subtle rounded-sm">
            <span className="text-xs text-text-secondary">
              Investment Ratio
            </span>
            <div className="text-xl font-bold mt-1">
              {investmentRatio.toFixed(2)}%
            </div>
            <div className="w-full bg-bg-primary h-1.5 rounded-full mt-3 overflow-hidden">
              <div
                className="bg-green-500 h-full rounded-full"
                style={{ width: `${Math.min(investmentRatio, 100)}%` }}
              ></div>
            </div>
          </div>

          <div className="p-4 bg-bg-muted border border-border-subtle rounded-sm">
            <span className="text-xs text-text-secondary">Savings Rate</span>
            <div className="text-xl font-bold mt-1">
              {savingsRate.toFixed(2)}%
            </div>
            <div className="w-full bg-bg-primary h-1.5 rounded-full mt-3 overflow-hidden">
              <div
                className="bg-yellow-500 h-full rounded-full"
                style={{ width: `${Math.min(savingsRate, 100)}%` }}
              ></div>
            </div>
          </div>

          <div className="p-4 bg-bg-muted border border-border-subtle rounded-sm">
            <span className="text-xs text-text-secondary">Debt Ratio</span>
            <div className="text-xl font-bold mt-1">
              {debtRatio.toFixed(2)}%
            </div>
            <div className="w-full bg-bg-primary h-1.5 rounded-full mt-3 overflow-hidden">
              <div
                className="bg-red-500 h-full rounded-full"
                style={{ width: `${Math.min(debtRatio, 100)}%` }}
              ></div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Dashboard;
