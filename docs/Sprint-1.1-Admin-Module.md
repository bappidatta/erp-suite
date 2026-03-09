# Sprint 1.1: Admin Module - User & Role Management
**Duration:** 2 weeks (Week 3-4)  
**Start Date:** March 9, 2026  
**End Date:** March 22, 2026

---

## Sprint Objectives
- Implement complete user management CRUD
- Build role management and permission assignment
- Create organization settings entity
- Establish admin dashboard baseline
- Set up CRUD patterns for reuse in other modules

---

## Backlog & Tasks

### Backend: User Management (Medium Priority → Do First)
#### Task 1.1.1: Extend User Entity
- [ ] Add detailed fields to User (FirstName, LastName, Email, Phone, Status, Department, Manager)
- [ ] Add soft delete support (IsDeleted, DeletedAt)
- [ ] Add audit fields (CreatedBy, CreatedAt, ModifiedBy, ModifiedAt)
- [ ] Create User domain events (UserCreated, UserUpdated, UserDeleted)
- **File Location:** `backend/src/Modules/Admin/Domain/User.cs`

#### Task 1.1.2: Create User CRUD Endpoints
- [ ] GET /api/admin/users (list with pagination, search, filter)
- [ ] GET /api/admin/users/{id} (detail)
- [ ] POST /api/admin/users (create with validation)
- [ ] PUT /api/admin/users/{id} (update)
- [ ] DELETE /api/admin/users/{id} (soft delete)
- [ ] POST /api/admin/users/{id}/activate (status change)
- **File Location:** `backend/src/Modules/Admin/Presentation/UsersController.cs`

#### Task 1.1.3: User Validation & Business Rules
- [ ] Email uniqueness validation
- [ ] First/Last name required
- [ ] Role must exist before assignment
- [ ] Prevent deleting current logged-in user
- [ ] Department must exist
- **File Location:** `backend/src/Modules/Admin/Application/Users/UsersValidator.cs`

#### Task 1.1.4: User Query Service
- [ ] Search by name, email
- [ ] Filter by department, status, role
- [ ] Sort capability
- [ ] Pagination (10, 20, 50 per page)
- **File Location:** `backend/src/Modules/Admin/Application/Users/UserQueryService.cs`

---

### Backend: Role & Permission Management
#### Task 1.1.5: Role & Permission Entities
- [ ] Role entity (RoleId, Name, Description, IsActive, CreatedAt, ModifiedAt)
- [ ] Permission entity (PermissionId, Name, Module, Action, Description)
- [ ] RolePermission join entity
- [ ] UserRole join entity with CreatedAt, AssignedBy
- **File Location:** `backend/src/Modules/Admin/Domain/`

#### Task 1.1.6: Role CRUD Endpoints
- [ ] GET /api/admin/roles (list)
- [ ] GET /api/admin/roles/{id} (detail with permissions)
- [ ] POST /api/admin/roles (create)
- [ ] PUT /api/admin/roles/{id} (update)
- [ ] DELETE /api/admin/roles/{id} (soft delete, prevent if users assigned)
- [ ] POST /api/admin/roles/{id}/permissions (assign/update permissions)
- [ ] GET /api/admin/roles/{id}/permissions (get assigned permissions)
- **File Location:** `backend/src/Modules/Admin/Presentation/RolesController.cs`

#### Task 1.1.7: Permission Seeding
- [ ] Create seed job for default permissions
- [ ] Seed standard roles (Admin, Manager, User, Viewer)
- [ ] Document permission structure
- [ ] Create migration with permissions data
- **File Location:** `backend/src/Modules/Admin/Infrastructure/Seeding/`

---

### Backend: Organization Settings
#### Task 1.1.8: Organization Settings Entity
- [ ] CompanyName, LegalName, RegistrationNumber
- [ ] Address, Phone, Email, Website
- [ ] Logo/Banner image storage path
- [ ] Currency (base), Fiscal year start/end, DateFormat, TimeZone
- [ ] System status (Active, Suspended, Demo)
- [ ] Audit fields
- **File Location:** `backend/src/Modules/Admin/Domain/OrganizationSettings.cs`

#### Task 1.1.9: Organization Settings Endpoints
- [ ] GET /api/admin/organization (current settings)
- [ ] PUT /api/admin/organization (update - admin only)
- [ ] POST /api/admin/organization/logo (upload image)
- [ ] Validation: required fields, image size limit (5MB)
- **File Location:** `backend/src/Modules/Admin/Presentation/OrganizationController.cs`

---

### Backend: Admin Dashboard Data
#### Task 1.1.10: Dashboard Queries
- [ ] Total user count (active/inactive/deleted)
- [ ] Active role count
- [ ] Last activity timestamp (from audit logs)
- [ ] System health status
- [ ] Endpoints: GET /api/admin/dashboard/stats
- **File Location:** `backend/src/Modules/Admin/Application/Dashboard/DashboardQueryService.cs`

---

### Frontend: User Management UI
#### Task 1.1.11: User List Screen
- [ ] Table with columns: Name, Email, Department, Role, Status, Actions
- [ ] Pagination (10, 20, 50 per page)
- [ ] Search by name/email
- [ ] Filter by department, status, role
- [ ] Bulk status change checkbox
- [ ] Create User button (bottom-right)
- [ ] Edit/Delete icons per row
- **File Location:** `frontend/src/modules/admin/pages/UsersPage.tsx`

