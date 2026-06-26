import type {
  GetCategoryResponse,
  NewCategoryRequest,
  UpdateCategoryRequest,
} from "../../types/categoryTypes";
import http from "./http";

const addAsync = async (
  bodyObject: NewCategoryRequest,
): Promise<GetCategoryResponse> => {
  try {
    return await http.post<GetCategoryResponse, NewCategoryRequest, undefined>(
      "category",
      bodyObject,
    );
  } catch (error) {
    if (error instanceof Error) {
      throw new Error(`Failed to add new category: ${error.message}`, {
        cause: error,
      });
    }
    throw new Error("Failed to add new category: Unknown error", {
      cause: error,
    });
  }
};

const getAllAsync = async (): Promise<GetCategoryResponse[]> => {
  try {
    return await http.get<GetCategoryResponse[], undefined>("category");
  } catch (error) {
    if (error instanceof Error) {
      throw new Error(`Failed to get categories: ${error.message}`, {
        cause: error,
      });
    }
    throw new Error("Failed to get categories: Unknown error", {
      cause: error,
    });
  }
};

const getByIdAsync = async (id: string): Promise<GetCategoryResponse> => {
  try {
    return await http.get<GetCategoryResponse, undefined>(`category/${id}`);
  } catch (error) {
    if (error instanceof Error) {
      throw new Error(`Failed to get a category: ${error.message}`, {
        cause: error,
      });
    }
    throw new Error("Failed to get a category: Unknown error", {
      cause: error,
    });
  }
};

const updateAsync = async (
  id: string,
  bodyObject: UpdateCategoryRequest,
): Promise<GetCategoryResponse> => {
  try {
    return await http.put<
      GetCategoryResponse,
      UpdateCategoryRequest,
      undefined
    >(`category/${id}`, bodyObject);
  } catch (error) {
    if (error instanceof Error) {
      throw new Error(`Failed to update category: ${error.message}`, {
        cause: error,
      });
    }
    throw new Error("Failed to update category: Unknown error", {
      cause: error,
    });
  }
};

const deleteAsync = async (id: string) => {
  try {
    await http.delete<void, undefined>(`category/${id}`);
  } catch (error) {
    if (error instanceof Error) {
      throw new Error(`Failed to delete category: ${error.message}`, {
        cause: error,
      });
    }
    throw new Error("Failed to delete category: Unknown error", {
      cause: error,
    });
  }
};

export default {
  addAsync,
  getAllAsync,
  getByIdAsync,
  updateAsync,
  deleteAsync,
};
