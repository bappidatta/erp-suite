import { Navigate, Route, Routes } from "react-router-dom";
import { DashboardPage } from "@app/DashboardPage";
import { ProtectedRoute } from "@app/ProtectedRoute";
import { LoginPage } from "@modules/admin/pages/LoginPage";
import { ProfilePage } from "@modules/admin/pages/ProfilePage";
import { RegisterPage } from "@modules/admin/pages/RegisterPage";

export function App() {
  return (
    <Routes>
      <Route path="/" element={<Navigate to="/dashboard" replace />} />
      <Route path="/login" element={<LoginPage />} />
      <Route path="/register" element={<RegisterPage />} />

      <Route element={<ProtectedRoute />}>
        <Route path="/dashboard" element={<DashboardPage />} />
        <Route path="/profile" element={<ProfilePage />} />
      </Route>

      <Route path="*" element={<Navigate to="/dashboard" replace />} />
    </Routes>
  );
}
