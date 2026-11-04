import type {
  AggregateVariationsByMonthType,
  FormattedChartData,
} from "../types/financialAggregatesTypes";

const FormatData = (
  data: AggregateVariationsByMonthType,
): FormattedChartData | null => {
  if (data.values.length === 0) return null;

  const newData = data.values.map((d) => {
    return {
      ...d,
      period: {
        startDate: new Date(d.period.startDate),
        endDate: new Date(d.period.endDate),
      },
    };
  });
  const sortedData = [...newData].sort(
    (a, b) => a.period.startDate.getTime() - b.period.startDate.getTime(),
  );

  const firstDate = new Date(sortedData[0].period.startDate);
  const lastDate = new Date(sortedData[sortedData.length - 1].period.startDate);

  const current = new Date(
    Date.UTC(firstDate.getUTCFullYear(), firstDate.getUTCMonth(), 1),
  );

  const end = new Date(
    Date.UTC(lastDate.getUTCFullYear(), lastDate.getUTCMonth(), 1),
  );

  const result = [];

  while (current <= end) {
    const year = current.getUTCFullYear();
    const month = current.getUTCMonth();

    result.push({
      month: new Date(current),
      amount:
        sortedData.find(
          (d) =>
            d.period.startDate.getFullYear() === year &&
            d.period.startDate.getMonth() === month,
        )?.amount.amount ?? 0,
    });
    if (current.getUTCMonth() === 11) {
      current.setUTCFullYear(current.getUTCFullYear() + 1);
      current.setUTCMonth(0);
    }
    current.setUTCMonth(current.getUTCMonth() + 1);
  }

  return {
    key: data.name,
    value: result,
  };
};

export default FormatData;
