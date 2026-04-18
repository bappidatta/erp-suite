# ERP Suite MVP - Implementation Plan

**Date:** March 8, 2026  
**Version:** 1.1  
**Duration:** 16-20 weeks (4-5 months)

---

## Executive Summary
This plan breaks down the ERP MVP into 4 phases across approximately 16-20 weeks. Each phase has clear deliverables, success criteria, and dependencies.

**Progress Legend:**
- `[x]` Completed
- `[-]` In Progress
- `[ ]` Not Started

---

## Phase Overview

| Phase | Duration | Focus | Deliverables |
|-------|----------|-------|--------------|
| **Phase 0: Foundation Setup** | 2 weeks | Dev environment, CI/CD, shared kernel | Runnable backend + frontend, DB setup, auth baseline |
| **Phase 1: Core Platform & Masters** | 4-5 weeks | Admin, identity, master data | User management, all master entities, basic CRUD |
| **Phase 2: Transactional Modules** | 6-8 weeks | Finance, Sales, Procurement, Inventory | Core business flows operational |
| **Phase 3: HR, Reporting & Hardening** | 4-5 weeks | HR module, dashboards, UAT prep | Complete MVP with reports, UAT-ready |

---

## Phase 0: Foundation Setup (Week 1-2) ✅

### Objectives
- Establish development environment
- Set up CI/CD pipeline
- Create shared building blocks
- Enable team to start parallel module work

### Sprint 0.1: Local Development Environment (Week 1)

**Backend Tasks:**
- [x] Create .NET solution file with module structure
- [x] Set up EF Core with PostgreSQL provider
- [x] Add migration infrastructure
- [x] Create BuildingBlocks projects:
  - [x] Domain (base entities, value objects, domain events)
  - [x] Application (result types, validation, contracts)
  - [x] Infrastructure (EF Core base repository, audit interceptor)
  - [x] Presentation (base controllers, filters, middleware)
- [x] Configure dependency injection container structure
- [x] Add logging (Serilog with structured logging)
- [x] Create first migration with audit columns standard

**Frontend Tasks:**
- [x] Run `npm install` in frontend directory
- [x] Initialize shadcn/ui: `npx shadcn@latest init`
- [x] Set up essential shadcn components (Button, Input, Table, Dialog, Form)
- [x] Create API client base (`src/shared/api/client.ts`)
- [x] Set up React Query or SWR for data fetching
- [x] Create auth context and protected route wrapper
- [x] Add basic layout shell (header, sidebar, content)

**Infrastructure Tasks:**
- [x] Create docker-compose.yml for PostgreSQL
- [ ] Add pgAdmin service (optional)
- [x] Create database initialization script
- [x] Set up local environment variables template (.env.example)

**Deliverables:**
- Backend API runs on https://localhost:5001
- Frontend runs on http://localhost:5173
- PostgreSQL accessible on localhost:5432
- Developers can create and run migrations
- Basic health check endpoint works

### Sprint 0.2: Authentication & Authorization Foundation (Week 2)

**Backend Tasks:**
- [x] Implement ASP.NET Core Identity or JWT-based auth
- [x] Create User, Role, Permission entities in Admin module
- [x] Build authentication endpoints (login, logout, refresh token)
- [x] Implement role-based authorization policies
- [ ] Add tenant/company context middleware
- [x] Create seed data for initial admin user

**Frontend Tasks:**
- [x] Build login page
- [x] Implement auth token storage (localStorage/sessionStorage)
- [x] Create auth guards for protected routes
- [x] Add logout functionality
- [x] Build basic user profile dropdown
- [x] Handle 401/403 responses globally

**Deliverables:**
- Login works end-to-end
- Protected routes enforce authentication
- Admin role can access all modules
- Token refresh mechanism works

### Sprint 0.3: Auth Hardening & Gap Closure (Extension)

**Backend Tasks:**
- [x] Add explicit logout endpoint and contract
- [x] Introduce policy-based authorization definitions
- [ ] Add tenant/company context middleware baseline
- [x] Keep admin authorization policy-driven (not only attribute-role literals)

**Frontend Tasks:**
- [x] Add global 401/403 handling path for API responses
- [x] Ensure logout API call and local session cleanup are both handled
- [x] Resolve broken profile navigation path

