import { createSlice, type PayloadAction } from "@reduxjs/toolkit";
import type {
  CategoryState,
  GetCategoryResponse,
} from "../../types/categoryTypes";

const initialState: CategoryState = {
  items: [],
};

export const categorySlice = createSlice({
  name: "categories",
  initialState,
  reducers: {
    setCategories: (state, action: PayloadAction<GetCategoryResponse[]>) => {
      state.items = action.payload;
    },
  },
});
export const { setCategories } = categorySlice.actions;
export default categorySlice.reducer;
