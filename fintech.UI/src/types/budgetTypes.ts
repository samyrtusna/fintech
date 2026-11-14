import type { Period } from "./financialAggregatesTypes";
import type { Money } from "./financialTransactionTypes";

export type BudgetType = "Global" | "ByCategory";

export interface GetBudgetResponse {
  id: string;
  categoryId: string;
  type: BudgetType;
  amount: Money;
  period: Period;
  categoryName: string;
  budgetConsumption: number;
  budgetConsumptionPercentage: number;
}

export interface BudgetSliceState {
  budgets: GetBudgetResponse[] | null;
}

export interface BudgetParamType {
  month: string;
}

export interface UpdateBudgetType {
  amount: number;
  currency: {
    code: string;
    symbol: string;
  };
}

export interface NewBudgetValuesType {
  categoryId: string;
  type: BudgetType;
  amount: Money;
}

export interface NewBudgetRequest extends NewBudgetValuesType {
  period: Period;
}
