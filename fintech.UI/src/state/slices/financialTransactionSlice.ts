import {
  createAsyncThunk,
  createSlice,
  type PayloadAction,
} from "@reduxjs/toolkit";
import type {
  NewTransactionRequest,
  TransactionsFilter,
  TransactionState,
  UpdateTransactionPayload,
} from "../../types/financialTransactionTypes";
import financialTrascationService from "../../API/Services/financialTrascationService";

const initialState: TransactionState = {
  transactions: null,
  selectedTransaction: null,
  currentFilter: null,
  isLoadingTransactions: false,
  isLoadingTransaction: false,
  isAdding: false,
  isUpdating: false,
  isDeleting: false,
  error: null,
  operationError: null,
};

export const fetchTransactions = createAsyncThunk(
  "financialTransaction/fetchTransactions",
  async (filter: TransactionsFilter, thunkApi) => {
    try {
      return await financialTrascationService.getByFilterAsync(filter);
    } catch (error) {
      return thunkApi.rejectWithValue(
        error instanceof Error ? error.message : "Failed to fetch transactions",
      );
    }
  },
);

export const addTransaction = createAsyncThunk(
  "financialTransactoion/addTransaction",
  async (bodyObject: NewTransactionRequest, thunkApi) => {
    try {
      return await financialTrascationService.addAsync(bodyObject);
    } catch (error) {
      return thunkApi.rejectWithValue(
        error instanceof Error ? error.message : "Failed to add transaction",
      );
    }
  },
);

export const updateTransaction = createAsyncThunk(
  "financialTransaction/updateTransaction",
  async ({ id, bodyObject }: UpdateTransactionPayload, thunkApi) => {
    try {
      return await financialTrascationService.updateAsync(id, bodyObject);
    } catch (error) {
      return thunkApi.rejectWithValue(
        error instanceof Error ? error.message : "Failed to update transaction",
      );
    }
  },
);

export const deleteTransaction = createAsyncThunk(
  "financialTransaction/deleteTransaction",
  async (id: string, thunkApi) => {
    try {
      await financialTrascationService.deleteAsync(id);
      return id;
    } catch (error) {
      return thunkApi.rejectWithValue(
        error instanceof Error ? error.message : "Failed to delete transaction",
      );
    }
  },
);

export const financialTransactionSlice = createSlice({
  name: "financialTransaction",
  initialState,

  reducers: {
    clearError: (state) => {
      state.error = null;
      state.operationError = null;
    },
    clearSelectedTransaction: (state) => {
      state.selectedTransaction = null;
    },
    setCurrentFilter: (state, action: PayloadAction<TransactionsFilter>) => {
      state.currentFilter = action.payload;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchTransactions.pending, (state) => {
        state.isLoadingTransactions = true;
        state.error = null;
      })
      .addCase(fetchTransactions.fulfilled, (state, action) => {
        state.isLoadingTransactions = false;
        state.transactions = action.payload;
        state.currentFilter = action.meta.arg;
      })
      .addCase(fetchTransactions.rejected, (state, action) => {
        state.isLoadingTransactions = false;
        state.error = action.payload as string;
      });
    builder
      .addCase(addTransaction.pending, (state) => {
        state.isAdding = true;
        state.operationError = null;
      })
      .addCase(addTransaction.fulfilled, (state) => {
        state.isAdding = false;
        // state.transactions.unshift(action.payload);
      })
      .addCase(addTransaction.rejected, (state, action) => {
        state.isAdding = false;
        state.operationError = action.payload as string;
      });
    builder
      .addCase(updateTransaction.pending, (state) => {
        state.isUpdating = true;
        state.operationError = null;
      })
      .addCase(updateTransaction.fulfilled, (state, action) => {
        state.isUpdating = false;
        const index = state.transactions?.items.findIndex(
          (t) => t.id === action.payload.id,
        );
        if (index !== -1) {
          state.transactions!.items[index!] = action.payload;
        }
        if (state.selectedTransaction?.id === action.payload.id) {
          state.selectedTransaction = action.payload;
        }
      })
      .addCase(updateTransaction.rejected, (state, action) => {
        state.isUpdating = false;
        state.operationError = action.payload as string;
      });
    builder
      .addCase(deleteTransaction.pending, (state) => {
        state.isDeleting = true;
        state.operationError = null;
      })
      .addCase(deleteTransaction.fulfilled, (state, action) => {
        state.isDeleting = false;
        state.transactions!.items = state.transactions!.items.filter(
          (t) => t.id !== action.payload,
        );
        if (state.selectedTransaction?.id === action.payload) {
          state.selectedTransaction = null;
        }
      })
      .addCase(deleteTransaction.rejected, (state, action) => {
        state.isDeleting = false;
        state.operationError = action.payload as string;
      });
  },
});

export const { clearError, clearSelectedTransaction, setCurrentFilter } =
  financialTransactionSlice.actions;

export default financialTransactionSlice.reducer;
