import {
  Line,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  Legend,
  createHorizontalChart,
} from "recharts";

export type LineConfig = {
  dataKey: string;
  name: string;
  stroke: string;
};

type ChartProps<TData> = {
  data: TData[];
  lines: LineConfig[];
  xAxisDataKey: string;
};

export default function LinesChart<TData extends Record<string, any>>({
  data,
  lines,
  xAxisDataKey,
}: ChartProps<TData>) {
  const Typed = createHorizontalChart<TData, string, number>()({
    XAxis,
    YAxis,
    Tooltip,
    Line,
  });

  return (
    <Typed.LineChart
      style={{
        width: "100%",
        maxWidth: "100%",
        height: "100%",
        maxHeight: "70vh",
        aspectRatio: 1.618,
      }}
      responsive
      data={data}
      margin={{ top: 10, right: 20, left: 20, bottom: 10 }}
    >
      <CartesianGrid strokeDasharray="3 3" />
      <Typed.XAxis dataKey={xAxisDataKey as any} />
      <Typed.YAxis
        width="auto"
        tickFormatter={(value: number) => `$${value.toLocaleString("en-US")}`}
      />
      <Tooltip
        formatter={(value) => [
          `$${Number(value).toLocaleString("en-US", {
            minimumFractionDigits: 2,
            maximumFractionDigits: 2,
          })}`,
        ]}
      />
      <Legend />
      {lines.map((line) => (
        <Typed.Line
          key={line.dataKey}
          type="monotone"
          dataKey={line.dataKey as any}
          name={line.name}
          stroke={line.stroke}
          strokeWidth={2}
          dot={{ r: 4 }}
        />
      ))}
    </Typed.LineChart>
  );
}
