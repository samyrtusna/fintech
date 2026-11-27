import { useEffect, type ReactNode } from "react";
import { useDispatch } from "react-redux";
import {
  setAccessToken,
  logout,
  setUserInformations,
} from "../state/slices/authSlice";
import authService from "../API/Services/authService";
import userService from "../API/Services/userService";

export function AuthProvider({ children }: { children: ReactNode }) {
  const dispatch = useDispatch();

  useEffect(() => {
    let isMounted = true;

    const initializeAuth = async () => {
      const existingToken = localStorage.getItem("accessToken");

      try {
        if (existingToken) {
          // Response interceptor will auto-refresh if existingToken is expired
          const userResponse = await userService.getUserAsync();
          if (isMounted) {
            dispatch(setUserInformations(userResponse));
          }
          return;
        }

        // No accessToken in storage: try silent refresh via httpOnly cookie
        const tokenResponse = await authService.refreshToken();
        if (!isMounted) return;

        dispatch(setAccessToken(tokenResponse));
        const userResponse = await userService.getUserAsync();
        if (isMounted) {
          dispatch(setUserInformations(userResponse));
        }
      } catch {
        if (isMounted && existingToken) {
          dispatch(logout());
        }
      }
    };

    initializeAuth();

    return () => {
      isMounted = false;
    };
  }, [dispatch]);

  return <>{children}</>;
}
