import type {
  BudgetParamType,
  GetBudgetResponse,
  NewBudgetRequest,
  UpdateBudgetType,
} from "../../types/budgetTypes";
import http from "./http";

const addAsync = async (
  bodyObject: NewBudgetRequest,
): Promise<GetBudgetResponse> => {
  try {
    return await http.post<GetBudgetResponse, NewBudgetRequest, undefined>(
      "budget/new",
      bodyObject,
    );
  } catch (error) {
    if (error instanceof Error) {
      throw new Error(`Failed to add budget: ${error.message}`, {
        cause: error,
      });
    }
    throw new Error("Failed to add budget: Unknown error", {
      cause: error,
    });
  }
};

const getByMonthAsync = async (
  month: BudgetParamType,
): Promise<GetBudgetResponse[]> => {
  try {
    return await http.get<GetBudgetResponse[], BudgetParamType>(
      "budget/byMonth",
      month,
    );
  } catch (error) {
    if (error instanceof Error) {
      throw new Error(`Failed to get month budgets: ${error.message}`, {
        cause: error,
      });
    }
    throw new Error("Failed to get month budgets: Unknown error", {
      cause: error,
    });
  }
};

const getByIdAsync = async (id: string): Promise<GetBudgetResponse> => {
  try {
    return await http.get<GetBudgetResponse, undefined>(`budget/${id}`);
  } catch (error) {
    if (error instanceof Error) {
      throw new Error(`Failed to get a budget: ${error.message}`, {
        cause: error,
      });
    }
    throw new Error("Failed to get a budget: Unknown error", {
      cause: error,
    });
  }
};

const updateAsync = async (
  id: string,
  bodyObject: UpdateBudgetType,
): Promise<GetBudgetResponse> => {
  try {
    return await http.put<GetBudgetResponse, UpdateBudgetType, undefined>(
      `budget/${id}`,
      bodyObject,
    );
  } catch (error) {
    if (error instanceof Error) {
      throw new Error(`Failed to update budget: ${error.message}`, {
        cause: error,
      });
    }
    throw new Error("Failed to update budget: Unknown error", {
      cause: error,
    });
  }
};

const deleteAsync = async (id: string) => {
  try {
    await http.delete<void, undefined>(`budget/${id}`);
  } catch (error) {
    if (error instanceof Error) {
      throw new Error(`Failed to delete budget: ${error.message}`, {
        cause: error,
      });
    }
    throw new Error("Failed to delete budget: Unknown error", {
      cause: error,
    });
  }
};

export default {
  addAsync,
  getByMonthAsync,
  getByIdAsync,
  updateAsync,
  deleteAsync,
};
