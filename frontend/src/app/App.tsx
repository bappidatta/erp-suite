import { Navigate, Route, Routes } from "react-router-dom";
import { DashboardPage } from "@app/DashboardPage";
import { ProtectedRoute } from "@app/ProtectedRoute";
import { LoginPage } from "@modules/admin/pages/LoginPage";
import { ProfilePage } from "@modules/admin/pages/ProfilePage";
import { RegisterPage } from "@modules/admin/pages/RegisterPage";
import { AdminDashboardPage } from "@modules/admin/pages/AdminDashboardPage";
import { UsersPage } from "@modules/admin/pages/UsersPage";
import { RolesPage } from "@modules/admin/pages/RolesPage";
import { OrganizationSettingsPage } from "@modules/admin/pages/OrganizationSettingsPage";
import { AuditLogPage } from "@modules/admin/pages/AuditLogPage";
import { CustomersPage } from "@modules/sales/pages/CustomersPage";
import { VendorsPage } from "@modules/procurement/pages/VendorsPage";
import { TaxCodesPage } from "@modules/finance/pages/TaxCodesPage";
import { ChartOfAccountsPage } from "@modules/finance/pages/ChartOfAccountsPage";

export function App() {
  return (
    <Routes>
      <Route path="/" element={<Navigate to="/dashboard" replace />} />
      <Route path="/login" element={<LoginPage />} />
      <Route path="/register" element={<RegisterPage />} />

      <Route element={<ProtectedRoute />}>
        <Route path="/dashboard" element={<DashboardPage />} />
        <Route path="/profile" element={<ProfilePage />} />
        <Route path="/admin" element={<AdminDashboardPage />} />
        <Route path="/admin/users" element={<UsersPage />} />
        <Route path="/admin/roles" element={<RolesPage />} />
        <Route path="/admin/organization" element={<OrganizationSettingsPage />} />
        <Route path="/admin/audit-log" element={<AuditLogPage />} />
        <Route path="/sales/customers" element={<CustomersPage />} />
        <Route path="/procurement/vendors" element={<VendorsPage />} />
        <Route path="/finance/tax-codes" element={<TaxCodesPage />} />
        <Route path="/finance/accounts" element={<ChartOfAccountsPage />} />
      </Route>

      <Route path="*" element={<Navigate to="/dashboard" replace />} />
    </Routes>
  );
}
