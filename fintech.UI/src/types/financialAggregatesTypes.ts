type AggregateName =
  | "NetBalance"
  | "NetCashFlow"
  | "TotalInvestment"
  | "TotalSavings"
  | "TctiveDebt"
  | "TotalIncome"
  | "TotalExpense"
  | "TotalContractedLoans"
  | "TotalInterestPayments"
  | "TotalPrincipalRepayments"
  | "TopSpendingCategory"
  | "TotalInflow"
  | "TotalOutflow"
  | "SavingsRate"
  | "EssentialExpensesRatio"
  | "ExpensesRatio"
  | "InvestmentRatio"
  | "DebtRatio"
  | "DebtPaymentRatio"
  | "InterestBurdenRatio";

export interface FilteredAggregatesResponse {
  date: Date;
  name: AggregateName;
  value: number;
  variation: number;
}

export interface AggregatesFilter {
  period: string;
  date: Date;
}

export interface GlobalAggregatesResponse {
  date: Date;
  totalCashFlow: number;
  totalInvestment: number;
  totalSavings: number;
  totalActiveDebt: number;
}

export interface AggregatesState {
  globalAggregates: GlobalAggregatesResponse | null;
  monthAggregates: FilteredAggregatesResponse[] | null;
}
