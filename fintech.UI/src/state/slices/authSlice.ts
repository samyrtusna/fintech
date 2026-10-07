import { createSlice, type PayloadAction } from "@reduxjs/toolkit";
import type { UserIformations, UserState } from "../../types/authTypes";

const initialState: UserState = {
  accessToken: null,
  userInformations: null,
};

const authUserSlice = createSlice({
  name: "AuthUser",
  initialState,
  reducers: {
    setAccessToken: (state, action: PayloadAction<string | null>) => {
      state.accessToken = action.payload;
    },
    setUserInformations: (
      state,
      action: PayloadAction<UserIformations | null>,
    ) => {
      state.userInformations = action.payload;
    },
    logout: (state) => {
      state.accessToken = null;
    },
  },
});

export const { setAccessToken, setUserInformations, logout } =
  authUserSlice.actions;
export default authUserSlice.reducer;
