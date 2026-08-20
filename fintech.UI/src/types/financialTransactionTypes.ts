export const FinancialTypes = {
  Income: "Income",
  Expense: "Expense",
  Investment: "Investment",
  ContractedLoan: "ContractedLoan",
  Debt: "Debt",
  PrincipalRepayment: "PrincipalRepayment",
  InterestPayment: "InterestPayment",
  Savings: "Savings",
} as const;

export type FinancialTypes = keyof typeof FinancialTypes;

export interface UpdateTransactionRequest {
  description?: string;
  isEssential?: boolean;
}
export interface NewTransactionRequest extends UpdateTransactionRequest {
  categoryId: string;
  amount: number;
  currency: string;
  transactionDate?: Date;
}

export interface GetTransactionResponse {
  id: string;
  type: FinancialTypes;
  exchangeRate: number;
  baseAmount: number;
  categoryName: string;
  categoryId: string;
  amount: number;
  currency: string;
  transactionDate?: string;
  description?: string;
  isEssential?: boolean;
}

export interface PaginatedTransactions {
  items: GetTransactionResponse[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export interface TransactionsFilter {
  year: number;
  month: number;
  day?: number;
  categoryId?: string;
  isEssential?: boolean;
  page: number;
  pageSize: number;
}

export interface UpdateTransactionPayload {
  id: string;
  bodyObject: UpdateTransactionRequest;
}

export interface TransactionState {
  transactions: PaginatedTransactions | null;
}

export interface MonthItem {
  key: string;
  label: string;
  year: number;
  month: number;
}

export interface FinancialTransactionProps {
  transaction: GetTransactionResponse;
  baseCurrency: string;
  handleBack: () => void;
}

export interface UpdateStateType {
  description: string;
  isEssential: boolean;
}
