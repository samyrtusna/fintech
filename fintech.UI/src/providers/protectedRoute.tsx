import type { ReactNode } from "react";
import { useAppSelector } from "../state/stateHooks";
import { Navigate } from "react-router-dom";

interface ProtectedRouteProps {
  children: ReactNode;
}

function ProtectedRoute({ children }: ProtectedRouteProps) {
  const token = useAppSelector((state) => state.authUser.accessToken);

  if (!token) {
    return (
      <Navigate
        to="/login"
        replace
      />
    );
  }
  return children;
}

export default ProtectedRoute;
