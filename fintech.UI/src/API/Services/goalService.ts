import type { MonthParamType } from "../../types/budgetTypes";
import type {
  GetGoalResponse,
  NewGoalRequest,
  UpdateGoalType,
} from "../../types/goalTypes";
import http from "./http";

const addAsync = async (
  bodyObject: NewGoalRequest,
): Promise<GetGoalResponse> => {
  try {
    return await http.post<GetGoalResponse, NewGoalRequest, undefined>(
      "goal/new",
      bodyObject,
    );
  } catch (error) {
    if (error instanceof Error) {
      throw new Error(`Failed to add goal: ${error.message}`, {
        cause: error,
      });
    }
    throw new Error("Failed to add goal: Unknown error", {
      cause: error,
    });
  }
};

const getByMonthAsync = async (
  month: MonthParamType,
): Promise<GetGoalResponse[]> => {
  try {
    return await http.get<GetGoalResponse[], MonthParamType>(
      "goal/byMonth",
      month,
    );
  } catch (error) {
    if (error instanceof Error) {
      throw new Error(`Failed to get month goals: ${error.message}`, {
        cause: error,
      });
    }
    throw new Error("Failed to get month goals: Unknown error", {
      cause: error,
    });
  }
};

const getByIdAsync = async (id: string): Promise<GetGoalResponse> => {
  try {
    return await http.get<GetGoalResponse, undefined>(`goal/${id}`);
  } catch (error) {
    if (error instanceof Error) {
      throw new Error(`Failed to get a goal: ${error.message}`, {
        cause: error,
      });
    }
    throw new Error("Failed to get a goal: Unknown error", {
      cause: error,
    });
  }
};

const updateAsync = async (
  id: string,
  bodyObject: UpdateGoalType,
): Promise<GetGoalResponse> => {
  try {
    return await http.put<GetGoalResponse, UpdateGoalType, undefined>(
      `goal/${id}`,
      bodyObject,
    );
  } catch (error) {
    if (error instanceof Error) {
      throw new Error(`Failed to update goal: ${error.message}`, {
        cause: error,
      });
    }
    throw new Error("Failed to update goal: Unknown error", {
      cause: error,
    });
  }
};

const deleteAsync = async (id: string) => {
  try {
    await http.delete<void, undefined>(`goal/${id}`);
  } catch (error) {
    if (error instanceof Error) {
      throw new Error(`Failed to delete goal: ${error.message}`, {
        cause: error,
      });
    }
    throw new Error("Failed to delete goal: Unknown error", {
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
