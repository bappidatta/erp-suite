# Product Requirements Document (PRD)
## ERP System - MVP

**Date:** March 8, 2026  
**Version:** 1.1 (MVP Scope + Architecture Direction)  
**Document Owner:** Product Team

---

## 1. Product Overview
The ERP MVP is a unified, web-based business management platform for small to mid-sized organizations. It centralizes finance, sales, procurement, inventory, and HR operations into one system with a single source of truth.

The MVP prioritizes operational control, data consistency, and auditability over advanced automation.

---

## 2. Problem Statement
Growing organizations often run critical operations across spreadsheets and disconnected tools. This leads to:
- Duplicate data entry and inconsistent records
- Delayed month-end close and poor reporting reliability
- Weak inventory visibility and procurement inefficiencies
- Limited traceability of approvals and business transactions

The ERP MVP solves these by standardizing master data, workflows, and core transactional modules in one platform.

---

## 3. Goals and Success Metrics
### 3.1 Business Goals
- Reduce operational fragmentation across departments
- Improve financial and inventory accuracy
- Establish process governance via role-based access and approvals
- Enable real-time operational dashboards for managers

### 3.2 MVP Success Metrics (first 90 days post go-live)
- 95% of core business transactions created in ERP (not external spreadsheets)
- Month-end close cycle reduced by at least 30%
- Inventory variance reduced to less than 2%
- At least 80% of approvals completed within SLA
- System uptime at or above 99.5%

---

## 4. Target Users and Personas
- Finance Manager: owns chart of accounts, AP/AR, reconciliation, period close
- Sales Executive: creates quotations and sales orders, tracks customer commitments
- Procurement Officer: handles requisitions, purchase orders, and vendor coordination
- Warehouse Operator: manages stock in/out, transfers, and cycle counts
- HR Administrator: maintains employee records and attendance baseline
- Department Approver: approves transactions based on limits and workflow
- System Administrator: configures users, roles, master data, and settings

---

## 5. MVP Scope Summary
### 5.1 In Scope (MVP)
- Core platform and security
- Master data management
- Finance (GL, AP, AR)
- Sales management
- Procurement management
- Inventory and warehouse management
- Basic HR and attendance
- Reporting and dashboards
- Notifications and approval workflows
- Web application delivery using approved technology stack and architecture constraints

### 5.2 Out of Scope (MVP)
See Section 11 for detailed advanced features explicitly excluded from MVP.

### 5.3 Approved Technology Stack and Architecture Constraints
- Application Type: Web-based ERP (browser-first)
- Backend: ASP.NET Core Web API
- ORM and data access: Entity Framework Core
- Frontend: React
- UI Component System: shadcn/ui
- Database: PostgreSQL
- Architecture Pattern: Clean Architecture with Modular Monolith deployment

Architecture intent for MVP:
- Modular monolith at runtime (single deployable backend), with clear module boundaries in code.
- Each business module (Finance, Sales, Procurement, Inventory, HR, Reporting, Admin) must own its application services, domain model, and infrastructure adapters.
- Cross-module dependencies must be explicit and go through contracts/application interfaces, not direct data-layer coupling.
- Shared kernel should be minimal (identity, common primitives, auditing, tenancy context).
- Database remains a single PostgreSQL instance for MVP, with schema organization aligned to module boundaries.

---

## 6. Functional Requirements by Core Module

## 6.1 Core Platform and Administration
### Objective
Provide foundational system capabilities used by all modules.

### Requirements
- Multi-company support (single tenant, multiple legal entities)
- Role-based access control (RBAC) with permission matrix by module/action
- User lifecycle: create, deactivate, reset password, enforce password policy
- Configurable fiscal year, currency, tax defaults, and numbering sequences
- Audit trail for create/update/delete and approval actions
- File attachment support on key transactions
- Basic in-app notifications for approvals, rejections, and exceptions

### Key MVP Deliverables
- Admin console for user/role setup
- Organization settings page
- Approval workflow configuration (single-level and two-level)

---

## 6.2 Master Data Management
### Objective
Standardize shared records to prevent duplication and ensure transaction integrity.

### Requirements
- Customer master (billing, shipping, tax info, credit limit)
- Vendor master (payment terms, tax details, bank details)
- Item master (SKU, UOM, category, valuation method for MVP: moving average)
- Warehouse and location master
- Employee master (basic profile and employment details)
- Tax master and chart of accounts master
- Duplicate prevention rules and validation checks

### Key MVP Deliverables
- CRUD interfaces for all master entities
- Import templates (CSV) for customer, vendor, item, and opening balances
- Active/inactive status and soft-delete policy

---

## 6.3 Finance Module (GL, AP, AR)
### Objective
Enable accurate bookkeeping and basic financial control.

### General Ledger (GL)
- Chart of Accounts setup and account hierarchy
- Journal entries (manual and system-generated)
- Trial balance, ledger book, and period close controls
- Lock posting periods after close

### Accounts Payable (AP)
- Vendor invoice recording
- 3-way matching (PO, Goods Receipt, Invoice) in baseline form
- Payment voucher generation and payment status tracking
- AP aging report

