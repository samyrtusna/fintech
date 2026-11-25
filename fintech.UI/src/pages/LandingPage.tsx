import { Link } from "react-router-dom";
import { AiOutlineTransaction } from "react-icons/ai";
import { IoMdAdd } from "react-icons/io";
import { MdAccountBalanceWallet, MdPieChart } from "react-icons/md";
import { useAppSelector } from "../state/stateHooks";
import { GoGoal } from "react-icons/go";
import ScrollReveal from "../components/ScrollReveal";

function LandingPage() {
  const user = useAppSelector((state) => state.authUser.userInformations);
  const symbol =
    user?.userCurrencies.find((c) => c.isDefault)?.currency.symbol ?? "";

  const globalAggregates = useAppSelector(
    (state) => state.financialAggregates?.globalAggregates,
  );
  const monthAggregates = useAppSelector(
    (state) => state.financialAggregates.monthAggregates,
  );
  const budgets = useAppSelector((state) => state.budgets.budgets);
  const goals = useAppSelector((state) => state.goals.goals);
  const recentTransactions =
    useAppSelector((state) => state.transactions.transactions?.items)?.slice(
      0,
      3,
    ) ?? [];

  const getGlobalVal = (name: string) =>
    globalAggregates?.find((ag) => ag.name.toLowerCase() === name.toLowerCase())
      ?.amount.amount ?? 0;

  const getMonthVal = (name: string) =>
    monthAggregates?.aggregates.find(
      (ag) => ag.name.toLowerCase() === name.toLowerCase(),
    )?.amount.amount ?? 0;

  const globalCashFlow = getGlobalVal("GlobalNetCashFlow");
  const globalSavings = getGlobalVal("GlobalTotalSavings");
  const globalInvestment = getGlobalVal("GlobalTotalInvestment");
  const globalDebt = getGlobalVal("GlobalActiveDebt");

  const totalIncome = getMonthVal("TotalIncome");
  const totalExpense = getMonthVal("TotalExpense");
  const savingsRate = getMonthVal("SavingsRate");

  const globalBudget = budgets?.find((b) => b.type === "Global");
  const activeGoalsCount = goals?.length ?? 0;
  const avgGoalProgress =
    activeGoalsCount > 0
      ? goals!.reduce((acc, g) => acc + g.goalProgress, 0) / activeGoalsCount
      : 0;
  return (
    <>
      {!user ? (
        <div className="relative min-h-dvh w-full bg-bg-primary text-text-primary flex flex-col items-center">
          <ScrollReveal>
            <section className="w-full max-w-5xl px-6 py-20 flex flex-col items-center text-center mt-10">
              <div className="inline-flex items-center gap-2 px-3 py-1 mb-6 rounded-full bg-bg-muted border border-border-subtle text-xs font-semibold">
                <span className="h-2 w-2 rounded-full bg-green-500 animate-pulse"></span>
                Your Personal Finances Dashboard
              </div>

              <h1 className="text-4xl md:text-6xl font-extrabold tracking-tight max-w-3xl leading-tight">
                A new month,{" "}
                <span className="text-blue-500">
                  a clearer financial picture.
                </span>
              </h1>

              <p className="mt-6 text-lg md:text-xl text-text-secondary max-w-2xl">
                Take control of your cash flow, monitor savings and investments,
                and keep your debt under control as each month unfolds.
              </p>

              <div className="mt-10 flex flex-col sm:flex-row gap-4 w-full justify-center">
                <Link
                  to="/register"
                  className="px-8 py-3 rounded-sm bg-blue-500 hover:bg-blue-600 text-bg-contrast font-bold shadow-card transition-colors"
                >
                  Get Started Free
                </Link>
                <Link
                  to="/login"
                  className="px-8 py-3 rounded-sm bg-form-bg border border-border-subtle hover:bg-bg-muted font-bold shadow-card transition-colors"
                >
                  Log In
                </Link>
              </div>
            </section>
          </ScrollReveal>

          <ScrollReveal>
            <section className="w-full max-w-6xl px-6 py-12 grid grid-cols-1 md:grid-cols-3 gap-6">
              {/* Card 1 */}
              <div className="p-6 bg-form-bg border border-border-subtle rounded-md shadow-card flex flex-col items-start">
                <div className="p-3 bg-blue-500/10 rounded-sm mb-4">
                  <MdAccountBalanceWallet className="size-6 text-blue-500" />
                </div>
                <h3 className="text-xl font-semibold mb-2">
                  Global & Monthly Aggregates
                </h3>
                <p className="text-sm text-text-secondary">
                  Instantly track net cash flow, active debts, savings, and
                  total investment metrics across custom periods.
                </p>
              </div>

              {/* Card 2 */}
              <div className="p-6 bg-form-bg border border-border-subtle rounded-md shadow-card flex flex-col items-start">
                <div className="p-3 bg-green-500/10 rounded-sm mb-4">
                  <AiOutlineTransaction className="size-6 text-green-500" />
                </div>
                <h3 className="text-xl font-semibold mb-2">
                  Smart Transactions
                </h3>
                <p className="text-sm text-text-secondary">
                  Log, categorize, and filter income, expenses, and loan
                  repayments with essential vs. non-essential tagging.
                </p>
              </div>

              {/* Card 3 */}
              <div className="p-6 bg-form-bg border border-border-subtle rounded-md shadow-card flex flex-col items-start">
                <div className="p-3 bg-yellow-500/10 rounded-sm mb-4">
                  <MdPieChart className="size-6 text-yellow-500" />
                </div>
                <h3 className="text-xl font-semibold mb-2">
                  Visual Breakdowns
                </h3>
                <p className="text-sm text-text-secondary">
                  Understand your spending distribution dynamically using
                  built-in interactive charts and ratio indicators.
                </p>
              </div>
            </section>
          </ScrollReveal>

          <ScrollReveal>
            <section className="w-full max-w-5xl px-6 my-16">
              <div className="p-8 md:p-12 bg-form-bg border border-border-subtle rounded-lg shadow-card flex flex-col md:flex-row items-center justify-between gap-8">
                <div className="space-y-3 text-center md:text-left">
                  <h2 className="text-2xl md:text-3xl font-bold">
                    Ready to take control?
                  </h2>
                  <p className="text-text-secondary text-sm md:text-base">
                    Small steps today. Better financial insights tomorrow.
                  </p>
                </div>
                <Link
                  to="/register"
                  className="px-6 py-3 bg-green-500 text-gray-50 font-bold rounded-sm shadow-card hover:bg-green-600 transition-colors whitespace-nowrap flex items-center gap-2"
                >
                  <IoMdAdd className="size-5" />
                  Create Your Account
                </Link>
              </div>
            </section>
          </ScrollReveal>
        </div>
      ) : (
        <div className="w-full max-w-6xl mx-auto p-4 md:p-6 space-y-6 bg-bg-primary text-text-primary">
          <ScrollReveal>
            <section className="p-6 bg-form-bg border border-border-subtle rounded-md shadow-card flex flex-col lg:flex-row lg:items-center lg:justify-between gap-4">
              <div>
                <div className="inline-flex items-center gap-2 px-3 py-1 mb-2 rounded-full bg-bg-muted border border-border-subtle text-xs font-semibold">
                  <span className="h-2 w-2 rounded-full bg-green-500" />
                  Active Workspace
                </div>
                <h1 className="text-2xl md:text-3xl font-extrabold tracking-tight">
                  Welcome back{user?.username ? `, ${user.username}` : ""}
                </h1>
                <p className="text-sm text-text-secondary mt-1">
                  A clear snapshot of your cash flow, monthly guardrails, and
                  goals.
                </p>
              </div>

              <div className="flex gap-3">
                <Link
                  to="/financialTransactions/new"
                  className="inline-flex items-center gap-1.5 px-1 sm:px-4 py-2.5 rounded-sm bg-blue-500 hover:bg-blue-600 text-bg-contrast text-xs sm:text-sm font-bold shadow-card transition-colors"
                >
                  <IoMdAdd className="size-5" />
                  Log Transaction
                </Link>
                <Link
                  to="/dashboard"
                  className="inline-flex items-center gap-1.5 px-1 sm:px-4 py-2.5 rounded-sm bg-bg-secondary border border-border-subtle hover:bg-bg-muted text-xs sm:text-sm font-semibold transition-colors"
                >
                  <MdPieChart className="size-4 text-yellow-500" />
                  Full Analytics
                </Link>
              </div>
            </section>
          </ScrollReveal>

          <ScrollReveal>
            <section className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
              <div className="p-4 bg-bg-secondary border border-border-subtle rounded-md shadow-card">
                <span className="text-xs text-text-secondary">
                  Global Net Cash Flow
                </span>
                <div className="text-xl font-bold mt-1">
                  {symbol}
                  {globalCashFlow.toFixed(2)}
                </div>
                <span className="text-xs text-text-muted">
                  Month: {symbol}
                  {(totalIncome - totalExpense).toFixed(2)} net
                </span>
              </div>

              <div className="p-4 bg-bg-secondary border border-border-subtle rounded-md shadow-card">
                <span className="text-xs text-text-secondary">
                  Total Savings
                </span>
                <div className="text-xl font-bold mt-1 text-green-500">
                  {symbol}
                  {globalSavings.toFixed(2)}
                </div>
                <span className="text-xs text-text-muted">
                  Savings rate: {savingsRate.toFixed(1)}%
                </span>
              </div>

              <div className="p-4 bg-bg-secondary border border-border-subtle rounded-md shadow-card">
                <span className="text-xs text-text-secondary">
                  Total Investments
                </span>
                <div className="text-xl font-bold mt-1 text-blue-500">
                  {symbol}
                  {globalInvestment.toFixed(2)}
                </div>
                <span className="text-xs text-text-muted">
                  Long-term position
                </span>
              </div>

              <div className="p-4 bg-bg-secondary border border-border-subtle rounded-md shadow-card">
                <span className="text-xs text-text-secondary">Active Debt</span>
                <div className="text-xl font-bold mt-1 text-orange-500">
                  {symbol}
                  {globalDebt.toFixed(2)}
                </div>
                <span className="text-xs text-text-muted">
                  Expenses: {symbol}
                  {totalExpense.toFixed(2)} this mo
                </span>
              </div>
            </section>
          </ScrollReveal>

          <ScrollReveal>
            <section className="grid grid-cols-1 lg:grid-cols-3 gap-6">
              <div className="p-6 bg-form-bg border border-border-subtle rounded-md shadow-card flex flex-col justify-between">
                <div>
                  <div className="flex items-center justify-between mb-3">
                    <div className="flex items-center gap-2">
                      <div className="p-2 bg-blue-50 rounded-sm">
                        <MdAccountBalanceWallet className="size-3 sm:size-5 text-blue-500" />
                      </div>
                      <h2 className="sm:text-lg font-bold">Monthly Budget</h2>
                    </div>
                    <Link
                      to="/budget"
                      className="text-xs font-semibold text-blue-900 hover:underline"
                    >
                      Manage
                    </Link>
                  </div>

                  {globalBudget ? (
                    <div className="space-y-3">
                      <div className="flex justify-between text-sm">
                        <span className="text-text-secondary">Used</span>
                        <span className="font-bold">
                          {globalBudget.budgetConsumptionPercentage.toFixed(0)}%
                        </span>
                      </div>
                      <div className="w-full bg-bg-muted h-2 rounded-full overflow-hidden">
                        <div
                          className={` h-full rounded-full ${globalBudget.budgetConsumptionPercentage < 50 ? "bg-green-500" : globalBudget.budgetConsumptionPercentage < 70 ? "bg-amber-500" : "bg-red-500"}`}
                          style={{
                            width: `${Math.min(globalBudget.budgetConsumptionPercentage, 100)}%`,
                          }}
                        />
                      </div>
                      <p className="text-xs text-text-secondary">
                        {symbol}
                        {globalBudget.budgetConsumption} of {symbol}
                        {globalBudget.amount.amount.toFixed(2)} global limit
                      </p>
                    </div>
                  ) : (
                    <p className="text-sm text-text-secondary">
                      No monthly budget set yet. Add a global or category limit
                      to stay ahead of overspending.
                    </p>
                  )}
                </div>

                <Link
                  to={globalBudget ? "/budget" : "/budget/new"}
                  className="mt-5 inline-block text-center py-2 px-4 rounded-sm bg-bg-secondary border border-border-subtle hover:bg-bg-muted text-sm font-semibold transition-colors"
                >
                  {globalBudget ? "View Budget Breakdown" : "+ Add a Budget"}
                </Link>
              </div>

              <div className="p-6 bg-form-bg border border-border-subtle rounded-md shadow-card flex flex-col justify-between">
                <div>
                  <div className="flex items-center justify-between mb-3">
                    <div className="flex items-center gap-2">
                      <div className="p-2 bg-green-50 rounded-sm">
                        <GoGoal className="size-3 sm:size-5 text-green-500" />
                      </div>
                      <h2 className="sm:text-lg font-bold">Monthly Goals</h2>
                    </div>
                    <Link
                      to="/goal"
                      className="text-xs font-semibold text-blue-900 hover:underline"
                    >
                      All Goals
                    </Link>
                  </div>

                  {activeGoalsCount > 0 ? (
                    <div className="space-y-3">
                      <div className="flex justify-between text-sm">
                        <span className="text-text-secondary">
                          {activeGoalsCount} active{" "}
                          {activeGoalsCount === 1 ? "goal" : "goals"}
                        </span>
                        <span className="font-bold text-green-900">
                          {avgGoalProgress.toFixed(0)}% avg
                        </span>
                      </div>
                      <ul className="space-y-2">
                        {goals!.slice(0, 2).map((g) => (
                          <li
                            key={g.id}
                            className="flex justify-between items-center text-sm p-2 rounded-sm bg-bg-muted"
                          >
                            <span className="truncate font-medium">
                              {g.title}
                            </span>
                            <span className="text-xs font-semibold ml-2">
                              {g.goalProgress.toFixed(0)}%
                            </span>
                          </li>
                        ))}
                      </ul>
                    </div>
                  ) : (
                    <p className="text-sm text-text-secondary">
                      Set a monthly savings or target objective to build
                      momentum one month at a time.
                    </p>
                  )}
                </div>

                <Link
                  to={activeGoalsCount > 0 ? "/goal" : "/goal/new"}
                  className="mt-5 inline-block text-center py-2 px-4 rounded-sm bg-bg-secondary border border-border-subtle hover:bg-bg-muted text-sm font-semibold transition-colors"
                >
                  {activeGoalsCount > 0 ? "Track Goals" : "+ Add a Goal"}
                </Link>
              </div>

              {/* Recent Transactions Card */}
              <div className="p-6 bg-form-bg border border-border-subtle rounded-md shadow-card flex flex-col justify-between">
                <div>
                  <div className="flex items-center justify-between mb-3">
                    <div className="flex items-center gap-2">
                      <div className="p-2 bg-yellow-50 rounded-sm">
                        <AiOutlineTransaction className="size-3 sm:size-5 text-yellow-500" />
                      </div>
                      <h2 className="sm:text-lg font-bold">
                        Recent Transactions
                      </h2>
                    </div>
                    <Link
                      to="/financialTransactions"
                      className="text-xs font-semibold text-blue-900 hover:underline"
                    >
                      See All
                    </Link>
                  </div>

                  {recentTransactions.length > 0 ? (
                    <ul className="space-y-2">
                      {recentTransactions.map((t) => (
                        <li
                          key={t.id}
                          className="flex justify-between items-center text-sm p-2 rounded-sm bg-bg-muted"
                        >
                          <span className="truncate">
                            {t.description || t.categoryName || "Transaction"}
                          </span>
                          <span className="font-semibold ml-2 whitespace-nowrap">
                            {t.amount?.currency?.symbol ?? symbol}
                            {Number(t.amount?.amount ?? 0).toFixed(2)}
                          </span>
                        </li>
                      ))}
                    </ul>
                  ) : (
                    <p className="text-sm text-text-secondary">
                      No transactions logged for this month yet. Start recording
                      income, expenses, savings, and investments.
                    </p>
                  )}
                </div>

                <Link
                  to="/financialTransactions"
                  className="mt-5 inline-block text-center py-2 px-4 rounded-sm bg-bg-secondary border border-border-subtle hover:bg-bg-muted text-sm font-semibold transition-colors"
                >
                  Open Transactions Ledger
                </Link>
              </div>
            </section>
          </ScrollReveal>
        </div>
      )}
    </>
  );
}

export default LandingPage;
