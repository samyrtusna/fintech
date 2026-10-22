import type {
  AggregateVariationsByMonthType,
  FormattedChartData,
} from "../types/financialAggregatesTypes";

const FormatData = (
  data: AggregateVariationsByMonthType[],
): FormattedChartData | null => {
  if (data.length === 0) return null;

  const newData = data.map((d) => {
    return {
      ...d,
      period: new Date(d.period),
    };
  });
  const sortedData = [...newData].sort(
    (a, b) => a.period.getTime() - b.period.getTime(),
  );

  const firstDate = new Date(sortedData[0].period);
  const lastDate = new Date(sortedData[sortedData.length - 1].period);

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
            d.period.getFullYear() === year &&
            d.period.getMonth() - 1 === month,
        )?.amount ?? 0,
    });
    if (current.getUTCMonth() === 11) {
      current.setUTCFullYear(current.getUTCFullYear() + 1);
      current.setUTCMonth(0);
    }
    current.setUTCMonth(current.getUTCMonth() + 1);
  }
  return {
    key: sortedData[0].categoryName ?? sortedData[0].type,
    value: result,
  };
};

export default FormatData;
