import { MdTrendingDown, MdTrendingUp } from "react-icons/md";
import type { FinancialTotalsType } from "../types/financialAggregatesTypes";
import PieCharts from "./PieChart";

interface PropsType {
  dataList: FinancialTotalsType[];
  symbol: string;
  total: number;
  variation: number;
  aggregate: string;
}

function PieChatCard(props: PropsType) {
  const { dataList, symbol, total, variation, aggregate } = props;
  const colors = [
    "#f3081c",
    "#10f308",
    "#f3ef08",
    "#8108f3",
    "#1447e6",
    "#a84832",
    "#a89532",
    "#a85e32",
    "#6da832",
    "#32a887",
    "#328ba8",
    "#a232a8",
    "#a83268",
    "#a85e32",
    "#a89e32",
  ];

  const pieChartData = (dataList ?? []).map((category, index) => ({
    name: category.name,
    value: category.amount,
    color: colors[index % colors.length],
  }));
  return (
    <div className="w-full space-y-5 my-auto">
      <div className="flex flex-col md:flex-row md:items-baseline w-full items-center md:gap-2 justify-center">
        <span className="text-xs font-semibold text-text-secondary uppercase tracking-wider">
          Total {aggregate}
        </span>
        <div className="flex gap-4 ">
          <div className="text-3xl font-extrabold">
            {symbol}
            {total?.toFixed(2)}
          </div>
          {variation !== null && (
            <div
              className={`mt-4 flex items-center gap-1 text-xs font-medium ${variation! >= 0 ? "text-green-500" : "text-red-500"}`}
            >
              {variation! >= 0 ? (
                <MdTrendingUp className="size-4" />
              ) : (
                <MdTrendingDown className="size-4" />
              )}
              <span>
                {variation! > 0 ? `+${variation}%` : `-${variation}%`}{" "}
              </span>
            </div>
          )}
        </div>
      </div>
      <div className="flex flex-col md:flex-row justify-between items-center w-full ">
        <div className="max-w-70 w-full md:w-1/2 md:px-5">
          <PieCharts data={pieChartData} />
        </div>

        <div className="w-full px-6 md:px-0 md:w-1/2 md:ml-5 text-xs space-y-1">
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
              <span className="text-text-muted">
                {symbol}
                {pie.value.toFixed(2)}
              </span>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}

export default PieChatCard;
