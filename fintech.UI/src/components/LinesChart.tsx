/* eslint-disable @typescript-eslint/no-explicit-any */
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
      <CartesianGrid
        strokeDasharray="3 3"
        stroke="var(--border-subtle)"
        opacity={0.6}
      />
      {/* eslint-disable-next-line @typescript-eslint/no-explicit-any */}
      <Typed.XAxis
        dataKey={xAxisDataKey as any}
        stroke="var(--text-muted)"
        tick={{ fill: "var(--text-muted)", fontSize: 12 }}
      />
      <Typed.YAxis
        width="auto"
        stroke="var(--text-muted)"
        tick={{ fill: "var(--text-muted)", fontSize: 12 }}
        tickFormatter={(value: number) => `$${value.toLocaleString("en-US")}`}
      />
      <Tooltip
        contentStyle={{
          backgroundColor: "var(--bg-surface)",
          borderColor: "var(--border-subtle)",
          borderRadius: "0.375rem",
          color: "var(--text-primary)",
          boxShadow: "var(--shadow-card)",
        }}
        itemStyle={{ color: "var(--text-primary)" }}
        labelStyle={{ color: "var(--text-secondary)", fontWeight: 600 }}
        formatter={(value) => [
          `$${Number(value).toLocaleString("en-US", {
            minimumFractionDigits: 2,
            maximumFractionDigits: 2,
          })}`,
        ]}
      />
      <Legend
        wrapperStyle={{ color: "var(--text-secondary)", fontSize: "0.75rem" }}
      />
      {lines.map((line) => (
        <Typed.Line
          key={String(line.dataKey)}
          type="monotone"
          /* eslint-disable-next-line @typescript-eslint/no-explicit-any */
          dataKey={line.dataKey as any}
          name={line.name}
          stroke={line.stroke}
          strokeWidth={2.5}
          dot={{ r: 4, fill: line.stroke }}
          activeDot={{ r: 6 }}
        />
      ))}
    </Typed.LineChart>
  );
}
