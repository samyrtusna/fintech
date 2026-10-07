import AggregateCard from "./AggregateCard";

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
    <div className="flex w-full justify-around">
      <AggregateCard
        aggregateName="Debt"
        symbol={symbol}
        aggregateValue={debtValue}
        condition={(debtValue ?? 0) > 20000}
      />
      <AggregateCard
        aggregateName="Investment"
        symbol={symbol}
        aggregateValue={investmentValue}
        condition={(investmentValue ?? 0) < 1000}
      />
      <AggregateCard
        aggregateName="CashFlow"
        symbol={symbol}
        aggregateValue={cashFlowValue}
        condition={(cashFlowValue ?? 0) < 200}
      />
      <AggregateCard
        aggregateName="Saving"
        symbol={symbol}
        aggregateValue={savingValue}
        condition={(savingValue ?? 0) < 200}
      />
    </div>
  );
}

export default GlobalAggregates;
