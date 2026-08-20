import { createSlice, type PayloadAction } from "@reduxjs/toolkit";
import type {
  CategoryState,
  GetCategoryResponse,
} from "../../types/categoryTypes";

const initialState: CategoryState = {
  categories: [],
};

export const categorySlice = createSlice({
  name: "categories",
  initialState,
  reducers: {
    setCategories: (state, action: PayloadAction<GetCategoryResponse[]>) => {
      state.categories = action.payload;
    },
  },
});
export const { setCategories } = categorySlice.actions;
export default categorySlice.reducer;
