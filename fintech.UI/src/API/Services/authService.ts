import type { AxiosError } from "axios";
import http from "./http";
import type { LoginRequest, RegisterRequest } from "../../types/authTypes";

const signup = async (credentials: RegisterRequest): Promise<string> => {
  try {
    const response = await http.post<string, RegisterRequest, undefined>(
      "auth/register/",
      credentials,
    );
    return response;
  } catch (error: unknown) {
    const axiosError = error as AxiosError;
    if (axiosError.response?.status === 400) {
      throw new Error("Email is already in use", { cause: error });
    }
    throw new Error("Signup failed", { cause: error });
  }
};

const login = async (Credentials: LoginRequest): Promise<string> => {
  try {
    const response = await http.post<string, LoginRequest, undefined>(
      "auth/login/",
      Credentials,
    );
    return response;
  } catch (error: unknown) {
    const axiosError = error as AxiosError;
    if (axiosError.response?.status === 400) {
      throw new Error("Invalid email or password", { cause: error });
    }
    throw new Error("Login failed", { cause: error });
  }
};

const logout = async (): Promise<string> => {
  try {
    const response = await http.post<string, undefined, undefined>(
      "auth/logout/",
    );
    return response;
  } catch (error: unknown) {
    const axiosError = error as AxiosError;
    if (axiosError.response?.status === 401) {
      throw new Error("Missing refresh token", { cause: error });
    }
    throw new Error("Logout failed", { cause: error });
  }
};

const refreshToken = async (): Promise<string> => {
  try {
    const response = await http.post<string, undefined, undefined>(
      "auth/refresh-token/",
    );
    return response;
  } catch (error: unknown) {
    const axiosError = error as AxiosError;
    if (axiosError.response?.status === 401) {
      throw new Error("invalid refresh token", { cause: error });
    }
    throw new Error("refresh token failed", { cause: error });
  }
};

export default { signup, login, logout, refreshToken };
