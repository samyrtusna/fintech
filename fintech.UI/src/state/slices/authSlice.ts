import {
  createAsyncThunk,
  createSlice,
  type PayloadAction,
} from "@reduxjs/toolkit";
import type {
  AuthInitialState,
  RegisterRequest,
  LoginRequest,
} from "../../types/authTypes";
import authService from "../../API/Services/authService";

const initialState: AuthInitialState = {
  loading: false,
  accessToken: null,
  error: "",
};

export const registerUser = createAsyncThunk(
  "auth/register",
  async (userData: RegisterRequest, thunkApi) => {
    try {
      return authService.signup(userData);
    } catch (error) {
      return thunkApi.rejectWithValue(
        error instanceof Error ? error.message : "Failed to sign up",
      );
    }
  },
);

export const loginUser = createAsyncThunk(
  "auth/login",
  async (userData: LoginRequest, thunkApi) => {
    try {
      return await authService.login(userData);
    } catch (error) {
      return thunkApi.rejectWithValue(
        error instanceof Error ? error.message : "Failed to sign in",
      );
    }
  },
);

export const refreshToken = createAsyncThunk(
  "auth/refresh-token/",
  async (_, thunkApi) => {
    try {
      return await authService.refreshToken();
    } catch (error) {
      return thunkApi.rejectWithValue(
        error instanceof Error ? error.message : "Failed to refresh token",
      );
    }
  },
);

const authUserSlice = createSlice({
  name: "authUser",
  initialState,
  reducers: {
    logout: (state) => {
      state.accessToken = null;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(registerUser.pending, (state) => {
        state.loading = true;
      })
      .addCase(loginUser.pending, (state) => {
        state.loading = true;
      })
      .addCase(refreshToken.pending, (state) => {
        state.loading = true;
      })
      .addCase(
        registerUser.fulfilled,
        (state, action: PayloadAction<string>) => {
          state.loading = false;
          state.accessToken = action.payload;
        },
      )
      .addCase(loginUser.fulfilled, (state, action: PayloadAction<string>) => {
        state.loading = false;
        state.accessToken = action.payload;
      })
      .addCase(
        refreshToken.fulfilled,
        (state, action: PayloadAction<string>) => {
          state.loading = false;
          state.accessToken = action.payload;
        },
      )

      .addCase(registerUser.rejected, (state, action) => {
        state.loading = false;
        state.accessToken = null;
        state.error = (action.payload as string) || "Login failed";
      })
      .addCase(loginUser.rejected, (state, action) => {
        state.loading = false;
        state.accessToken = null;
        state.error = (action.payload as string) || "Login failed";
      })
      .addCase(refreshToken.rejected, (state, action) => {
        state.loading = false;
        state.accessToken = null;
        state.error = (action.payload as string) || "Token refresh failed";
      });
  },
});

export const { logout } = authUserSlice.actions;
export default authUserSlice.reducer;