### Accounts Receivable (AR)
- Customer invoice generation from sales orders or delivery notes
- Receipt entry and allocation against invoices
- AR aging report
- Credit limit check at sales order confirmation

### Tax and Compliance (MVP Baseline)
- Tax code mapping for transactions
- Tax calculation at line/document level
- Tax summary reports for filing support

### Key MVP Deliverables
- Financial posting engine for all integrated modules
- Month-end close checklist with posting status indicators
- Exportable reports (CSV/PDF)

---

## 6.4 Sales Module
### Objective
Manage the order-to-cash lifecycle from quotation to invoicing.

### Requirements
- Lead-independent customer order process (CRM-lite, no full CRM)
- Quotation creation, revision, and approval
- Sales order creation from approved quotation
- Price list and discount rules (basic, fixed and percentage)
- Delivery note generation
- Invoice trigger to AR
- Sales return handling (against invoice)
- Order status tracking: Draft, Submitted, Approved, Fulfilled, Partially Fulfilled, Closed

### Key MVP Deliverables
- Sales pipeline view (quotation to invoice)
- Margin visibility per order line (using moving average cost)
- Basic customer account statement

---

## 6.5 Procurement Module
### Objective
Manage procure-to-pay lifecycle with spend controls.

### Requirements
- Purchase requisition creation by departments
- RFQ-lite flow (optional request to one or more vendors)
- Purchase order creation and approval
- Goods receipt note (GRN) and quality status (accepted/rejected only)
- Vendor invoice handoff to AP
- Purchase return flow
- PO status tracking: Draft, Approved, Open, Partially Received, Closed

### Key MVP Deliverables
- Budget check placeholder (alert-only, no hard stop)
- Lead time tracking by vendor
- Open PO and overdue delivery reports

---

## 6.6 Inventory and Warehouse Module
### Objective
Provide real-time stock visibility and movement control.

### Requirements
- Stock movement types: receipt, issue, transfer, adjustment
- Multi-warehouse stock visibility
- Batch tracking (MVP basic identifier; no expiry-driven FEFO logic)
- Reorder level and low-stock alerts
- Cycle count entry and variance adjustment
- Stock valuation using moving average
- Inventory ledger with transaction traceability

### Key MVP Deliverables
- Current stock report by warehouse and SKU
- Stock aging report (basic)
- Negative stock prevention configurable by item category

---

## 6.7 Basic HR and Attendance Module
### Objective
Support foundational workforce administration tied to operations.

### Requirements
- Employee onboarding records and department assignment
- Attendance capture (manual and CSV import)
- Leave request and approval (basic leave types)
- Shift templates (single shift per employee in MVP)
- Employee status management (active/inactive/resigned)

### Key MVP Deliverables
- Attendance summary report
- Leave balance view
- Department-wise headcount dashboard

---

## 6.8 Reporting and Dashboards
### Objective
Enable management visibility across operational and financial KPIs.

### Requirements
- Role-specific dashboards (Finance, Sales, Procurement, Warehouse, HR)
- Standard reports:
  - Trial Balance, AP Aging, AR Aging
  - Sales by customer/item/period
  - Purchase by vendor/item/period
  - Stock on hand, stock movement, stock aging
  - Attendance and leave summaries
- Filters by company, date range, department, warehouse, and status
- Export to CSV and PDF

### Key MVP Deliverables
- Dashboard widgets with near real-time refresh (up to 5-minute delay acceptable)
- Saved report views per user

---

## 6.9 Approval Workflow and Notifications
### Objective
Enforce governance and approval controls on critical transactions.

### Requirements
- Approval policies by document type and threshold amount
- Single-stage and two-stage approval chains
- Approve/reject with mandatory comments on rejection
- Email and in-app notifications for pending approvals
- Escalation alerts for overdue approvals

### Key MVP Deliverables
- Approval inbox by user role
- SLA timer and breached approval report

---

## 6.10 Technical Implementation Blueprint (MVP)
### Objective
Define implementation guardrails for a web-based ERP built as a Clean Architecture modular monolith.

### Clean Architecture Layers (per module)
- Domain Layer: entities, value objects, domain services, domain rules
- Application Layer: use cases, commands/queries, DTOs, interfaces, validation, orchestration
- Infrastructure Layer: EF Core repositories, external integrations, persistence configuration
- Presentation Layer: ASP.NET Core controllers/minimal APIs and React-based UI clients

### Modular Monolith Rules
- One ASP.NET Core host for MVP deployment.
- Modules organized as independent projects/folders with internal encapsulation.
- Inter-module communication in-process via application contracts and events.
- No direct table access across modules from EF Core DbContext usage; use module APIs/contracts.

### Data and Persistence Standards
- PostgreSQL as system of record for all modules in MVP.
- EF Core migrations managed with traceable module ownership.
- Concurrency control for financial and inventory transactions (optimistic concurrency + transactional boundaries).
- Audit columns standardization: created_at, created_by, updated_at, updated_by, version.

