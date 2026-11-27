import { createSlice, type PayloadAction } from "@reduxjs/toolkit";
import type { UserIformations, UserState } from "../../types/authTypes";

const ACCESS_TOKEN_KEY = "accessToken";

const getStoredToken = (): string | null => {
  try {
    return localStorage.getItem(ACCESS_TOKEN_KEY);
  } catch {
    return null;
  }
};

const setStoredToken = (token: string | null) => {
  try {
    if (token) {
      localStorage.setItem(ACCESS_TOKEN_KEY, token);
    } else {
      localStorage.removeItem(ACCESS_TOKEN_KEY);
    }
  } catch {
    // Ignore storage quota/privacy mode errors
  }
};

const initialState: UserState = {
  accessToken: getStoredToken(),
  userInformations: null,
};

const authUserSlice = createSlice({
  name: "AuthUser",
  initialState,
  reducers: {
    setAccessToken: (state, action: PayloadAction<string | null>) => {
      state.accessToken = action.payload;
      setStoredToken(action.payload);
    },
    setUserInformations: (
      state,
      action: PayloadAction<UserIformations | null>,
    ) => {
      state.userInformations = action.payload;
    },
    logout: (state) => {
      state.accessToken = null;
      state.userInformations = null;
      setStoredToken(null);
    },
  },
});

export const { setAccessToken, setUserInformations, logout } =
  authUserSlice.actions;
export default authUserSlice.reducer;
