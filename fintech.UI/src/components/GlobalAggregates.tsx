import GlobalAggregateCard from "./GlobalAggregateCard";
import ScrollReveal from "./ScrollReveal";

interface PropsType {
  debtValue: number | undefined;
  investmentValue: number | undefined;
  cashFlowValue: number | undefined;
  savingValue: number | undefined;
  symbol: string;
}
function GlobalAggregates(props: PropsType) {
  const { debtValue, investmentValue, cashFlowValue, savingValue, symbol } =
    props;
  return (
    <ScrollReveal className="w-full">
      <div className="w-full grid grid-cols-2 md:grid-cols-4 gap-4 justify-around">
        <GlobalAggregateCard
          aggregateName="Debt"
          symbol={symbol}
          aggregateValue={debtValue}
          condition={(debtValue ?? 0) > 20000}
        />

        <GlobalAggregateCard
          aggregateName="Investment"
          symbol={symbol}
          aggregateValue={investmentValue}
          condition={(investmentValue ?? 0) < 1000}
        />

        <GlobalAggregateCard
          aggregateName="CashFlow"
          symbol={symbol}
          aggregateValue={cashFlowValue}
          condition={(cashFlowValue ?? 0) < 200}
        />

        <GlobalAggregateCard
          aggregateName="Saving"
          symbol={symbol}
          aggregateValue={savingValue}
          condition={(savingValue ?? 0) < 200}
        />
      </div>
    </ScrollReveal>
  );
}

export default GlobalAggregates;
