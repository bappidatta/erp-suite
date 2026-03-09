# Sprint 1.1: Admin Module - User & Role Management

**Duration:** 2 weeks (Week 3-4)  
**Start Date:** March 9, 2026  
**End Date:** March 22, 2026  
**Status:** 🟢 IN PROGRESS  
**Velocity:** 20 points (estimated)

---

## Sprint Progress

| Metric | Value |
|--------|-------|
| **Total Points** | 20 |
| **Completed** | 3 (15%) |
| **In Progress** | 0 |
| **Not Started** | 17 (85%) |
| **Est. Completion** | March 18, 2026 |

---

## Sprint Objectives
- Implement complete user management CRUD
- Build role management and permission assignment
- Create organization settings entity
- Establish admin dashboard baseline
- Set up CRUD patterns for reuse in other modules

---

## Key Decisions

1. **User Entity Design**
   - Inherited from `BaseAuditableEntity` for automatic audit fields and soft delete
   - `UserStatus` enum instead of boolean `IsActive` — more flexible for future states
   - Built-in account locking after 5 failed login attempts (15-minute lockout)
   - Supports department & manager hierarchy via nullable foreign keys

2. **Domain Events**
   - Events are immutable record types inheriting `DomainEvent`
   - Dispatched via repository after unit-of-work commit
   - Enables event-driven communication with other modules

3. **Unit Tests**
   - All backend services must have accompanying unit tests
   - Target 90%+ coverage for business logic methods

---

## Backlog & Tasks

### Backend: User Management

#### ✅ Task 1.1.1: Extend User Entity — COMPLETE
**Points:** 3 | **Commit:** `1647c3c` | **Completed:** March 9, 2026

- [x] Add detailed fields: Phone, DepartmentId, ManagerId, Status, LoginAttempts, LockedUntil
- [x] Soft delete and audit fields inherited from `BaseAuditableEntity`
- [x] `UserStatus` enum: Active, Inactive, Locked, Suspended
- [x] Domain events: `UserCreatedEvent`, `UserUpdatedEvent`, `UserDeletedEvent`
- [x] Behavioral methods: `UpdateProfile()`, `ChangeStatus()`, `RecordFailedLogin()`, `RecordSuccessfulLogin()`, `Activate()`, `Deactivate()`, `Lock()`, `Suspend()`
- [x] Static factory: `User.Create(email, passwordHash, firstName, lastName, roleId, phone?, mustChangePassword?)`

**Files Created:**
- `backend/src/Modules/Admin/Domain/.../Entities/UserStatus.cs`
- `backend/src/Modules/Admin/Domain/.../Events/UserCreatedEvent.cs`
- `backend/src/Modules/Admin/Domain/.../Events/UserUpdatedEvent.cs`
- `backend/src/Modules/Admin/Domain/.../Events/UserDeletedEvent.cs`

**Files Modified:**
- `backend/src/Modules/Admin/Domain/.../Entities/User.cs`

---

#### 🟡 Task 1.1.2: Create User CRUD Endpoints — NEXT
**Points:** 5 | **Status:** Ready to Start | **Prerequisites:** ✅ Task 1.1.1

- [ ] `IUserService` interface + `UserService` implementation
- [ ] DTOs: `CreateUserRequest`, `UpdateUserRequest`, `UserResponse`, `UserListResponse`, `PagedResult<T>`
- [ ] GET `/api/admin/users` (list with pagination, search, filter)
- [ ] GET `/api/admin/users/{id}` (detail)
- [ ] POST `/api/admin/users` (create with validation)
- [ ] PUT `/api/admin/users/{id}` (update)
- [ ] DELETE `/api/admin/users/{id}` (soft delete)
- [ ] POST `/api/admin/users/{id}/activate` (status change)
- [ ] **Unit tests** for `UserService` (create, duplicate email, self-delete guard, account locking)
- **File Location:** `backend/src/Modules/Admin/Application/Users/`, `backend/src/Modules/Admin/Presentation/UsersController.cs`

---

#### 🔴 Task 1.1.3: User Validation & Business Rules — BLOCKED (on 1.1.2)
**Points:** 3

- [ ] Email uniqueness validation
- [ ] First/Last name required
- [ ] Role must exist before assignment
- [ ] Prevent deleting current logged-in user
- [ ] Department must exist
- [ ] **Unit tests** for all validators
- **File Location:** `backend/src/Modules/Admin/Application/Users/UsersValidator.cs`

---

#### 🔴 Task 1.1.4: User Query Service
**Points:** 3

- [ ] Search by name, email
- [ ] Filter by department, status, role
- [ ] Sort capability
- [ ] Pagination (10, 20, 50 per page)
- [ ] **Unit tests** for query/filter logic
- **File Location:** `backend/src/Modules/Admin/Application/Users/UserQueryService.cs`


---

### Backend: Role & Permission Management

#### 🔴 Task 1.1.5: Role & Permission Entities
**Points:** 5

