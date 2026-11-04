import { createSlice, type PayloadAction } from "@reduxjs/toolkit";
import type {
  AggregatesState,
  FinancialAggregatesType,
  MonthlyAggregatesResponse,
} from "../../types/financialAggregatesTypes";

const initialState: AggregatesState = {
  globalAggregates: null,
  monthAggregates: null,
};

export const financialAggregatesSlice = createSlice({
  name: "financialAggregates",
  initialState,
  reducers: {
    setGlobalAggregates: (
      state,
      action: PayloadAction<FinancialAggregatesType[]>,
    ) => {
      state.globalAggregates = action.payload;
    },
    setMonthAggregates: (
      state,
      action: PayloadAction<MonthlyAggregatesResponse>,
    ) => {
      state.monthAggregates = action.payload;
    },
  },
});

export const { setGlobalAggregates, setMonthAggregates } =
  financialAggregatesSlice.actions;
export default financialAggregatesSlice.reducer;
