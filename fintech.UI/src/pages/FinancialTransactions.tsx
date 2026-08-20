import { useEffect, useState } from "react";
import generateMonths from "../helpers/generateMonths";
import { Link } from "react-router-dom";
import { ChevronLeft, ChevronRight, Plus } from "lucide-react";
import { toast } from "sonner";
import {
  type GetTransactionResponse,
  type MonthItem,
  type TransactionsFilter,
} from "../types/financialTransactionTypes";
import { useAppDispatch, useAppSelector } from "../state/stateHooks";
import useClickOutside from "../hooks/useClickOutside";
import FinancialTransaction from "../components/FinancialTransaction";
import { DecodeToken } from "../helpers/tokenDecoder";
import FormatAmount from "../helpers/amountFormatter";
import Calendar from "../components/Calendar";
import financialTrascationService from "../API/Services/financialTrascationService";
import { setFinancialTransactions } from "../state/slices/financialTransactionSlice";

type PeriodType = "Month" | "Day";

function FinancialTransactions() {
  const months = generateMonths();

  const [selectedMonth, setSelectedMonth] = useState<MonthItem | null>(
    months[0],
  );
  const [selectedDay, setSelectedDay] = useState<Date | null>(null);
  const [dateDropdown, setDateDropdown] = useState<boolean>(false);
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
    setDateDropdown(false),
  );

  const token = useAppSelector((state) => state.authUser.accessToken);
  const transactionsState = useAppSelector((state) => state.transactions);
  const dispatch = useAppDispatch();

  const baseCurrency = DecodeToken(token!).baseCurrency;

  const filter: TransactionsFilter = {
    year: selectedMonth ? selectedMonth.year : selectedDay!.getFullYear(),
    month: selectedMonth ? selectedMonth.month : selectedDay!.getMonth() + 1,
    day: selectedDay?.getDate(),
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

  const fetchTransactions = async () => {
    try {
      const fetchedTransactions =
        await financialTrascationService.getByFilterAsync(filter);
      dispatch(setFinancialTransactions(fetchedTransactions));
    } catch (error) {
      toast.error(
        error instanceof Error ? error.message : "Failed to fetch transactions",
      );
    }
  };

  useEffect(() => {
    fetchTransactions();
  }, [selectedMonth, selectedDay, period, page]);

  const tableHead = ["Type", "Category", "Amount", "Currency", "Essential"];

  const togglePeriod = () => {
    const newPeriod = period === "Month" ? "Day" : "Month";

    setPeriod(newPeriod);

    if (newPeriod === "Month") {
      setSelectedMonth(months[0]);
      setSelectedDay(null);
    } else {
      setSelectedDay(new Date());
      setSelectedMonth(null);
    }
    setPage(1);
    setPeriodDropdown(false);
  };

  return (
    <div className="relative">
      <div
        className={`relative min-h-dvh w-full ${activeTransaction && "blur-3xl"}`}
      >
        <div className="hidden md:block absolute w-11/12 bg-bg-surface -top-14 bottom-2 left-1/2 -translate-x-1/2 -z-10"></div>
        <div className="flex w-full justify-center mt-5">
          <div
            ref={periodRef}
            className="relative"
          >
            <button
              onClick={() => setPeriodDropdown(!periodDropdown)}
              className={`py-1 px-2 w-20 bg-bg-secondary hover:bg-bg-muted cursor-pointer ${periodDropdown ? "rounded-tl-sm" : "rounded-l-sm"}`}
            >
              {period}
            </button>
            <div className={!periodDropdown ? "hidden" : "absolute top-8 z-20"}>
              <button
                onClick={togglePeriod}
                className="w-20 py-1 px-2  bg-bg-secondary hover:bg-bg-muted rounded-b-sm cursor-pointer"
              >
                {period === ("Month" as PeriodType) ? "Day" : "Month"}
              </button>
            </div>
          </div>
          <div
            ref={monthRef}
            className="relative"
          >
            {period === "Month" ? (
              <div>
                <button
                  onClick={() => setDateDropdown(!dateDropdown)}
                  className={`w-30 py-1 px-2 bg-bg-secondary hover:bg-bg-muted cursor-pointer ${dateDropdown ? "rounded-tr-sm" : "rounded-r-sm"}`}
                >
                  {selectedMonth?.label}
                </button>
                <div
                  className={
                    !dateDropdown
                      ? "hidden"
                      : "absolute top-8 flex flex-col z-20"
                  }
                >
                  {months
                    .filter((month) => month.key !== selectedMonth?.key)
                    .map((month, index, arr) => (
                      <button
                        key={month.key}
                        onClick={() => {
                          setSelectedMonth(month);
                          setSelectedDay(null);
                          setPage(1);
                          setDateDropdown(!dateDropdown);
                        }}
                        className={`w-30 py-1 px-2 bg-bg-secondary hover:bg-bg-muted cursor-pointer
                  ${index === arr.length - 1 ? "rounded-b-md" : ""}`}
                      >
                        {month.label}
                      </button>
                    ))}
                </div>
              </div>
            ) : (
              <div>
                <button
                  onClick={() => setDateDropdown(!dateDropdown)}
                  className={`w-30 py-1 px-2 bg-bg-secondary hover:bg-bg-muted cursor-pointer ${dateDropdown ? "rounded-tr-sm" : "rounded-r-sm"}`}
                >
                  {selectedDay?.toLocaleDateString("en-GB")}
                </button>
                <div
                  className={
                    !dateDropdown
                      ? "hidden"
                      : "absolute top-8 flex flex-col z-20"
                  }
                >
                  <Calendar
                    selectedDate={selectedDay}
                    handledate={(date) => {
                      setSelectedDay(date);
                      setSelectedMonth(null);
                      setPage(1);
                      setDateDropdown(false);
                    }}
                  />
                </div>
              </div>
            )}
          </div>
        </div>
        <div className="mt-10 w-full flex justify-center">
          <div className="relative w-full md:w-fit flex justify-center bg-bg-secondary">
            <table className="border-collapse w-full md:w-fit table-fixed shadow-card">
              <thead>
                <tr className="hidden md:table-row bg-bg-muted">
                  {tableHead.map((head) => (
                    <th
                      key={head}
                      className={`${head === "Category" ? "w-50" : "w-30"} py-4`}
                    >
                      {head}
                    </th>
                  ))}
                </tr>
                <tr className="md:hidden bg-bg-muted">
                  <th className="py-4">Type</th>
                  <th>Category</th>
                  <th>Amount</th>
                </tr>
              </thead>
              <tbody>
                {transactionsState.transactions?.items.map((transaction) => (
                  <tr
                    key={transaction.id}
                    onClick={() => setActiveTransaction(transaction)}
                    className={
                      "border-b border-b-border-subtle  cursor-pointer hover:bg-bg-muted"
                    }
                  >
                    <td className="flex justify-start py-3 pl-4">
                      {transaction.type}
                    </td>
                    <td className="md:py-3 pl-4">{transaction.categoryName}</td>
                    <td className="flex justify-end py-3 pr-6">
                      {FormatAmount(transaction.baseAmount)}
                    </td>
                    <td className="hidden md:table-cell w-30 py-3 pl-10">
                      {baseCurrency}
                    </td>
                    <td className="hidden md:table-cell w-30 py-3 pl-10">
                      {transaction.isEssential ? "True" : "False"}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
            <div className="absolute -top-9 md:-top-12 right-0 md:-right-12 h-8 md:h-12 aspect-square group bg-bg-secondary hover:bg-bg-muted">
              <Link to="newFinancialTransaction">
                <Plus className="stroke-1 stroke-text-primary w-full h-full rounded-sm shadow-card hover:stroke-gray-700" />
              </Link>
              <span className="absolute opacity-0 group-hover:opacity-100 -top-5 -right-40 px-2 py-1 bg-bg-secondary rounded-sm">
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
            <ChevronLeft className="stroke-text-secondary" />
          </button>

          <span>
            Page {page} of {totalPages}
          </span>

          <button
            disabled={page === totalPages}
            onClick={() => setPage((prev) => prev + 1)}
            className="px-3 py-1 disabled:opacity-0 cursor-pointer"
          >
            <ChevronRight className="stroke-text-secondary" />
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
