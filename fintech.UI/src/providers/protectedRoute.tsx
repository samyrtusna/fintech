import type { ReactNode } from "react";
import { useAppSelector } from "../state/stateHooks";
import { Navigate, useLocation } from "react-router-dom";

interface ProtectedRouteProps {
  children: ReactNode;
}

function ProtectedRoute({ children }: ProtectedRouteProps) {
  const token = useAppSelector((state) => state.authUser.accessToken);
  const location = useLocation();
  const effectiveToken = token ?? localStorage.getItem("accessToken");

  if (!effectiveToken) {
    return (
      <Navigate
        to="/login"
        state={{ from: location }}
        replace
      />
    );
  }

  return <>{children}</>;
}

export default ProtectedRoute;