#### Task 1.1.12: User Create/Edit Form
- [ ] First Name (text, required)
- [ ] Last Name (text, required)
- [ ] Email (email, required, unique validation)
- [ ] Phone (text, optional)
- [ ] Department (dropdown, required)
- [ ] Manager (searchable dropdown, optional)
- [ ] Role (multi-select or single-select dropdown)
- [ ] Status (Active/Inactive radio)
- [ ] Submit & Cancel buttons
- [ ] Form validation with error messages
- [ ] Modal or slide-out dialog
- **File Location:** `frontend/src/modules/admin/components/UserForm.tsx`

#### Task 1.1.13: User Deletion Confirmation
- [ ] Confirmation modal for delete action
- [ ] Warning message if user has active approvals/documents
- [ ] Option to reassign documents to another user
- **File Location:** `frontend/src/modules/admin/components/DeleteUserModal.tsx`

---

### Frontend: Role Management UI
#### Task 1.1.14: Role List Screen
- [ ] Table with columns: Name, Description, User Count, Permissions Count, Status, Actions
- [ ] Create Role button
- [ ] Edit/Delete icons
- [ ] Quick view permissions (hover tooltip or click)
- **File Location:** `frontend/src/modules/admin/pages/RolesPage.tsx`

#### Task 1.1.15: Role Create/Edit Form
- [ ] Name (text, required, unique)
- [ ] Description (textarea, optional)
- [ ] Status (Active/Inactive)
- [ ] Permissions section:
  - [ ] Tree/expandable list by module
  - [ ] Checkboxes for each permission
  - [ ] Select All / Deselect All buttons per module
- [ ] Submit & Cancel
- **File Location:** `frontend/src/modules/admin/components/RoleForm.tsx`

#### Task 1.1.16: Permission Assignment to User
- [ ] User detail page section: "Roles"
- [ ] Show current roles with assignment date and assigned-by
- [ ] Add role button
- [ ] Remove role from modal
- [ ] List available roles with filter
- **File Location:** `frontend/src/modules/admin/components/UserRolesSection.tsx`

---

### Frontend: Organization Settings UI
#### Task 1.1.17: Organization Settings Screen
- [ ] Company name, legal name, registration number (text)
- [ ] Address fields (street, city, state, postal code, country)
- [ ] Phone, email, website (text)
- [ ] Logo upload area (drag & drop, file picker)
- [ ] Currency dropdown (USD, EUR, GBP, INR, etc.)
- [ ] Fiscal year start/end (date pickers)
- [ ] Date format, time zone (dropdowns)
- [ ] System status (dropdown, admin view only)
- [ ] Save button with success/error toast
- **File Location:** `frontend/src/modules/admin/pages/OrganizationSettingsPage.tsx`

---

### Frontend: Admin Dashboard
#### Task 1.1.18: Admin Dashboard Screen
- [ ] Welcome card with user name
- [ ] 4 stat cards:
  - [ ] Active Users count
  - [ ] Roles count
  - [ ] Pending Approvals (stub for now)
  - [ ] System Health
- [ ] Quick links:
  - [ ] Manage Users
  - [ ] Manage Roles
  - [ ] Organization Settings
  - [ ] Audit Log
- [ ] Recent activity list (last 10 actions)
- **File Location:** `frontend/src/modules/admin/pages/AdminDashboardPage.tsx`

---

### Backend: Audit & Logging
#### Task 1.1.19: Audit Log Entity & Service
- [ ] AuditLog entity (UserId, Action, Module, EntityId, OldValues, NewValues, Timestamp, IPAddress)
- [ ] Automatic capture on Create/Update/Delete via interceptor
- [ ] Endpoint: GET /api/admin/audit-logs (filterable by module, date range, user)
- **File Location:** `backend/src/Modules/Admin/Infrastructure/Audit/`

#### Task 1.1.20: Soft Delete Middleware
- [ ] All queries auto-filter soft-deleted records
- [ ] Admin can view deleted records (IsDeleted filter)
- [ ] Restore endpoint: PATCH /api/admin/{resource}/{id}/restore
- **File Location:** `backend/src/BuildingBlocks/Infrastructure/SoftDeleteQueryFilter.cs`

---

### Testing & Integration
#### Task 1.1.21: Backend API Integration Tests
- [ ] User CRUD operations
- [ ] Role permission assignments
- [ ] Permission validation
- [ ] Soft delete behavior
- [ ] Audit log capture
- [ ] Test file: `backend/tests/Integration/Admin.Tests/`

#### Task 1.1.22: Frontend Component Testing
- [ ] User form validation
- [ ] Table sorting/pagination
- [ ] Search/filter functionality
- [ ] Role assignment modal
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

| Risk | Impact | Mitigation |
|------|--------|------------|
| Scope creep (adding more admin features) | Medium | Stick to CRUD + dashboard, defer advanced features |
| Permission system too complex | Medium | Start with simple role-based, avoid attribute-based initially |
| Performance with large user sets | Low | Add pagination, indexing on Email |
| UI inconsistency across forms | Low | Use shared form components from shadcn |

---

## Dependencies & Blockers

- ✅ Authentication endpoints foundation (Phase 0.2) - COMPLETE
- ✅ Database migrations infrastructure - COMPLETE
- ✅ Object mapper / DTOs pattern - READY
- 🔄 Role-based authorization policy middleware - IN PROGRESS (Phase 0.3)

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
| Backend Lead | 1.1.1-1.1.10, 1.1.19-1.1.20 | Sprint Backlog |
| Frontend Lead | 1.1.11-1.1.18 | Sprint Backlog |
| QA | 1.1.21-1.1.22 | Test Plan |
