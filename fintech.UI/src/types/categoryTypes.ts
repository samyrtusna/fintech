import type { FinancialTypes } from "./financialTransactionTypes";

export interface NewCategoryRequest {
  name: string;
  type: FinancialTypes;
  parentCategoryId: string;
}

export interface GetCategoryResponse {
  id: string;
  name: string;
  type: FinancialTypes;
  parentCategoryId?: string;
  parentCategory?: string;
}

export interface UpdateCategoryRequest {
  name: string;
}
