import { useEffect, useState } from "react";
import generateMonths from "../helpers/generateMonths";
import { Link } from "react-router-dom";
import { ChevronLeft, ChevronRight, Plus } from "lucide-react";
import { toast } from "sonner";
import {
  type GetTransactionResponse,
  type MonthItem,
} from "../types/financialTransactionTypes";
import { useAppDispatch, useAppSelector } from "../state/stateHooks";
import { fetchTransactions } from "../state/slices/financialTransactionSlice";
import Spinner from "../components/Spinner";
import useClickOutside from "../hooks/useClickOutside";
import FinancialTransaction from "../components/FinancialTransaction";
import { DecodeToken } from "../helpers/tokenDecoder";

type PeriodType = "Month" | "Day";

function FinancialTransactions() {
  const months = generateMonths();

  const [selectedMonth, setSelectedMonth] = useState<MonthItem>(months[0]);
  const [monthDropdown, setMonthDropdown] = useState<boolean>(false);
  const [period, setPeriod] = useState<PeriodType>("Month");
  const [periodDropdown, setPeriodDropdown] = useState<boolean>(false);
  const [activeTransaction, setActiveTransaction] =
    useState<GetTransactionResponse | null>(null);
  const [page, setPage] = useState(1);
  const pageSize = 10;

  const periodRef = useClickOutside<HTMLDivElement>(() =>
    setPeriodDropdown(false),
  );
  const monthRef = useClickOutside<HTMLDivElement>(() =>
    setMonthDropdown(false),
  );

  const token = useAppSelector((state) => state.authUser.accessToken);
  const transactionsState = useAppSelector((state) => state.transactions);
  const dispatch = useAppDispatch();

  const baseCurrency = DecodeToken(token!).baseCurrency;

  const filter = {
    year: selectedMonth.year,
    month: selectedMonth.month,
    page,
    pageSize,
  };

  const totalPages =
    transactionsState.transactions == null
      ? 0
      : Math.ceil(
          transactionsState.transactions.totalCount /
            transactionsState.transactions.pageSize,
        );

  useEffect(() => {
    dispatch(fetchTransactions(filter));
  }, [dispatch, selectedMonth, page]);

  const tableHead = ["Type", "Category", "Amount", "Currency", "IsEssential"];

  const togglePeriod = () => {
    setPeriod((prev) => (prev === "Month" ? "Day" : "Month"));
    setPeriodDropdown(!periodDropdown);
  };

  if (transactionsState.isLoadingTransactions) {
    return (
      <div className="flex h-dvh w-dvw justify-center items-center">
        <Spinner />
      </div>
    );
  }

  if (transactionsState.error) {
    toast.error(transactionsState.error);
  }

  return (
    <div className="relative">
      <div
        className={`relative min-h-dvh w-full ${activeTransaction && "blur-3xl"}`}
      >
        <div className="hidden md:block absolute w-11/12 bg-bg-sec -top-14 bottom-2 left-1/2 -translate-x-1/2 rounded-t-2xl -z-10"></div>
        <div className="flex w-full justify-center mt-5 bg-transparent">
          <div
            ref={periodRef}
            className="relative"
          >
            <button
              onClick={() => setPeriodDropdown(!periodDropdown)}
              className={`py-1 px-2 w-20 border border-gray-200 cursor-pointer ${periodDropdown ? "rounded-tl-md" : "rounded-l-md"}`}
            >
              {period}
            </button>
            <div className={!periodDropdown ? "hidden" : "absolute top-8 z-20"}>
              <button
                onClick={togglePeriod}
                className="w-20 py-1 px-2  border border-gray-200 rounded-b-md cursor-pointer"
              >
                {period === ("Month" as PeriodType) ? "Day" : "Month"}
              </button>
            </div>
          </div>
          <div
            ref={monthRef}
            className="relative"
          >
            <button
              onClick={() => setMonthDropdown(!monthDropdown)}
              className={`w-30 py-1 px-2 border border-gray-200 cursor-pointer bg-bg ${monthDropdown ? "rounded-tr-md" : "rounded-r-md"}`}
            >
              {selectedMonth.label}
            </button>
            <div
              className={
                !monthDropdown ? "hidden" : "absolute top-8 flex flex-col z-20"
              }
            >
              {months
                .filter((month) => month.key !== selectedMonth.key)
                .map((month, index, arr) => (
                  <button
                    key={month.key}
                    onClick={() => {
                      setSelectedMonth(month);
                      setPage(1);
                      setMonthDropdown(!monthDropdown);
                    }}
                    className={`w-30 py-1 px-2 border border-gray-200 cursor-pointer bg-bg
                  ${index === arr.length - 1 ? "rounded-b-md" : ""}`}
                  >
                    {month.label}
                  </button>
                ))}
            </div>
          </div>
        </div>
        <div className="mt-10 w-full flex justify-center">
          <div className="relative w-full md:w-fit flex justify-center">
            <table className="border-collapse w-full md:w-fit table-fixed shadow-2xl">
              <thead>
                <tr className="hidden md:table-row bg-gray-300">
                  {tableHead.map((head) => (
                    <th
                      key={head}
                      className="md:w-30 py-4"
                    >
                      {head}
                    </th>
                  ))}
                </tr>
                <tr className="md:hidden bg-gray-300">
                  <th>Type</th>
                  <th>Category</th>
                  <th>Amount</th>
                </tr>
              </thead>
              <tbody>
                {transactionsState.transactions?.items.map(
                  (transaction, index, arr) => (
                    <tr
                      key={transaction.id}
                      onClick={() => setActiveTransaction(transaction)}
                      className={`border-t border-t-gray-200 bg-bg cursor-pointer ${index === arr.length - 1 ? "border-b border-b-gray-200" : ""}`}
                    >
                      <td className="md:w-30 flex justify-start py-3 pl-4">
                        {transaction.type}
                      </td>
                      <td className="md:w-30 py-3 pl-4">
                        {transaction.categoryName}
                      </td>
                      <td className="md:w-30 flex justify-end py-3 px-4">
                        {transaction.baseAmount}
                      </td>
                      <td className="hidden md:table-cell w-30 py-3 pl-10">
                        {transaction.currency}
                      </td>
                      <td className="hidden md:table-cell w-30 py-3 pl-10">
                        {transaction.isEssential ? "True" : "False"}
                      </td>
                    </tr>
                  ),
                )}
              </tbody>
            </table>
            <div className="absolute -top-9 md:-top-12 right-0 md:-right-12 h-8 md:h-12 aspect-square group">
              <Link to="newFinancialTransaction">
                <Plus className="stroke-1 stroke-gray-500 w-full h-full rounded-md shadow-lg hover:stroke-gray-700" />
              </Link>
              <span className="absolute opacity-0 group-hover:opacity-100 -top-5 -right-40 px-2 py-1 bg-gray-300 border border-gray-200 rounded-lg">
                Add new Transaction
              </span>
            </div>
          </div>
        </div>
        <div className="flex justify-center gap-2 py-6">
          <button
            disabled={page === 1}
            onClick={() => setPage((prev) => prev - 1)}
            className="px-3 py-1 disabled:opacity-0 cursor-pointer"
          >
            <ChevronLeft className="stroke-gray-500" />
          </button>

          <span>
            Page {page} of {totalPages}
          </span>

          <button
            disabled={page === totalPages}
            onClick={() => setPage((prev) => prev + 1)}
            className="px-3 py-1 disabled:opacity-0 cursor-pointer"
          >
            <ChevronRight className="stroke-gray-500" />
          </button>
        </div>
      </div>
      {activeTransaction && (
        <FinancialTransaction
          transaction={activeTransaction}
          baseCurrency={baseCurrency}
          handleBack={() => setActiveTransaction(null)}
        />
      )}
    </div>
  );
}

export default FinancialTransactions;
