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
import { NumberSequencesPage } from "@modules/admin/pages/NumberSequencesPage";
import { CustomersPage } from "@modules/sales/pages/CustomersPage";
import { VendorsPage } from "@modules/procurement/pages/VendorsPage";
import { TaxCodesPage } from "@modules/finance/pages/TaxCodesPage";
import { ChartOfAccountsPage } from "@modules/finance/pages/ChartOfAccountsPage";
import { JournalEntriesPage } from "@modules/finance/pages/JournalEntriesPage";
import { FinancialPeriodsPage } from "@modules/finance/pages/FinancialPeriodsPage";
import { TrialBalancePage } from "@modules/finance/pages/TrialBalancePage";
import { LedgerInquiryPage } from "@modules/finance/pages/LedgerInquiryPage";
import { CategoriesPage } from "@modules/inventory/pages/CategoriesPage";
import { UomsPage } from "@modules/inventory/pages/UomsPage";
import { WarehousesPage } from "@modules/inventory/pages/WarehousesPage";
import { ItemsPage } from "@modules/inventory/pages/ItemsPage";
import { DepartmentsPage } from "@modules/hr/pages/DepartmentsPage";
import { EmployeesPage } from "@modules/hr/pages/EmployeesPage";

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
        <Route path="/admin/number-sequences" element={<NumberSequencesPage />} />
        <Route path="/sales/customers" element={<CustomersPage />} />
        <Route path="/procurement/vendors" element={<VendorsPage />} />
        <Route path="/finance/tax-codes" element={<TaxCodesPage />} />
        <Route path="/finance/accounts" element={<ChartOfAccountsPage />} />
        <Route path="/finance/journal-entries" element={<JournalEntriesPage />} />
        <Route path="/finance/financial-periods" element={<FinancialPeriodsPage />} />
        <Route path="/finance/trial-balance" element={<TrialBalancePage />} />
        <Route path="/finance/ledger" element={<LedgerInquiryPage />} />
        <Route path="/inventory/categories" element={<CategoriesPage />} />
        <Route path="/inventory/uoms" element={<UomsPage />} />
        <Route path="/inventory/warehouses" element={<WarehousesPage />} />
        <Route path="/inventory/items" element={<ItemsPage />} />
        <Route path="/hr/departments" element={<DepartmentsPage />} />
        <Route path="/hr/employees" element={<EmployeesPage />} />
      </Route>

      <Route path="*" element={<Navigate to="/dashboard" replace />} />
    </Routes>
  );
}