- [ ] Extend `Role.cs` to inherit from `BaseAuditableEntity` (currently `BaseEntity`)
- [ ] `Permission` entity (PermissionId, Name, Module, Action, Description)
- [ ] `RolePermission` join entity
- [ ] `UserRole` join entity with AssignedAt, AssignedBy
- **File Location:** `backend/src/Modules/Admin/Domain/`

#### 🔴 Task 1.1.6: Role CRUD Endpoints
**Points:** 5

- [ ] GET `/api/admin/roles` (list)
- [ ] GET `/api/admin/roles/{id}` (detail with permissions)
- [ ] POST `/api/admin/roles` (create)
- [ ] PUT `/api/admin/roles/{id}` (update)
- [ ] DELETE `/api/admin/roles/{id}` (soft delete, prevent if users assigned)
- [ ] POST `/api/admin/roles/{id}/permissions` (assign/update permissions)
- [ ] GET `/api/admin/roles/{id}/permissions` (get assigned permissions)
- [ ] **Unit tests** for `RoleService`
- **File Location:** `backend/src/Modules/Admin/Presentation/RolesController.cs`

#### 🔴 Task 1.1.7: Permission Seeding
**Points:** 2

- [ ] Create seed job for default permissions
- [ ] Seed standard roles (Admin, Manager, User, Viewer)
- [ ] Document permission structure
- [ ] Create migration with permissions data
- **File Location:** `backend/src/Modules/Admin/Infrastructure/Seeding/`

---

### Backend: Organization Settings

#### 🔴 Task 1.1.8: Organization Settings Entity
**Points:** 2

- [ ] CompanyName, LegalName, RegistrationNumber
- [ ] Address, Phone, Email, Website
- [ ] Logo/Banner image storage path
- [ ] Currency (base), Fiscal year start/end, DateFormat, TimeZone
- [ ] System status (Active, Suspended, Demo)
- [ ] Audit fields
- **File Location:** `backend/src/Modules/Admin/Domain/OrganizationSettings.cs`

#### 🔴 Task 1.1.9: Organization Settings Endpoints
**Points:** 2

- [ ] GET `/api/admin/organization` (current settings)
- [ ] PUT `/api/admin/organization` (update — admin only)
- [ ] POST `/api/admin/organization/logo` (upload image, 5MB limit)
- [ ] **Unit tests** for settings service
- **File Location:** `backend/src/Modules/Admin/Presentation/OrganizationController.cs`

---

### Backend: Admin Dashboard Data

#### 🔴 Task 1.1.10: Dashboard Queries
**Points:** 2

- [ ] Total user count (active/inactive/deleted)
- [ ] Active role count
- [ ] Last activity timestamp (from audit logs)
- [ ] System health status
- [ ] GET `/api/admin/dashboard/stats`
- **File Location:** `backend/src/Modules/Admin/Application/Dashboard/DashboardQueryService.cs`

---

### Frontend: User Management UI

#### 🔴 Task 1.1.11: User List Screen
**Points:** 3

- [ ] Table: Name, Email, Department, Role, Status, Actions
- [ ] Pagination (10, 20, 50 per page)
- [ ] Search by name/email
- [ ] Filter by department, status, role
- [ ] Bulk status change checkbox
- [ ] Create User button, Edit/Delete icons per row
- **File Location:** `frontend/src/modules/admin/pages/UsersPage.tsx`

#### 🔴 Task 1.1.12: User Create/Edit Form
**Points:** 3

- [ ] First Name, Last Name (required), Email (required, unique)
- [ ] Phone (optional), Department (dropdown), Manager (searchable)
- [ ] Role (dropdown), Status (Active/Inactive)
- [ ] Form validation with error messages
- [ ] Modal or slide-out dialog
- **File Location:** `frontend/src/modules/admin/components/UserForm.tsx`

#### 🔴 Task 1.1.13: User Deletion Confirmation
**Points:** 1

- [ ] Confirmation modal for delete action
- [ ] Warning if user has active approvals/documents
- [ ] Option to reassign documents to another user
- **File Location:** `frontend/src/modules/admin/components/DeleteUserModal.tsx`

---

### Frontend: Role Management UI

#### 🔴 Task 1.1.14: Role List Screen
**Points:** 2

- [ ] Table: Name, Description, User Count, Permissions Count, Status, Actions
- [ ] Create Role button, Edit/Delete icons, quick view permissions
- **File Location:** `frontend/src/modules/admin/pages/RolesPage.tsx`

#### 🔴 Task 1.1.15: Role Create/Edit Form
**Points:** 2

- [ ] Name (required, unique), Description, Status
- [ ] Permissions: tree by module, checkboxes, Select All per module
- **File Location:** `frontend/src/modules/admin/components/RoleForm.tsx`

#### 🔴 Task 1.1.16: Permission Assignment to User
**Points:** 1

- [ ] User detail page "Roles" section
- [ ] Show current roles with assignment date/by, Add/Remove role
- **File Location:** `frontend/src/modules/admin/components/UserRolesSection.tsx`

---

### Frontend: Organization Settings UI

#### 🔴 Task 1.1.17: Organization Settings Screen
**Points:** 2

