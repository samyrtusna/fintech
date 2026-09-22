import { useEffect, useState } from "react";
import generateMonths from "../helpers/generateMonths";
import { Link, useNavigate } from "react-router-dom";
import { ChevronLeft, ChevronRight, Plus } from "lucide-react";
import { toast } from "sonner";
import {
  type MonthItem,
  type TransactionsFilter,
} from "../types/financialTransactionTypes";
import { useAppDispatch, useAppSelector } from "../state/stateHooks";
import useClickOutside from "../hooks/useClickOutside";
import { DecodeToken } from "../helpers/tokenDecoder";
import FormatAmount from "../helpers/amountFormatter";
import Calendar from "../components/Calendar";
import financialTrascationService from "../API/Services/financialTrascationService";
import { setFinancialTransactions } from "../state/slices/financialTransactionSlice";
import { format } from "date-fns";

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
  const [page, setPage] = useState(1);

  const navigate = useNavigate();
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

  const totalPages =
    transactionsState.transactions == null
      ? 0
      : Math.ceil(
          transactionsState.transactions.totalCount /
            transactionsState.transactions.pageSize,
        );

  useEffect(() => {
    const filter: TransactionsFilter = {
      year:
        period === "Month" ? selectedMonth!.year : selectedDay!.getFullYear(),

      month:
        period === "Month" ? selectedMonth!.month : selectedDay!.getMonth() + 1,

      day: period === "Day" ? selectedDay!.getDate() : undefined,

      page,
      pageSize,
    };

    const fetchTransactions = async () => {
      try {
        const fetchedTransactions =
          await financialTrascationService.getByFilterAsync(filter);

        dispatch(setFinancialTransactions(fetchedTransactions));
      } catch (error) {
        toast.error(
          error instanceof Error
            ? error.message
            : "Failed to fetch transactions",
        );
      }
    };

    fetchTransactions();
  }, [period, selectedMonth, selectedDay, page, dispatch]);

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
    <div className="relative ">
      <div className="relative min-h-dvh w-full">
        {/* <div className="hidden md:block absolute w-11/12 bg-bg-secondary -top-14 bottom-2 left-1/2 -translate-x-1/2 -z-10"></div> */}
        <div className="flex w-full justify-center mt-5">
          <div
            ref={periodRef}
            className="relative"
          >
            <button
              onClick={() => setPeriodDropdown(!periodDropdown)}
              className={`py-1 px-2 w-20 bg-navbar-bg hover:bg-btn-standard cursor-pointer ${periodDropdown ? "rounded-tl-sm" : "rounded-l-sm"}`}
            >
              {period}
            </button>
            <div className={!periodDropdown ? "hidden" : "absolute top-8 z-20"}>
              <button
                onClick={togglePeriod}
                className="w-20 py-1 px-2  bg-navbar-bg hover:bg-btn-standard rounded-b-sm cursor-pointer"
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
                  onClick={() => {
                    setDateDropdown((prev) => !prev);
                  }}
                  className={`w-50 py-1 px-2 bg-navbar-bg hover:bg-btn-standard cursor-pointer ${dateDropdown ? "rounded-tr-sm" : "rounded-r-sm"}`}
                >
                  {selectedMonth?.label}
                </button>
                {dateDropdown && (
                  <div className="absolute top-8 w-50 flex flex-col z-50">
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
                          className={`py-1 px-2 bg-navbar-bg hover:bg-btn-standard cursor-pointer
                  ${index === arr.length - 1 ? "rounded-b-sm" : ""}`}
                        >
                          {month.label}
                        </button>
                      ))}
                  </div>
                )}
              </div>
            ) : (
              <div>
                <button
                  onClick={() => setDateDropdown(!dateDropdown)}
                  className={`w-30 py-1 px-2 bg-btn-standard hover:bg-btn-standard-hover cursor-pointer ${dateDropdown ? "rounded-tr-sm" : "rounded-r-sm"}`}
                >
                  {format(selectedDay!, "dd-MM-yyyy")}
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
          <div className="relative w-full md:w-fit flex justify-center ">
            <table className="border-collapse w-full md:w-fit table-fixed shadow-card">
              <thead>
                <tr className="hidden md:table-row bg-table-head-bg">
                  {tableHead.map((head) => (
                    <th
                      key={head}
                      className={`${head === "Category" ? "w-50" : "w-30"} py-4`}
                    >
                      {head}
                    </th>
                  ))}
                </tr>
                <tr className="md:hidden bg-table-head-bg">
                  <th className="py-4">Type</th>
                  <th>Category</th>
                  <th>Amount</th>
                </tr>
              </thead>
              <tbody className="bg-table-row-bg">
                {transactionsState.transactions?.items.map((transaction) => (
                  <tr
                    key={transaction.id}
                    onClick={() =>
                      navigate(`financialTransaction/${transaction.id}`)
                    }
                    className={
                      "border-b border-b-border-subtle  cursor-pointer hover:bg-table-row-alt-bg"
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
            <div className="absolute -top-9 md:-top-12 right-0 md:-right-12 h-8 md:h-12 aspect-square group bg-navbar-bg hover:bg-btn-standard">
              {/* TODO Add Button */}
              <Link to="newFinancialTransaction">
                <Plus className="stroke-1 stroke-text-primary w-full h-full rounded-sm shadow-card hover:stroke-gray-700" />
              </Link>
              <span className="absolute opacity-0 md:group-hover:opacity-100 -top-5 -right-40 px-2 py-1 bg-navbar-bg rounded-sm">
                Add new Transaction
              </span>
            </div>
          </div>
        </div>
        <div className="flex justify-center gap-2 py-6">
          <button
            disabled={page === 1}
            onClick={() => setPage((prev) => prev - 1)}
            className="px-2 py-1 mr-1 rounded-md disabled:opacity-0 cursor-pointer bg-navbar-bg hover:bg-btn-standard "
          >
            <ChevronLeft className="stroke-text-primary" />
          </button>

          <span className="px-2 pt-1 rounded-md bg-navbar-bg">
            Page {page} of {totalPages}
          </span>

          <button
            disabled={page === totalPages}
            onClick={() => setPage((prev) => prev + 1)}
            className="px-2 py-1 ml-1 rounded-md disabled:opacity-0 cursor-pointer bg-navbar-bg hover:bg-btn-standard "
          >
            <ChevronRight className="stroke-text-primary" />
          </button>
        </div>
      </div>
    </div>
  );
}

export default FinancialTransactions;