**Deliverables:**
- Logout endpoint available and callable
- Global 401/403 handling in place
- Tenant context attached per request
- Profile route works from header menu

---

## Phase 1: Core Platform & Master Data (Week 3-7) ✅

### Objectives
- Complete Admin module
- Build all master data entities
- Establish CRUD patterns for all modules to follow

### Sprint 1.1: Admin Module (Week 3-4)

**Backend Tasks:**
- [x] User management CRUD
- [x] Role management CRUD
- [x] Permission assignment to roles
- [x] Organization/Company settings entity
- [x] Fiscal year configuration
- [x] Currency and tax defaults
- [ ] Number sequence configuration for documents
- [x] Audit log query endpoints

**Frontend Tasks:**
- [x] User list, create, edit, delete screens
- [x] Role list and assignment screen
- [x] Organization settings screen
- [ ] Fiscal year configuration screen (deferred to Phase 2)
- [x] Admin dashboard with system stats

**Deliverables:**
- [x] Complete user and role management
- [x] System administrator can configure organization settings
- [x] Audit trail visible for admin actions

### Sprint 1.2: Master Data Module - Part 1 (Week 5)

**Backend Tasks:**
- [x] Customer master (with credit limit, tax info, addresses)
- [x] Vendor master (with payment terms, bank details)
- [x] Tax master (tax codes, rates, applicability)
- [x] Chart of Accounts (hierarchy, account types)
- [x] Validation rules and duplicate prevention

**Frontend Tasks:**
- [x] Customer list, create, edit screens with search/filter
- [x] Vendor list, create, edit screens
- [x] Tax configuration screen
- [x] Chart of Accounts tree view and management
- [ ] CSV import UI for customers and vendors (deferred)

**Deliverables:**
- [x] Customer and vendor masters fully functional
- [x] Tax codes can be configured
- [x] Chart of accounts set up and ready

### Sprint 1.3: Master Data Module - Part 2 (Week 6-7)

**Backend Tasks:**
- [x] Item master (SKU, UOM, category, valuation method)
- [x] Warehouse and location master
- [x] Employee master (basic profile, department, status)
- [x] Department master
- [x] UOM master
- [x] Category master
- [ ] Master data import/export endpoints (CSV) (deferred)

**Frontend Tasks:**
- [x] Item master list, create, edit screens with UOM and category dropdowns
- [x] Warehouse and location management screens
- [x] Employee list and profile screens with department dropdown
- [x] Department and category management
- [ ] CSV import screens with validation feedback (deferred)
- [ ] Master data dashboard showing record counts (deferred)

**Deliverables:**
- [x] All master data entities operational
- [ ] CSV import/export working for key entities (deferred)
- [x] Data validation prevents duplicates and invalid entries

---

## Phase 2: Transactional Modules (Week 8-15)

### Objectives
- Implement core business flows
- Enable Procure-to-Pay, Order-to-Cash, Record-to-Report cycles
- Build approval workflows

### Sprint 2.1: Finance Module - GL Foundation (Week 8-9)

**Backend Tasks:**
- [ ] Journal entry entity and posting logic
- [ ] Financial posting service (from other modules)
- [ ] Period lock mechanism
- [ ] Trial balance calculation
- [ ] Ledger query service
- [ ] GL posting validation rules

**Frontend Tasks:**
- [ ] Manual journal entry screen
- [ ] Trial balance report screen
- [ ] Ledger inquiry screen with drill-down
- [ ] Period close checklist screen
- [ ] GL dashboard with account balances

**Deliverables:**
- Manual journal entries can be created and posted
- Trial balance generates correctly
- Period close prevents backdated postings

### Sprint 2.2: Finance Module - AP & AR (Week 10-11)

**Backend Tasks:**
- [ ] Vendor invoice entity and posting
- [ ] Payment voucher and allocation logic
- [ ] Customer invoice entity and posting
- [ ] Receipt entry and allocation
- [ ] AP aging calculation
- [ ] AR aging calculation
- [ ] Credit limit check service

**Frontend Tasks:**
- [ ] Vendor invoice entry screen
- [ ] Payment voucher screen with invoice allocation
- [ ] AP aging report
- [ ] Customer invoice screen (manual and from SO)
- [ ] Receipt entry screen with allocation
- [ ] AR aging report
- [ ] Customer account statement