- [ ] Company name, legal name, registration number, address fields
- [ ] Logo upload area (drag & drop, file picker)
- [ ] Currency, fiscal year, date format, time zone dropdowns
- [ ] Save button with success/error toast
- **File Location:** `frontend/src/modules/admin/pages/OrganizationSettingsPage.tsx`

---

### Frontend: Admin Dashboard

#### 🔴 Task 1.1.18: Admin Dashboard Screen
**Points:** 2

- [ ] Welcome card, 4 stat cards (Active Users, Roles, Pending Approvals, System Health)
- [ ] Quick links: Users, Roles, Organization Settings, Audit Log
- [ ] Recent activity list (last 10 actions)
- **File Location:** `frontend/src/modules/admin/pages/AdminDashboardPage.tsx`

---

### Backend: Audit & Logging

#### 🔴 Task 1.1.19: Audit Log Entity & Service
**Points:** 3

- [ ] `AuditLog` entity (UserId, Action, Module, EntityId, OldValues, NewValues, Timestamp, IPAddress)
- [ ] Automatic capture on Create/Update/Delete via interceptor
- [ ] GET `/api/admin/audit-logs` (filterable by module, date range, user)
- **File Location:** `backend/src/Modules/Admin/Infrastructure/Audit/`

#### 🔴 Task 1.1.20: Soft Delete Middleware
**Points:** 1

- [ ] All queries auto-filter soft-deleted records (via DbContext query filters)
- [ ] Admin can view deleted records via flag
- [ ] Restore endpoint: PATCH `/api/admin/{resource}/{id}/restore`
- **File Location:** `backend/src/BuildingBlocks/Infrastructure/SoftDeleteQueryFilter.cs`

---

### Testing & Integration

#### 🔴 Task 1.1.21: Backend Unit & Integration Tests
**Points:** 3

- [ ] User CRUD operations (unit tests for service layer)
- [ ] Role permission assignment tests
- [ ] Soft delete behavior tests
- [ ] Audit log capture tests
- [ ] Test project: `backend/tests/Unit/Admin.Tests/` and `backend/tests/Integration/Admin.Tests/`

#### 🔴 Task 1.1.22: Frontend Component Testing
**Points:** 1

- [ ] User form validation tests
- [ ] Table sorting/pagination tests
- [ ] Search/filter functionality tests
- [ ] Test file: `frontend/src/modules/admin/__tests__/`

---

## Acceptance Criteria

✅ **User Management**
- Create, read, update, delete users
- Search and filter functionality works
- Email uniqueness enforced
- Cannot delete current logged-in user
- Soft delete works (IsDeleted flag)

✅ **Role Management**
- Create, read, update, delete roles
- Assign/remove permissions from roles
- Assign/remove roles from users
- Cannot delete role if users are assigned

✅ **Organization Settings**
- Update company info
- Upload logo
- Set fiscal year, currency, timezone
- Settings persist

✅ **Admin Dashboard**
- Displays stat cards with accurate counts
- Quick links navigate correctly
- Recent activity populated

✅ **API Contracts**
- All endpoints return consistent response structure
- Error handling with 400/404/500 status codes
- Pagination works on list endpoints
- Authentication enforced

✅ **UI/UX**
- Forms have validation feedback
- Lists have loading and error states
- Confirmation modals for destructive actions
- Responsive on desktop and tablet

---

## Definition of Done

For each task:
- [ ] Code written and tested
- [ ] Unit tests added (90%+ coverage for business logic)
- [ ] Integration tests passed
- [ ] Code reviewed and approved
- [ ] Merged to main branch
- [ ] Documented (inline comments, if needed)
- [ ] No console warnings/errors

---

## Risks & Mitigation

| Risk | Impact | Likelihood | Mitigation |
|------|--------|------------|-----------|
| Scope creep (adding more admin features) | Medium | Low | Stick to CRUD + dashboard, defer advanced features |
| Permission system too complex | Medium | Low | Start with simple role-based, avoid attribute-based initially |
| Performance with large user sets | Low | Low | Add pagination, indexing on Email |
| UI inconsistency across forms | Low | Low | Use shared form components from shadcn |
| Soft delete query filtering issues | Medium | Low | Use DbContext query filters — already in BaseDbContext |
| Account locking edge cases | Low | Low | Covered by unit tests for `RecordFailedLogin()` |

---

## Dependencies & Blockers

- ✅ Authentication endpoints foundation (Phase 0.2) — COMPLETE
- ✅ Database migrations infrastructure — COMPLETE
- ✅ Object mapper / DTOs pattern — READY
- ✅ Role-based authorization policy middleware — COMPLETE (Phase 0.3)

---

## Success Metrics

By end of sprint:
- All user management operations working end-to-end
- Admin can manage roles and permissions
- Audit trail captures all admin actions
- Dashboard loads in < 1s
- All acceptance criteria met
- Zero critical bugs in main branch

---

## Team Assignments (Recommended)

| Role | Tasks | Tracker |
|------|-------|---------|
| Backend Lead | 1.1.1–1.1.10, 1.1.19–1.1.20 | Sprint Backlog |
| Frontend Lead | 1.1.11–1.1.18 | Sprint Backlog |
| QA | 1.1.21–1.1.22 | Test Plan |
