import {
  Line,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  Legend,
  createHorizontalChart,
} from "recharts";

export type LineConfig<TData> = {
  dataKey: keyof TData;
  name: string;
  stroke: string;
};

type ChartProps<TData> = {
  data: TData[];
  lines: LineConfig<TData>[];
  xAxisDataKey: keyof TData;
};

export default function LinesChart<TData>({
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
      {/* eslint-disable-next-line @typescript-eslint/no-explicit-any */}
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
          key={String(line.dataKey)}
          type="monotone"
          /* eslint-disable-next-line @typescript-eslint/no-explicit-any */
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
