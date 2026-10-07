import type {
  AggregatesFilter,
  FilteredAggregatesResponse,
  GlobalAggregatesResponse,
} from "../../types/financialAggregatesTypes";
import http from "./http";

const getGlobalAggregates = async (): Promise<GlobalAggregatesResponse> => {
  try {
    return await http.get<GlobalAggregatesResponse, undefined>(
      "financialAggregates/global",
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

const getByFilterAsync = async (
  aggregatesFilter: AggregatesFilter,
): Promise<FilteredAggregatesResponse[]> => {
  try {
    return await http.get<FilteredAggregatesResponse[], AggregatesFilter>(
      "financialAggregates/filter",
      aggregatesFilter,
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

export default { getByFilterAsync, getGlobalAggregates };
