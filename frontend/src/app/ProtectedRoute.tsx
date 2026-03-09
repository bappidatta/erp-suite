import { Navigate, Outlet, useLocation } from "react-router-dom";
import { useAuth } from "@shared/auth/auth-context";
import { AppShell } from "@app/components/layout/AppShell";

export function ProtectedRoute() {
  const { isAuthenticated, isRestoring } = useAuth();
  const location = useLocation();

  if (isRestoring) {
    return null;
  }

  if (!isAuthenticated) {
    return <Navigate to="/login" replace state={{ from: location }} />;
  }

  return (
    <AppShell>
      <Outlet />
    </AppShell>
  );
}