**Deliverables:**
- AP cycle works: invoice → payment → posting
- AR cycle works: invoice → receipt → allocation
- Aging reports accurate

### Sprint 2.3: Sales Module (Week 12-13)

**Backend Tasks:**
- [ ] Quotation entity and approval workflow
- [ ] Sales order entity with credit limit check
- [ ] Price list and discount engine
- [ ] Delivery note entity
- [ ] Sales order to invoice trigger
- [ ] Sales return entity
- [ ] Order status tracking and lifecycle

**Frontend Tasks:**
- [ ] Quotation create, edit, submit, approve screens
- [ ] Sales order create and management screens
- [ ] Price list configuration
- [ ] Delivery note screen
- [ ] Sales invoice screen (integration with AR)
- [ ] Sales return screen
- [ ] Sales pipeline dashboard
- [ ] Order fulfillment tracking

**Deliverables:**
- Quotation → Sales Order → Delivery → Invoice flow works
- Credit limit enforced
- Sales returns processed correctly

### Sprint 2.4: Procurement Module (Week 13-14)

**Backend Tasks:**
- [ ] Purchase requisition entity and approval
- [ ] Purchase order entity and approval
- [ ] Goods receipt note (GRN) entity
- [ ] PO to GRN matching
- [ ] Purchase return entity
- [ ] Vendor lead time tracking

**Frontend Tasks:**
- [ ] Purchase requisition screen
- [ ] Purchase order screen with approval workflow
- [ ] GRN entry screen with PO reference
- [ ] Purchase return screen
- [ ] Open PO report
- [ ] Overdue delivery report
- [ ] Procurement dashboard

**Deliverables:**
- Requisition → PO → GRN → Invoice matching works
- Approval workflows functional
- Purchase returns handled

### Sprint 2.5: Inventory Module (Week 14-15)

**Backend Tasks:**
- [ ] Stock movement entity (receipt, issue, transfer, adjustment)
- [ ] Stock balance calculation (moving average)
- [ ] Batch tracking (basic)
- [ ] Reorder level alerts
- [ ] Cycle count entity and variance adjustment
- [ ] Inventory ledger service
- [ ] Negative stock prevention rules

**Frontend Tasks:**
- [ ] Stock receipt screen (from GRN)
- [ ] Stock issue screen
- [ ] Stock transfer screen (warehouse to warehouse)
- [ ] Stock adjustment screen
- [ ] Cycle count entry screen
- [ ] Current stock report
- [ ] Stock movement report
- [ ] Low stock alerts dashboard
- [ ] Inventory valuation report

**Deliverables:**
- Stock movements tracked accurately
- Moving average cost calculated correctly
- Multi-warehouse stock visibility works
- Reorder alerts functional

---

## Phase 3: HR, Reporting & Hardening (Week 16-20)

### Objectives
- Complete HR baseline
- Build operational dashboards
- Prepare for UAT
- Fix critical bugs

### Sprint 3.1: HR Module (Week 16-17)

**Backend Tasks:**
- [x] Employee detailed profile
- [ ] Attendance entity and bulk import
- [ ] Leave request entity and approval
- [ ] Leave balance calculation
- [ ] Shift template configuration
- [ ] Employee status transitions

**Frontend Tasks:**
- [x] Employee profile screen
- [ ] Attendance capture and CSV import
- [ ] Leave request and approval screens
- [ ] Leave balance view
- [ ] Shift management screen
- [ ] Attendance summary report
- [ ] Department headcount dashboard

**Deliverables:**
- Employee records managed
- Attendance captured and reported
- Leave approval workflow works

### Sprint 3.2: Reporting & Dashboards (Week 17-18)

**Backend Tasks:**
- [ ] Dashboard data aggregation services for each module
- [ ] Report export service (CSV, PDF)
- [ ] Saved report views per user
- [ ] Financial reports: Trial Balance, P&L skeleton, Balance Sheet skeleton
- [ ] Operational reports: Sales by customer/item, Purchase by vendor, Stock aging

**Frontend Tasks:**
- [ ] Dashboard widgets (cards, charts, tables)
- [ ] Finance dashboard
- [ ] Sales dashboard
- [ ] Procurement dashboard
- [ ] Inventory dashboard
- [ ] HR dashboard
- [ ] Report viewer with filters (date range, company, warehouse, etc.)
- [ ] Export buttons (CSV/PDF)

