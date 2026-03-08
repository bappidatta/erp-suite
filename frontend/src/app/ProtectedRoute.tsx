import { Navigate, Outlet, useLocation } from "react-router-dom";
import { useAuth } from "@shared/auth/auth-context";
import { DashboardLayout } from "@app/components/layout/DashboardLayout";

export function ProtectedRoute() {
  const { isAuthenticated } = useAuth();
  const location = useLocation();

  if (!isAuthenticated) {
    return <Navigate to="/login" replace state={{ from: location }} />;
  }

  return (
    <DashboardLayout>
      <Outlet />
    </DashboardLayout>
  );
}
