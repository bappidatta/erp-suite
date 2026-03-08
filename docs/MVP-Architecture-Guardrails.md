# MVP Architecture Guardrails
## ERP Web Application (ASP.NET Core + EF Core + React + shadcn/ui + PostgreSQL)

**Date:** March 8, 2026  
**Version:** 1.1  
**Applies To:** MVP phase only

---

## 1. Purpose
These guardrails keep the ERP MVP fast to deliver while preserving long-term maintainability. The goal is to get core business flows live without creating architecture debt that blocks scale.

Primary principle: build only what is required for MVP business outcomes (Procure-to-Pay, Order-to-Cash, Record-to-Report).

---

## 2. Architectural North Star
- Runtime style: Modular Monolith (single backend deployable)
- Design style: Pragmatic Clean Architecture
- UI style: Module-aligned React SPA using shadcn/ui
- Data style: Single PostgreSQL database with clear module ownership

This is intentionally not microservices for MVP.

---

## 3. Core Rules (Non-Negotiable)
1. One deployable backend API for MVP.
2. Module boundaries are explicit and enforced in code.
3. No direct cross-module table access.
4. Business rules live in Domain/Application layers, not controllers.
5. Every critical transaction is auditable.
6. No framework-driven complexity unless tied to a real MVP requirement.

---

## 4. Module Boundary Guardrails
Modules: Admin, Master Data, Finance, Sales, Procurement, Inventory, HR, Reporting.

Rules:
- Each module owns:
  - Domain entities and rules
  - Application use cases
  - Infrastructure adapters/repositories
  - API endpoints for module features
- Inter-module calls happen through contracts (interfaces, application services, or internal events).
- Shared kernel is minimal: identity context, base primitives, auditing contracts, common result/error types.
- If a change impacts two modules, ownership is decided first; avoid shared mutable logic.

Acceptance check:
- A developer can remove one module project/folder and compile errors are limited to declared contracts.

---

## 5. Clean Architecture Guardrails (Pragmatic)
Required:
- Domain layer has no dependency on EF Core, ASP.NET Core, or UI libraries.
- Application layer depends on Domain, not Infrastructure.
- Infrastructure depends on Application/Domain contracts.
- Presentation depends on Application contracts.

Avoid for MVP:
- Excessive generic repository patterns where EF Core already provides required behavior.
- Complex mediation/event frameworks for simple CRUD use cases.
- Over-splitting into too many assemblies if team velocity drops.

Rule of thumb:
- Introduce abstractions only when at least two real use cases need them.

---

## 6. Data and Transaction Guardrails
- PostgreSQL is the single source of truth.
- EF Core migrations must be versioned and reviewed.
- Each module has defined schema ownership (table prefixes or separate schemas).
- Transactions are explicit for financial, inventory, and posting operations.
- Concurrency protection is required for stock and accounting balances.
- Hard deletes are disallowed for transactional records; use soft delete or status transitions.

Minimum data standards:
- created_at, created_by, updated_at, updated_by, version on core entities.
- UUID/long ID strategy must be consistent across modules.

---

## 7. API and Integration Guardrails
- API-first backend with stable versioned contracts (`/api/v1/...`).
- Validate inputs at API boundary and Application layer.
- Use idempotency for create operations that may be retried (payments, postings, goods receipts).
- External integrations for MVP are limited to email and CSV.
- No synchronous hard dependency on third-party services for critical transaction completion.

---

## 8. Frontend Guardrails (React + shadcn/ui)
- Frontend routes map to ERP modules.
- Shared UI components come from shadcn/ui plus a thin internal design system layer.
- Forms use consistent validation and error handling patterns.
- Role/permission checks are enforced both in UI and backend (backend is source of truth).
- Avoid building custom UI primitives when shadcn/ui already covers the use case.

React-specific implementation rules:
- Use a feature-first folder structure aligned to ERP modules (finance, sales, procurement, inventory, hr, admin, reporting).
- Keep shared code limited to cross-cutting concerns only (auth, API client, design tokens, common hooks, table primitives).
- Prefer server-state/data fetching libraries and cache invalidation patterns over ad-hoc global state for API data.
- Use local component state for view concerns; use global state only for true cross-module UI/session needs.
- Implement route-level guards for authentication and module permission checks.
- Standardize form stack (single form library + schema validation) across all modules.
- Define typed API contracts for request/response models to keep frontend-backend contracts stable.
- Wrap each module with consistent loading, empty, and error states.

React UI/UX MVP constraints:
- Desktop-first responsive behavior with usable tablet/mobile views for approvals and lookups.
- Maximum initial page payload budget should be defined and tracked; use code-splitting per route/module.
- Long data tables must support server-side pagination, sorting, and filtering.
- Accessibility baseline: keyboard navigation, visible focus states, and semantic labels for form controls.

React testing minimum:
- One component test for each critical transaction form.
- One integration test for each module's main happy-path screen flow.
- Contract tests for API client adapters used in finance/inventory transaction screens.

---

## 9. Security and Compliance Guardrails
- Authentication via JWT/session strategy in ASP.NET Core.
- Authorization must be policy-based and role-aware.
- Company/tenant context must be enforced on every query and command.
- Audit logs are immutable for approvals and financial postings.
- Sensitive fields (passwords, tokens, keys) must never be logged.

---

## 10. Delivery Guardrails (Speed vs Quality)
Required per feature:
1. Business scenario and acceptance criteria defined.
2. API contract defined before UI wiring.
3. Minimal tests for critical paths:
   - Domain rule tests
   - Application use-case tests
   - One API integration test per critical flow
4. Basic observability: structured logs and error correlation ID.
5. Frontend quality checks:
  - Lint + type-check must pass
  - Build must pass with no critical warnings
  - Route/module-level loading and error states implemented

Defer until post-MVP unless needed by a concrete risk:
- Distributed messaging platform
- Saga orchestration frameworks
- Advanced event sourcing
- Full CQRS read/write split across all modules

---

## 11. Definition of Done (Architecture)
A feature is architecture-done only if:
- It stays within module boundaries.
- It does not introduce forbidden dependencies.
- It includes migration updates (if data model changed).
- It includes audit behavior for sensitive operations.
- It includes tests for critical rules and happy-path transaction.
- It is documented in module API notes/changelog.

---

## 12. Governance and Review Cadence
- Weekly architecture review: 30 minutes max, focused on boundary violations and delivery blockers.
- Pull request checklist includes guardrail compliance.
- Any exception must include:
  - reason
  - impact
  - rollback plan
  - follow-up date

---

## 13. Explicit Anti-Patterns (Do Not Do in MVP)
- Splitting into microservices before MVP product-market validation.
- Creating shared database access utilities that bypass module ownership.
- Premature abstraction for unknown future requirements.
- Building a custom component library instead of using shadcn/ui foundation.
- Over-optimizing for hyper-scale before baseline KPI targets are met.
- Creating a large global frontend state store for all module data.
- Duplicating API calling logic in screens instead of using a shared API client layer.
- Mixing module-specific UI components into shared folders without reuse justification.

---

## 14. MVP Architecture Exit Criteria
Architecture is considered MVP-ready when:
- All core modules run in one deployable with clear boundaries.
- Core transactional flows are consistent and auditable.
- Team can deliver new module features without cross-module regressions.
- No unresolved high-severity boundary violations remain.
