import { MdTrendingDown, MdTrendingUp } from "react-icons/md";
import ScrollReveal from "./ScrollReveal";

interface PropsType {
  title: string;
  aggregateValue: number;
  aggregateRatio?: number;
  aggregateVariation: number;
  symbol: string;
  isAnActif: boolean;
  isBaseElement: boolean;
}
function PeriodicAggregateCard(props: PropsType) {
  const {
    title,
    aggregateValue,
    aggregateRatio,
    aggregateVariation,
    symbol,
    isAnActif,
    isBaseElement,
  } = props;

  return (
    <ScrollReveal>
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

        <div className="flex justify-center items-center my-2">
          <h3
            className={`text-xl font-bold text-center mr-1 ${(isAnActif && aggregateVariation >= 0) || (!isAnActif && aggregateVariation < 0) ? "text-green-500" : "text-red-500"}`}
          >
            {aggregateVariation?.toFixed()}%
          </h3>
          {aggregateVariation > 0 ? (
            <MdTrendingUp
              className={`size-6 ${isAnActif ? "text-green-500" : "text-red-500"}`}
            />
          ) : aggregateVariation < 0 ? (
            <MdTrendingDown
              className={`size-6 ${isAnActif ? "text-red-500" : "text-green-500"}`}
            />
          ) : null}
        </div>
      </div>
    </ScrollReveal>
  );
}

export default PeriodicAggregateCard;
