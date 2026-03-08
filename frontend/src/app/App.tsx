import { Navigate, Route, Routes } from "react-router-dom";
import { DashboardPage } from "@app/DashboardPage";
import { ProtectedRoute } from "@app/ProtectedRoute";
import { LoginPage } from "@modules/admin/pages/LoginPage";

export function App() {
  return (
    <Routes>
      <Route path="/" element={<Navigate to="/dashboard" replace />} />
      <Route path="/login" element={<LoginPage />} />

      <Route element={<ProtectedRoute />}>
        <Route path="/dashboard" element={<DashboardPage />} />
      </Route>

      <Route path="*" element={<Navigate to="/dashboard" replace />} />
    </Routes>
  );
}
