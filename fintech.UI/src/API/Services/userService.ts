import type { UserIformations } from "../../types/authTypes";
import http from "./http";

const getUserAsync = async (): Promise<UserIformations> => {
  try {
    return await http.get<UserIformations, undefined>("user/informations");
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

export default { getUserAsync };
