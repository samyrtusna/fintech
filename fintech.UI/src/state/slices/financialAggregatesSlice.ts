import { createSlice, type PayloadAction } from "@reduxjs/toolkit";
import type {
  AggregatesState,
  GlobalAggregatesResponse,
} from "../../types/financialAggregatesTypes";

const initialState: AggregatesState = {
  globalAggregates: null,
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
  },
});

export const { setGlobalAggregates } = financialAggregatesSlice.actions;
export default financialAggregatesSlice.reducer;
