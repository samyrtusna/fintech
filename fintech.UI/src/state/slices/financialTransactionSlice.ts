import { createSlice, type PayloadAction } from "@reduxjs/toolkit";
import type {
  PaginatedTransactions,
  TransactionState,
} from "../../types/financialTransactionTypes";

const initialState: TransactionState = {
  transactions: null,
};

export const financialTransactionSlice = createSlice({
  name: "financialTransaction",
  initialState,
  reducers: {
    setFinancialTransactions: (
      state,
      action: PayloadAction<PaginatedTransactions>,
    ) => {
      state.transactions = action.payload;
    },
  },
});

export const { setFinancialTransactions } = financialTransactionSlice.actions;
export default financialTransactionSlice.reducer;
