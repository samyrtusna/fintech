import { createSlice, type PayloadAction } from "@reduxjs/toolkit";
import type { AuthInitialState } from "../../types/authTypes";

const initialState: AuthInitialState = {
  accessToken: null,
};

const authUserSlice = createSlice({
  name: "authUser",
  initialState,
  reducers: {
    setAccessToken: (state, action: PayloadAction<string | null>) => {
      state.accessToken = action.payload;
    },
    logout: (state) => {
      state.accessToken = null;
    },
  },
});

export const { setAccessToken, logout } = authUserSlice.actions;
export default authUserSlice.reducer;
