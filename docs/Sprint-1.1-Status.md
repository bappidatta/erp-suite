# Sprint 1.1 Status - Admin Module

**Date:** March 9, 2026  
**Sprint:** Phase 1, Sprint 1.1  
**Status:** 🟡 In Progress

---

## 🎯 Sprint Goals

Implement complete Admin module — the foundation for all other modules:
- User management CRUD with role assignment
- Role and permission management
- Organization settings
- Admin dashboard with system stats
- Audit log endpoint
- Unit tests for all backend services

---

## 📋 Tasks

### Backend: User Management
- [x] Extend User entity with Phone, DepartmentId, ManagerId, Status, LoginAttempts, LockedUntil
- [x] `UserStatus` enum: Active, Inactive, Locked, Suspended
- [x] Domain events: `UserCreatedEvent`, `UserUpdatedEvent`, `UserDeletedEvent`
- [x] Behavioral methods: `UpdateProfile()`, `ChangeStatus()`, `RecordFailedLogin()`, `RecordSuccessfulLogin()`, `Activate()`, `Deactivate()`, `Lock()`, `Suspend()`
- [x] Static factory: `User.Create(email, passwordHash, firstName, lastName, roleId, phone?, mustChangePassword?)`
- [ ] `IUserService` interface + `UserService` implementation
- [ ] DTOs: `CreateUserRequest`, `UpdateUserRequest`, `UserResponse`, `UserListResponse`, `PagedResult<T>`
- [ ] GET `/api/admin/users` (list with pagination, search, filter)
- [ ] GET `/api/admin/users/{id}`
- [ ] POST `/api/admin/users`
- [ ] PUT `/api/admin/users/{id}`
- [ ] DELETE `/api/admin/users/{id}` (soft delete)
- [ ] POST `/api/admin/users/{id}/activate`
- [ ] Email uniqueness validation
- [ ] Role existence check on assignment
- [ ] Prevent deleting current logged-in user
- [ ] Department existence check
- [ ] Search by name/email, filter by department/status/role, sort, paginate
- [ ] Unit tests for `UserService` and validators

### Backend: Role & Permission Management
- [ ] Extend `Role.cs` to inherit from `BaseAuditableEntity`
- [ ] `Permission` entity (Name, Module, Action, Description)
- [ ] `RolePermission` join entity
- [ ] `UserRole` join entity with AssignedAt, AssignedBy
- [ ] GET `/api/admin/roles` (list)
- [ ] GET `/api/admin/roles/{id}` (detail with permissions)
- [ ] POST `/api/admin/roles`
- [ ] PUT `/api/admin/roles/{id}`
- [ ] DELETE `/api/admin/roles/{id}` (soft delete, prevent if users assigned)
- [ ] POST `/api/admin/roles/{id}/permissions`
- [ ] GET `/api/admin/roles/{id}/permissions`
- [ ] Seed standard roles (Admin, Manager, User, Viewer) and default permissions
- [ ] Unit tests for `RoleService`

### Backend: Organization Settings
- [ ] `OrganizationSettings` entity (CompanyName, LegalName, RegistrationNumber, Address, Phone, Email, Website, Logo path, Currency, FiscalYear, DateFormat, TimeZone, Status)
- [ ] GET `/api/admin/organization`
- [ ] PUT `/api/admin/organization`
- [ ] POST `/api/admin/organization/logo` (5MB limit)
- [ ] Unit tests for settings service

### Backend: Admin Dashboard & Audit
- [ ] GET `/api/admin/dashboard/stats` (user count, role count, last activity, system health)
- [ ] `AuditLog` entity (UserId, Action, Module, EntityId, OldValues, NewValues, Timestamp, IPAddress)
- [ ] Automatic audit capture on Create/Update/Delete via interceptor
- [ ] GET `/api/admin/audit-logs` (filterable by module, date range, user)

### Frontend: User Management UI
- [ ] User list screen (table, pagination, search, filter, bulk status change)
- [ ] User create/edit form (fields: name, email, phone, department, manager, role, status)
- [ ] Delete confirmation modal (with reassign option)
- [ ] User detail — Roles section (add/remove roles with assignment metadata)

### Frontend: Role Management UI
- [ ] Role list screen (table: name, description, user count, permissions count, status)
- [ ] Role create/edit form with permissions tree (checkboxes per module, Select All)

### Frontend: Organization Settings UI
- [ ] Settings screen (company info, address, logo upload, currency, fiscal year, timezone)

### Frontend: Admin Dashboard
- [ ] Stat cards: Active Users, Roles, Pending Approvals (stub), System Health
- [ ] Quick links: Users, Roles, Organization Settings, Audit Log
- [ ] Recent activity list (last 10 actions)

### Testing
- [ ] Backend unit tests for all service classes (`UserService`, `RoleService`, settings service)
- [ ] Backend integration tests: User CRUD, role permission assignment, soft delete, audit capture
- [ ] Frontend component tests: form validation, table pagination, search/filter

---

## 🏗️ Architecture Decisions

### Domain Design
- **User entity** inherits from `BaseAuditableEntity` — automatic audit fields and soft delete built-in
- **`UserStatus` enum** instead of boolean `IsActive` — supports Active, Inactive, Locked, Suspended states
- **Account locking** — built into `User.RecordFailedLogin()`: locks for 15 minutes after 5 failed attempts
- **Domain events** — immutable record types (`UserCreatedEvent`, `UserUpdatedEvent`, `UserDeletedEvent`) dispatched after unit-of-work commit

### Service Layer
- **Unit test requirement** — all backend services must have accompanying unit tests (target 90%+ coverage for business logic)
- **`IUserService`** interface for testability and DI
- **`PagedResult<T>`** generic wrapper for all list responses

### Security
- Soft delete enforced by `BaseDbContext` global query filters — deleted records invisible by default
- Prevent self-deletion enforced in service layer, not just controller
- Role existence validated before assignment

---

## 📊 Progress

**Overall:** 15%  
**Backend:** 20% (entity done, CRUD not started)  
**Frontend:** 0%  
**Testing:** 0%

---

## Notes

- Task 1.1.1 (User entity extension) complete at commit `1647c3c`
- `Role.cs` currently inherits `BaseEntity` — needs upgrade to `BaseAuditableEntity` in Task 1.1.5
- Soft delete query filters already exist in `BaseDbContext` — no additional middleware needed
- `MustChangePassword` flag exists on `User` entity from Sprint 0.3; enforcement UI deferred

---

**Sprint Start:** March 9, 2026  
**Est. Completion:** March 22, 2026  
**Repository:** https://github.com/bappidatta/erp-suite
