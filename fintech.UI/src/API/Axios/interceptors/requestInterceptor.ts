import AxiosService from "../axiosInstance";
import type { AxiosError } from "axios";

export function requestInterceptor() {
  AxiosService.interceptors.request.use(
    (config) => {
      const token = localStorage.getItem("accessToken");
      if (token) {
        config.headers.Authorization = `Bearer ${token}`;
      }
      return config;
    },
    (error: AxiosError) => {
      return Promise.reject(error);
    },
  );
}
