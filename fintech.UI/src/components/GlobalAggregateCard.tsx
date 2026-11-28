import FormatAmount from "../helpers/amountFormatter";

interface PropsType {
  aggregateName: string;
  aggregateValue: number | undefined;
  condition: boolean;
  symbol: string;
}

function GlobalAggregateCard(props: PropsType) {
  const { aggregateName, symbol, aggregateValue, condition } = props;
  return (
    <div className="p-4 w-full bg-bg-surface border-thin border-border-subtle rounded-md shadow-card animate-slow-drift">
      <h3 className="text-xs font-semibold uppercase tracking-wider text-text-muted md:mr-4">
        {aggregateName}
      </h3>
      <div className="flex items-baseline mt-1.5">
        <h3
          className={`text-lg font-semibold ${condition ? "text-red-500" : "text-emerald-600 dark:text-emerald-400"}`}
        >
          {symbol}
        </h3>
        <h2
          className={`text-xl font-bold ml-1 ${condition ? "text-red-500" : "text-emerald-600 dark:text-emerald-400"}`}
        >
          {FormatAmount(aggregateValue ?? 0)}
        </h2>
      </div>
    </div>
  );
}

export default GlobalAggregateCard;
