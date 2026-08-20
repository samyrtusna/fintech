import type { GetSymbolsResponse } from "../../types/exchangeRateApiTypes";
import http from "./http";

const getCurrencies = async (): Promise<GetSymbolsResponse> => {
  try {
    return await http.get<GetSymbolsResponse, undefined>(
      "exchangeRateAPI/symbols",
    );
  } catch (error) {
    if (error instanceof Error) {
      throw new Error(`Failed to get currency symbols: ${error.message}`, {
        cause: error,
      });
    }
    throw new Error("Failed to get currency symbols: unknown error", {
      cause: error,
    });
  }
};

export default { getCurrencies };
