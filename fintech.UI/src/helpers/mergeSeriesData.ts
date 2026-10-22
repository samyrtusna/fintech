// mergeSeriesData.ts
import type { FormattedChartData } from "../types/financialAggregatesTypes";

export type MergedDataPoint = {
  label: string;
  [key: string]: string | number;
};

export function mergeSeriesData(
  seriesList: (FormattedChartData | null | undefined)[],
): MergedDataPoint[] {
  const periodMap = new Map<string, MergedDataPoint>();

  seriesList.forEach((series) => {
    if (!series) return;

    const { key, value } = series;
    value.forEach((item) => {
      const dateKey = new Date(
        Date.UTC(item.month.getUTCFullYear(), item.month.getUTCMonth(), 1),
      ).toDateString();

      const monthLabel = new Date(
        Date.UTC(item.month.getUTCFullYear(), item.month.getUTCMonth() + 1, 1),
      );

      if (!periodMap.has(dateKey)) {
        periodMap.set(dateKey, {
          label: monthLabel.toLocaleDateString("en-US", {
            month: "short",
            year: "numeric",
            timeZone: "UTC",
          }),
        });
      }

      const existingEntry = periodMap.get(dateKey)!;
      existingEntry[key] = item.amount;
    });
  });

  return Array.from(periodMap.values()).sort((a, b) => {
    return (
      new Date(a.label as string).getTime() -
      new Date(b.label as string).getTime()
    );
  });
}
