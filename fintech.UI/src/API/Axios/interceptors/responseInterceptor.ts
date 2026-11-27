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
        originalRequest &&
        !originalRequest._retry &&
        !originalRequest.url?.includes("/auth/refresh-token")
      ) {
        originalRequest._retry = true;

        try {
          const newAccessToken = await authService.refreshToken();
          store.dispatch(setAccessToken(newAccessToken));

          originalRequest.headers = {
            ...originalRequest.headers,
            Authorization: `Bearer ${newAccessToken}`,
          };

          return AxiosService(originalRequest);
        } catch (refreshTokenError) {
          console.error("Failed to refresh token", refreshTokenError);
          store.dispatch(logout());
          if (window.location.pathname !== "/login") {
            window.location.href = "/login";
          }
          return Promise.reject(refreshTokenError);
        }
      }

      return Promise.reject(error);
    },
  );
}
