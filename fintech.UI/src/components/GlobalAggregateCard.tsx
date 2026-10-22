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
    <div className=" p-2 w-full  bg-bg-contrast border-thin border-border-subtle rounded-md shadow-card animate-slow-drift">
      <h3 className="md:mr-4">{aggregateName} </h3>
      <div className="flex items-baseline">
        <h3
          className={`text-lg ${condition ? "text-red-500" : "text-green-500"}`}
        >
          {symbol}
        </h3>
        <h2
          className={`text-xl font-bold ml-1 ${condition ? "text-red-500" : "text-green-500"}`}
        >
          {FormatAmount(aggregateValue ?? 0)}
        </h2>
      </div>
    </div>
  );
}

export default GlobalAggregateCard;
