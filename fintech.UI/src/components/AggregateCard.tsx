import FormatAmount from "../helpers/amountFormatter";

interface PropsType {
  aggregateName: string;
  aggregateValue: number | undefined;
  condition: boolean;
  symbol: string;
}

function AggregateCard(props: PropsType) {
  const { aggregateName, symbol, aggregateValue, condition } = props;
  return (
    <div className="flex flex-col justify-center lg:flex lg:flex-row lg:justify-between items-center py-2 w-1/6 lg:w-fit lg:px-2 bg-bg-contrast border-thin border-border-subtle rounded-md animate-slow-drift">
      <h3 className="md:text-lg md:mr-4">{aggregateName} </h3>
      <div className="flex items-baseline">
        <h3
          className={`text-lg ${condition ? "text-red-500" : "text-green-500"}`}
        >
          {symbol}
        </h3>
        <h2
          className={`md:text-xl font-bold ml-1 ${condition ? "text-red-500" : "text-green-500"}`}
        >
          {FormatAmount(aggregateValue ?? 0)}
        </h2>
      </div>
    </div>
  );
}

export default AggregateCard;
