import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";
import type { GetCategoryResponse } from "../../types/categoryTypes";
import categoryService from "../../API/Services/categoryService";

interface CategoryState {
  loading: boolean;
  categories: GetCategoryResponse[];
  error: string | null;
}

const initialState: CategoryState = {
  loading: false,
  categories: [],
  error: null,
};

export const fetchCategories = createAsyncThunk(
  "category/fetchCategories",
  async (_, thunkApi) => {
    try {
      return await categoryService.getAllAsync();
    } catch (error) {
      return thunkApi.rejectWithValue(
        error instanceof Error ? error.message : "Failed to fetch categories",
      );
    }
  },
);

export const categorySlice = createSlice({
  name: "categoryService",
  initialState,
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(fetchCategories.pending, (state) => {
        state.loading = true;
      })
      .addCase(fetchCategories.fulfilled, (state, action) => {
        state.loading = false;
        state.categories = action.payload;
      })
      .addCase(fetchCategories.rejected, (state, action) => {
        state.loading = false;
        state.categories = [];
        state.error = action.payload as string;
      });
  },
});

export default categorySlice.reducer;
