interface PropsType {
  title: string;
  AggregateName: string;
  aggregateValue: number;
  aggregateRatio?: number;
  aggregateVariation: number;
  condition: boolean;
  symbol: string;
  isAnActif: boolean;
  isBaseElement: boolean;
}
function PeriodicAggregateCard(props: PropsType) {
  const {
    title,
    AggregateName,
    aggregateValue,
    aggregateRatio,
    aggregateVariation,
    condition,
    symbol,
    isAnActif,
    isBaseElement,
  } = props;
  return (
    <div className="w-full h-fit p-6 my-5 border-thin border-border-subtle rounded-md bg-bg-contrast shadow-card">
      <progress
        value={aggregateRatio}
        max={100}
        className={` ${isBaseElement ? "hidden" : "block"}
                          w-full h-3
                          appearance-none
                          overflow-hidden
                          rounded-full
                          bg-bg-surface
                          [&::-webkit-progress-bar]:bg-bg-surface
                          [&::-webkit-progress-bar]:rounded-full
                          [&::-webkit-progress-value]:bg-green-500
                          [&::-webkit-progress-value]:rounded-full
                          [&::-moz-progress-bar]:bg-green-500
                          [&::-moz-progress-bar]:rounded-full`}
      ></progress>
      <h3 className="text-2xl mt-3 text-center font-bold">{title}</h3>

      <h3
        className={`text-4xl text-center font-bold mt-2  ${isAnActif ? "text-green-500" : "text-red-500"}`}
      >
        <span>{symbol}</span>
        {aggregateValue}
      </h3>
      <h3 className="text-2xl text-center font-semibold">{AggregateName}</h3>

      <h3
        className={`text-xl font-bold text-center ${condition ? "text-green-500" : "text-red-500"}`}
      >
        {aggregateVariation?.toFixed()}%
      </h3>
      <p className="text-lg font-bold text-center">
        {aggregateVariation > 0 ? (
          <span className="mr-1">Increase</span>
        ) : (
          <span className="mr-1">Decrease</span>
        )}
        vs, previous month
      </p>
      <div className={`${isBaseElement ? "hidden" : "block"}`}>
        <h3
          className={`text-xl font-bold text-center ${condition ? "text-green-500" : "text-red-500"}`}
        >
          {aggregateRatio}%
        </h3>
        <p className="text-lg font-bold text-center">
          Percentage of income spent
        </p>
      </div>
    </div>
  );
}

export default PeriodicAggregateCard;
