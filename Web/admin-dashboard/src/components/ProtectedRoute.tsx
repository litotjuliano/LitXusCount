import type { ReactNode } from "react";
import { Navigate } from "react-router-dom";
import { authStorage } from "../api/authStorage";

interface ProtectedRouteProps {
  children: ReactNode;
}

const ProtectedRoute = ({ children }: ProtectedRouteProps) => {
  if (!authStorage.isAuthenticated()) {
    return <Navigate to='/sign-in' replace />;
  }

  return children;
};

export default ProtectedRoute;
