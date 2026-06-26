import type {
  GetTransactionResponse,
  NewTransactionRequest,
  PaginatedTransactions,
  TransactionsFilter,
  UpdateTransactionRequest,
} from "../../types/financialTransactionTypes";
import http from "./http";

const addAsync = async (
  bodyObject: NewTransactionRequest,
): Promise<GetTransactionResponse> => {
  try {
    return await http.post<
      GetTransactionResponse,
      NewTransactionRequest,
      undefined
    >("financialTransaction", bodyObject);
  } catch (error) {
    if (error instanceof Error) {
      throw new Error(`Failed to add transaction: ${error.message}`, {
        cause: error,
      });
    }
    throw new Error("Failed to add transaction: Unknown error", {
      cause: error,
    });
  }
};

const getByFilterAsync = async (
  transactionFilter: TransactionsFilter,
): Promise<PaginatedTransactions> => {
  try {
    return await http.get<PaginatedTransactions, TransactionsFilter>(
      "financialTransaction/filter",
      transactionFilter,
    );
  } catch (error) {
    if (error instanceof Error) {
      throw new Error(`Failed to get filtered transactions: ${error.message}`, {
        cause: error,
      });
    }
    throw new Error("Failed to get filtered transactions: Unknown error", {
      cause: error,
    });
  }
};

const getByIdAsync = async (id: string): Promise<GetTransactionResponse> => {
  try {
    return await http.get<GetTransactionResponse, undefined>(
      `financialTransaction/${id}`,
    );
  } catch (error) {
    if (error instanceof Error) {
      throw new Error(`Failed to get a transaction: ${error.message}`, {
        cause: error,
      });
    }
    throw new Error("Failed to get a transaction: Unknown error", {
      cause: error,
    });
  }
};

const updateAsync = async (
  id: string,
  bodyObject: UpdateTransactionRequest,
): Promise<GetTransactionResponse> => {
  try {
    return await http.put<
      GetTransactionResponse,
      UpdateTransactionRequest,
      undefined
    >(`financialTransaction/${id}`, bodyObject);
  } catch (error) {
    if (error instanceof Error) {
      throw new Error(`Failed to update transaction: ${error.message}`, {
        cause: error,
      });
    }
    throw new Error("Failed to update transaction: Unknown error", {
      cause: error,
    });
  }
};

const deleteAsync = async (id: string) => {
  try {
    await http.delete<void, undefined>(`financialTransaction/${id}`);
  } catch (error) {
    if (error instanceof Error) {
      throw new Error(`Failed to delete transaction: ${error.message}`, {
        cause: error,
      });
    }
    throw new Error("Failed to delete transaction: Unknown error", {
      cause: error,
    });
  }
};

export default {
  addAsync,
  getByFilterAsync,
  getByIdAsync,
  updateAsync,
  deleteAsync,
};
