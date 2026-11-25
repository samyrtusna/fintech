import { createSlice, type PayloadAction } from "@reduxjs/toolkit";
import type { GetGoalResponse, GoalSliceState } from "../../types/goalTypes";

const initialState: GoalSliceState = {
  goals: null,
};

export const goalSlice = createSlice({
  name: "goalSlice",
  initialState,
  reducers: {
    setGoal: (state, action: PayloadAction<GetGoalResponse[]>) => {
      state.goals = action.payload;
    },
  },
});

export const { setGoal } = goalSlice.actions;
export default goalSlice.reducer;
