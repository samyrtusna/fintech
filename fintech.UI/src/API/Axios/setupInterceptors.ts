import { requestInterceptor } from "./interceptors/requestInterceptor";
import { responseInterceptor } from "./interceptors/responseInterceptor";

export function setupAxiosInterceptors() {
  requestInterceptor();
  responseInterceptor();
}
