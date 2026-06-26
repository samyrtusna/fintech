import axios from "axios";

const apiUrl: string = import.meta.env.VITE_API_URL;

const AxiosService = axios.create({
  baseURL: apiUrl,
  headers: { "Content-Type": "application/json" },
  withCredentials: true,
});

export default AxiosService;
