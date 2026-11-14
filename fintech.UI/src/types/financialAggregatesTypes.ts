import type { FinancialTypes } from "./financialTransactionTypes";

export interface MonthlyAggregatesResponse {
  aggregates: FinancialAggregatesType[];
  periodicVariations: AggregateVariationsByMonthType[];
}

export interface AggregatesState {
  globalAggregates: FinancialAggregatesType[] | null;
  monthAggregates: MonthlyAggregatesResponse | null;
}

export interface FinancialAggregatesType {
  period: Period;
  periodType?: "MonthlyAggregate" | "YearlyAggregate" | "GlobalAggregate";
  type?: FinancialTypes;
  name: string;
  amount: Money;
  variation?: number;
}

export interface Period {
  startDate: string;
  endDate: string;
}

export interface Money {
  amount: number;
  currency: Currency;
}

export interface Currency {
  code: string;
  symbol: string;
}

export interface AggregateVariationsByMonthType {
  name: string;
  values: AggregateVariationsType[];
}

interface AggregateVariationsType {
  period: Period;
  periodType: "MonthlyAggregate" | "YearlyAggregate" | "GlobalAggregate";
  type: FinancialTypes;
  amount: Money;
}

export interface ChartDataPoint {
  month: Date;
  amount: number;
}

export interface FormattedChartData {
  key: string;
  value: ChartDataPoint[];
}
