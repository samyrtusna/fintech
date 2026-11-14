import { createSlice, type PayloadAction } from "@reduxjs/toolkit";
import type {
  BudgetSliceState,
  GetBudgetResponse,
} from "../../types/budgetTypes";

const initialState: BudgetSliceState = {
  budgets: null,
};

export const budgetSlice = createSlice({
  name: "budgetSlice",
  initialState,
  reducers: {
    setBudget: (state, action: PayloadAction<GetBudgetResponse[]>) => {
      state.budgets = action.payload;
    },
  },
});

export const { setBudget } = budgetSlice.actions;
export default budgetSlice.reducer;
