import type { GetCategoryResponse } from "./categoryTypes";
import type { Period } from "./financialAggregatesTypes";
import type { FinancialTypes, Money } from "./financialTransactionTypes";

export interface NewGoalValues {
  title: string;
  categoryId?: string;
  targetType?: FinancialTypes;
  targetAmount: Money;
}
export interface NewGoalRequest extends NewGoalValues {
  period: Period;
}

export interface GetGoalResponse {
  id: string;
  title: string;
  categoryId?: string;
  targetType?: FinancialTypes;
  targetAmount: Money;
  period: Period;
  goalProgress: number;
  category?: GetCategoryResponse;
}

export interface UpdateGoalType {
  title?: string;
  targetAmount?: Money;
}

export interface GoalSliceState {
  goals: GetGoalResponse[] | null;
}
