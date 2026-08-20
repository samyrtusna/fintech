import type { AxiosError, AxiosRequestConfig } from "axios";
import AxiosService from "../axiosInstance";
import { logout, setAccessToken } from "../../../state/slices/authSlice";
import authService from "../../Services/authService";
import store from "../../../state/store";

interface CustomAxiosRequestConfig extends AxiosRequestConfig {
  _retry?: boolean;
}

export function responseInterceptor() {
  AxiosService.interceptors.response.use(
    (response) => response,
    async (error: AxiosError) => {
      const originalRequest = error.config as CustomAxiosRequestConfig;
      if (
        error.response?.status === 401 &&
        !originalRequest._retry &&
        !originalRequest.url?.includes("/auth/refresh-token")
      ) {
        originalRequest._retry = true;

        try {
          const accessToken = await authService.refreshToken();
          store.dispatch(setAccessToken(accessToken));
          return AxiosService(originalRequest);
        } catch (refreshTokenError) {
          console.error("Failed to refresh token", refreshTokenError);
          store.dispatch(logout());
          window.location.href = "/login";
          return Promise.reject(refreshTokenError);
        }
      }
      return Promise.reject(error);
    },
  );
}