**Deliverables:**
- Role-specific dashboards live
- All MVP reports available
- Export functionality works

### Sprint 3.3: Approval Workflows & Notifications (Week 18)

**Backend Tasks:**
- [ ] Workflow configuration service
- [ ] Approval queue service
- [ ] Email notification service
- [ ] In-app notification entity
- [ ] Escalation timer service (background job)

**Frontend Tasks:**
- [ ] Approval inbox screen
- [ ] Workflow configuration screen (admin)
- [ ] In-app notification center
- [ ] Approve/reject modal with comments
- [ ] SLA breach report

**Deliverables:**
- Approvals work across all transactional modules
- Email notifications sent for pending approvals
- Escalation alerts functional

### Sprint 3.4: UAT Preparation & Bug Fixes (Week 19-20)

**Tasks:**
- [ ] End-to-end flow testing:
  - [ ] Procure-to-Pay
  - [ ] Order-to-Cash
  - [ ] Record-to-Report
- [ ] Create UAT test cases and scripts
- [ ] Data migration dry run
- [ ] Performance testing (concurrent users, large datasets)
- [ ] Security audit (RBAC enforcement, SQL injection, XSS checks)
- [ ] Bug triage and critical fix sprints
- [ ] User training material preparation
- [ ] Deployment runbook

**Deliverables:**
- UAT sign-off from business stakeholders
- Performance meets SLA (3s page load, 300 concurrent users)
- Security vulnerabilities addressed
- Production deployment checklist ready

---

## Phase 4: Go-Live Preparation (Week 20+)

### Pre-Go-Live Checklist
- [ ] Production environment provisioned
- [ ] Database backup/restore tested
- [ ] Migration scripts validated with production-like data volume
- [ ] Monitoring and alerting configured
- [ ] User roles and permissions configured in production
- [ ] Opening balances loaded (GL, Customers, Vendors, Inventory)
- [ ] Cutover plan reviewed and approved
- [ ] Rollback plan documented

### Go-Live Activities
- [ ] Execute cutover (freeze legacy systems, migrate data)
- [ ] Smoke test all critical flows
- [ ] User acceptance final verification
- [ ] Hypercare support (first 2 weeks)

---

## Success Metrics (Post Go-Live)
Track these in first 90 days:
- 95%+ of transactions created in ERP (not external tools)
- Month-end close cycle reduced by 30%
- Inventory variance < 2%
- 80%+ of approvals completed within SLA
- System uptime ≥ 99.5%

---

## Risk Register

| Risk | Impact | Mitigation |
|------|--------|------------|
| Data migration quality issues | High | Multiple dry runs, reconciliation checkpoints |
| Scope creep | High | Strict change control board, defer non-MVP to backlog |
| Key resource unavailability | Medium | Cross-train team, document decisions |
| Integration delays | Medium | Keep MVP dependencies minimal, mock external systems |
| User resistance | Medium | Early stakeholder engagement, training, feedback loops |
| Performance bottlenecks | Medium | Load testing in week 19, optimize hot paths |

---

## Team Structure Recommendation

- **Backend Team (2-3 developers):** Module implementation, API contracts
- **Frontend Team (2-3 developers):** UI screens, forms, dashboards
- **DevOps (1):** CI/CD, infrastructure, database management
- **QA (1-2):** Test automation, UAT coordination
- **Product Owner (1):** Requirements clarification, prioritization, UAT sign-off
- **Tech Lead/Architect (1):** Code reviews, architecture compliance, technical decisions

---

## Next Immediate Actions

1. **Start Sprint 2.1 — Finance GL Foundation** (next sprint)
2. Implement journal entry entity and posting logic
3. Build trial balance and ledger inquiry backend services
4. Create GL frontend screens (journal entry, trial balance, ledger)
5. Ensure period lock mechanism prevents backdated entries

---

## Appendix: Technology Versions

- ASP.NET Core: 8.0
- EF Core: 8.0
- PostgreSQL: 16+
- React: 18.3+
- Node.js: 20 LTS
- TypeScript: 5.8+
- Vite: 6+
- shadcn/ui: latest
