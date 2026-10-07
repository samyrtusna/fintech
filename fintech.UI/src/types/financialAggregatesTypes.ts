type AggregateName =
  | "netBalance"
  | "netCashFlow"
  | "totalInvestment"
  | "totalSavings"
  | "activeDebt"
  | "totalIncome"
  | "totalExpense"
  | "totalContractedLoans"
  | "totalInterestPayments"
  | "totalPrincipalRepayments"
  | "topSpendingCategory"
  | "totalInflow"
  | "totalOutflow"
  | "savingsRate"
  | "essentialExpensesRatio"
  | "expensesRatio"
  | "investmentRatio"
  | "debtRatio"
  | "debtPaymentRatio"
  | "interestBurdenRatio";

export interface FilteredAggregatesResponse {
  year: number;
  month?: number;
  name: AggregateName;
  value: number;
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
}
