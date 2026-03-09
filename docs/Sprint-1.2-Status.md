# Sprint 1.2 Status - Master Data (Modular)

**Date:** June 2025
**Sprint:** Phase 1, Sprint 1.2
**Status:** ✅ Complete

---

## 🎯 Sprint Goals

Implement master data entities distributed across their owning modules following modular monolith architecture:
- **Customer** → Sales module
- **Vendor** → Procurement module
- **TaxCode + Account (Chart of Accounts)** → Finance module

Full-stack CRUD with search, filter, pagination, and frontend management pages.

---

## 📋 Tasks

### Backend: Sales Module (Customer)
- [x] Domain: `Customer` entity with `Create()`, `Update()`, `Activate()`, `Deactivate()` methods
- [x] Application: DTOs (`CreateCustomerRequest`, `UpdateCustomerRequest`, `CustomerResponse`, `GetCustomersQuery`)
- [x] Application: FluentValidation validators
- [x] Application: `ICustomerService` interface (7 methods)
- [x] Infrastructure: `CustomerService` with search/filter/sort/paginate, duplicate check, soft-delete
- [x] Infrastructure: `DependencyInjection.AddSalesInfrastructure()`
- [x] Controller: `CustomersController` at `api/sales/customers`

### Backend: Procurement Module (Vendor)
- [x] Domain: `Vendor` entity with banking details, lead time
- [x] Application: DTOs, validators, `IVendorService` interface
- [x] Infrastructure: `VendorService` with full CRUD
- [x] Infrastructure: `DependencyInjection.AddProcurementInfrastructure()`
- [x] Controller: `VendorsController` at `api/procurement/vendors`

### Backend: Finance Module (TaxCode + Account)
- [x] Domain: `TaxCode` entity with `TaxType` enum (Percentage, Fixed)
- [x] Domain: `Account` entity with `AccountType` enum, self-referencing hierarchy
- [x] Application: DTOs, validators, `ITaxCodeService`, `IAccountService` interfaces
- [x] Infrastructure: `TaxCodeService`, `AccountService` with tree building
- [x] Infrastructure: `DependencyInjection.AddFinanceInfrastructure()`
- [x] Controllers: `TaxCodesController` at `api/finance/tax-codes`, `AccountsController` at `api/finance/accounts`

### Backend: Integration
- [x] ErpDbContext updated with Sales/Procurement/Finance entity usings
- [x] Admin.Infrastructure.csproj references Sales.Domain, Procurement.Domain, Finance.Domain
- [x] Api.csproj references all 6 new projects (Application + Infrastructure per module)
- [x] ErpSuite.slnx updated with all module projects
- [x] Program.cs DI: `AddSalesInfrastructure()`, `AddProcurementInfrastructure()`, `AddFinanceInfrastructure()`
- [x] Validator registration for all module assemblies
- [x] Old MasterData module completely removed

### Frontend: Sales Module
- [x] Types: `Customer`, `CreateCustomerRequest`, `UpdateCustomerRequest`, `PagedResult<T>`
- [x] API: `salesApi.ts` (CRUD + activate/deactivate)
- [x] Page: `CustomersPage.tsx` with table, search, filter, pagination, create/edit/delete dialogs

### Frontend: Procurement Module
- [x] Types: `Vendor`, `CreateVendorRequest`, `UpdateVendorRequest`
- [x] API: `procurementApi.ts` (CRUD + activate/deactivate)
- [x] Page: `VendorsPage.tsx` with banking details fieldset

### Frontend: Finance Module
- [x] Types: `TaxCode`, `Account`, `AccountTreeNode`, request DTOs
- [x] API: `financeApi.ts` (TaxCode CRUD + Account CRUD + tree endpoint)
- [x] Page: `TaxCodesPage.tsx` with type/rate display
- [x] Page: `ChartOfAccountsPage.tsx` with tree view + list view tabs

### Frontend: Navigation
- [x] App.tsx: Routes for `/sales/customers`, `/procurement/vendors`, `/finance/tax-codes`, `/finance/accounts`
- [x] Sidebar.tsx: Sales, Procurement, Finance nav groups with submenu items

---

## 🏗️ Architecture Notes

- **Modular Monolith**: Each domain entity lives in its owning module (Sales, Procurement, Finance)
- **Shared DbContext**: All entities registered in `ErpDbContext` (Admin.Infrastructure) — single database
- **Clean Architecture per module**: Domain → Application → Infrastructure layers with separate .csproj files
- **API routes**: Module-scoped (`/api/sales/`, `/api/procurement/`, `/api/finance/`)
- **DB tables unchanged**: `customers`, `vendors`, `tax_codes`, `accounts` — same schema, just C# namespace reorganization

---

## ✅ Build Status

- Backend: **0 errors, 0 warnings** (18 projects)
- Frontend: **0 TypeScript errors**
- EF Migration: `Sprint1_2_MasterData` applied (table structure unchanged by restructuring)
