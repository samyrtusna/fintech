import { Link } from "react-router-dom";
import { AiOutlineTransaction } from "react-icons/ai";
import { IoMdAdd } from "react-icons/io";
import { MdAccountBalanceWallet, MdPieChart } from "react-icons/md";

function LandingPage() {
  return (
    <div className="relative min-h-dvh w-full bg-bg-primary text-text-primary flex flex-col items-center">
      {/* Hero Section */}
      <section className="w-full max-w-5xl px-6 py-20 flex flex-col items-center text-center mt-10">
        <div className="inline-flex items-center gap-2 px-3 py-1 mb-6 rounded-full bg-bg-muted border border-border-subtle text-xs font-semibold">
          <span className="h-2 w-2 rounded-full bg-green-500 animate-pulse"></span>
          Your Personal Finances Dashboard
        </div>

        <h1 className="text-4xl md:text-6xl font-extrabold tracking-tight max-w-3xl leading-tight">
          A new month,{" "}
          <span className="text-blue-500">a clearer financial picture.</span>
        </h1>

        <p className="mt-6 text-lg md:text-xl text-text-secondary max-w-2xl">
          Take control of your cash flow, monitor savings and investments, and
          keep your debt under control as each month unfolds.
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

      {/* Feature Highlights Grid */}
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
            Instantly track net cash flow, active debts, savings, and total
            investment metrics across custom periods.
          </p>
        </div>

        {/* Card 2 */}
        <div className="p-6 bg-form-bg border border-border-subtle rounded-md shadow-card flex flex-col items-start">
          <div className="p-3 bg-green-500/10 rounded-sm mb-4">
            <AiOutlineTransaction className="size-6 text-green-500" />
          </div>
          <h3 className="text-xl font-semibold mb-2">Smart Transactions</h3>
          <p className="text-sm text-text-secondary">
            Log, categorize, and filter income, expenses, and loan repayments
            with essential vs. non-essential tagging.
          </p>
        </div>

        {/* Card 3 */}
        <div className="p-6 bg-form-bg border border-border-subtle rounded-md shadow-card flex flex-col items-start">
          <div className="p-3 bg-yellow-500/10 rounded-sm mb-4">
            <MdPieChart className="size-6 text-yellow-500" />
          </div>
          <h3 className="text-xl font-semibold mb-2">Visual Breakdowns</h3>
          <p className="text-sm text-text-secondary">
            Understand your spending distribution dynamically using built-in
            interactive charts and ratio indicators.
          </p>
        </div>
      </section>

      {/* Callout Banner Section */}
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
    </div>
  );
}

export default LandingPage;
