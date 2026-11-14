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
    const initializeAuth = async () => {
      try {
        const tokenResponse = await authService.refreshToken();
        dispatch(setAccessToken(tokenResponse));
        const userResponse = await userService.getUserAsync();
        dispatch(setUserInformations(userResponse));
      } catch {
        dispatch(logout());
      }
    };

    initializeAuth();
  }, [dispatch]);

  return <>{children}</>;
}