### Frontend Standards
- React SPA for authenticated ERP users.
- shadcn/ui as default component library with a shared design token layer.
- Module-based frontend routing aligned with backend module boundaries.
- Form validation parity between frontend and backend for critical transaction flows.

### Security and Access
- ASP.NET Core Identity/JWT-based authentication (or equivalent token strategy).
- Authorization policy mapping aligned to ERP roles and approval limits.
- Tenant/company context enforced at API and query levels.

### Key MVP Deliverables
- Baseline solution template implementing one vertical slice per core module.
- Shared engineering standards document (naming, layering, dependency rules, migration rules).
- CI checks for architecture boundaries (forbidden references/dependency violations).

---

## 7. Non-Functional Requirements (MVP)
- Availability: minimum 99.5% monthly uptime
- Performance: 95th percentile page load under 3 seconds for standard list/detail pages
- Scalability: support up to 300 concurrent users in MVP target environment
- Security:
  - RBAC enforcement on all API endpoints
  - Encryption in transit (TLS)
  - Hashed passwords and secure session handling
- Auditability: immutable audit logs for financial and approval events
- Backup and Recovery:
  - Daily automated backup
  - RPO up to 24 hours, RTO up to 8 hours
- Compliance readiness: system logs and data export support for audits

---

## 8. Integrations (MVP)
### In Scope
- SMTP/email service for notifications
- CSV import/export framework for data migration and reporting

### Deferred Beyond MVP
- Bank API integrations
- E-invoicing government gateways
- Biometric device integrations
- E-commerce marketplace integrations

---

## 9. Assumptions and Constraints
- MVP is a web application (desktop browser-first), responsive for tablet/mobile viewing
- Single language support in MVP
- Single base currency with optional transaction currency display
- No advanced AI/ML forecasting in MVP
- Workflow complexity limited to two approval levels
- Backend stack is fixed to ASP.NET Core + EF Core
- Frontend stack is fixed to React + shadcn/ui
- Database stack is fixed to PostgreSQL
- Deployment style for MVP is fixed to modular monolith (not microservices)

---

## 10. MVP Milestones and Release Plan
- Phase 1 (Foundation): Core platform, security, master data
- Phase 2 (Transactions): Finance, Sales, Procurement, Inventory
- Phase 3 (People + Insights): HR basic, dashboards, reporting, UAT hardening
- Go-live Readiness: migration validation, role training, cutover checklist

Suggested MVP timeline: 16-24 weeks depending on integration and migration complexity.

---

## 11. Advanced Features Explicitly Out of Scope (Post-MVP Backlog)

### 11.1 Finance Advanced
- Multi-book accounting and advanced consolidation
- Automated bank reconciliation via direct bank feeds
- Advanced fixed assets lifecycle and depreciation engine
- Complex revenue recognition (IFRS/ASC-specific automation)

### 11.2 Sales and CRM Advanced
- Full CRM (lead scoring, campaign automation, opportunity forecasting)
- CPQ (advanced pricing, bundles, guided selling)
- Territory management and incentive compensation engine

### 11.3 Procurement Advanced
- Supplier portal and self-service onboarding
- Contract lifecycle management with clause analytics
- Advanced sourcing events and bid optimization

### 11.4 Inventory and Supply Chain Advanced
- FEFO/FIFO with expiry and lot quality workflows
- Advanced demand forecasting and replenishment optimization
- WMS features: wave picking, putaway optimization, barcode device orchestration

### 11.5 HR Advanced
- Full payroll engine with statutory compliance automation
- Performance management and OKR system
- Learning management and succession planning

### 11.6 Platform Advanced
- Multi-tenant SaaS isolation architecture
- Workflow studio with no-code conditional branching beyond two levels
- Embedded BI with ad hoc semantic modeling
- AI copilot for transaction assistance and anomaly detection
- Microservices decomposition of ERP modules
- Polyglot persistence across module-specific databases

### 11.7 Integrations Advanced
- Real-time integration bus and iPaaS connectors
- EDI integration with suppliers/customers
- Native mobile apps with offline sync

---

## 12. Acceptance Criteria (MVP Exit)
- All in-scope modules pass UAT with agreed sign-off
- End-to-end flows validated:
  - Procure-to-Pay
  - Order-to-Cash
  - Record-to-Report
- Core reports reconcile with transactional records
- Role-based security and audit trail verified
- Production cutover checklist completed and approved

---

## 13. Risks and Mitigations
- Data migration quality risk: run multiple mock migrations and reconciliation checkpoints
- Scope creep risk: strict change control and backlog triage
- User adoption risk: role-based training and SOP documentation
- Integration delays: keep MVP with low external dependency footprint

---

## 14. Open Questions for Stakeholder Alignment
- Which statutory tax/report formats are mandatory at go-live?
- Is batch tracking mandatory for all items or specific categories only?
- What approval thresholds should be applied per department?
- Is single-company rollout required first, followed by multi-company phase-in?
- What is the exact month-end close SLA target?

---

## 15. Implementation Companion Document
- MVP Architecture Guardrails: `docs/MVP-Architecture-Guardrails.md`
