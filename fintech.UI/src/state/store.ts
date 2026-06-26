import { configureStore } from "@reduxjs/toolkit";
import authUserReducer from "./slices/authSlice";
import transactionsReducer from "./slices/financialTransactionSlice";
import categoriesReducer from "./slices/categorySlice";

const store = configureStore({
  reducer: {
    authUser: authUserReducer,
    transactions: transactionsReducer,
    categories: categoriesReducer,
  },
  devTools: import.meta.env.MODE !== "production",
});

export default store;
export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
