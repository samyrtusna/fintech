import { createSlice, type PayloadAction } from "@reduxjs/toolkit";
import type {
  AggregatesState,
  FilteredAggregatesResponse,
  GlobalAggregatesResponse,
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
      action: PayloadAction<GlobalAggregatesResponse>,
    ) => {
      state.globalAggregates = action.payload;
    },
    setMonthAggregates: (
      state,
      action: PayloadAction<FilteredAggregatesResponse[]>,
    ) => {
      state.monthAggregates = action.payload;
    },
  },
});

export const { setGlobalAggregates, setMonthAggregates } =
  financialAggregatesSlice.actions;
export default financialAggregatesSlice.reducer;
