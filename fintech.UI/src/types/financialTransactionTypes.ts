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
  currencySymbol: string;
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
  currencySymbol: string;
  baseCurrency: string;
  baseCurrencySymbol: string;
  transactionDate: string;
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
  date: Date;
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
  value: Date;
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
