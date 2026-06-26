import AxiosService from "../Axios/axiosInstance";

type MethodType = "get" | "post" | "put" | "delete";

const request = async <TResponse, TBody, TParams>(
  method: MethodType,
  route: string,
  body?: TBody,
  params?: TParams,
) => {
  const response = await AxiosService.request<TResponse>({
    method,
    url: route,
    data: body,
    params,
  });
  return response.data;
};

const Get = <TResponse, TParams>(route: string, params?: TParams) =>
  request<TResponse, undefined, TParams>("get", route, undefined, params);

const Post = <TResponse, TBody, TParams>(
  route: string,
  body?: TBody,
  params?: TParams,
) => request<TResponse, TBody, TParams>("post", route, body, params);

const Put = <TResponse, TBody, TParams>(
  route: string,
  body: TBody,
  params?: TParams,
) => request<TResponse, TBody, TParams>("put", route, body, params);

const Delete = <TResponse, TParams>(route: string, params?: TParams) =>
  request<TResponse, undefined, TParams>("delete", route, undefined, params);

const http = { get: Get, post: Post, put: Put, delete: Delete };

export default http;
