import type {
  FinancialAggregatesType,
  MonthlyAggregatesResponse,
} from "../../types/financialAggregatesTypes";
import http from "./http";

const getGlobalAggregates = async (): Promise<FinancialAggregatesType[]> => {
  try {
    return await http.get<FinancialAggregatesType[], { date: Date }>(
      "financialAggregates/global",
      { date: new Date() },
    );
  } catch (error) {
    if (error instanceof Error) {
      throw new Error(`Failed to get global Aggregates: ${error.message}`, {
        cause: error,
      });
    }
    throw new Error("Failed to get global Aggregates: Unknown error", {
      cause: error,
    });
  }
};

const getByMonth = async (
  monthDate: Date,
): Promise<MonthlyAggregatesResponse> => {
  try {
    return await http.get<MonthlyAggregatesResponse, { date: Date }>(
      "financialAggregates/month",
      { date: monthDate },
    );
  } catch (error) {
    if (error instanceof Error) {
      throw new Error(`Failed to get filtered Aggregates: ${error.message}`, {
        cause: error,
      });
    }
    throw new Error("Failed to get filtered Aggregates: Unknown error", {
      cause: error,
    });
  }
};

export default { getByMonth, getGlobalAggregates };
