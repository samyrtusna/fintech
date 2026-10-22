import type { FinancialTypes } from "./financialTransactionTypes";

export interface FilteredAggregatesResponse {
  date: Date;
  totals: FinancialTotalsType[];
  aggregates: CalculatedAggregatesType;
  variations: AggregateVariationType;
  aggregatesVariationsByMonth: AggregateVariationsByMonthType[];
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
  monthAggregates: FilteredAggregatesResponse | null;
}

export interface FinancialTotalsType {
  type: string;
  name: string;
  filter?: string;
  amount: number;
}

export interface CalculatedAggregatesType {
  netBalance: number;
  netCashFlow: number;
  totalInflow: number;
  totalOutflow: number;
  savingsRate: number;
  essentialExpensesRatio: number;
  expensesRatio: number;
  investmentRatio: number;
  debtRatio: number;
  debtPaymentRatio: number;
  interestBurdenRatio: number;
}
export interface AggregateVariationType {
  incomeVariation: number;
  expensesVariation: number;
  savingsVariation: number;
  investmentVariation: number;
  debtVariation: number;
  netBalanceVariation: number;
  netCashFlowVariation: number;
}

export interface AggregateVariationsByMonthType {
  period: Date;
  type: FinancialTypes;
  categoryName?: string;
  amount: number;
}

export interface ChartDataPoint {
  month: Date;
  amount: number;
}

export interface FormattedChartData {
  key: string;
  value: ChartDataPoint[];
}
