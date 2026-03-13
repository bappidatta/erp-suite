---
description: "Use when writing or modifying any code in the ERP Suite workspace. Covers backend C# architecture, frontend React/TypeScript conventions, database patterns, API design, and testing standards for this modular monolith ERP system."
applyTo: "**"
---

# ERP Suite — Architecture & Coding Conventions

## Backend Architecture (C# / ASP.NET Core)

### Module Structure

Every module follows Clean Architecture with four layers:

```
Domain/          → Entities, value objects, enums, domain errors
Application/     → Services (interfaces), DTOs (records), FluentValidation validators
Infrastructure/  → EF Core repositories, DbContext configurations, DI registration
Presentation/    → Minimal API endpoint definitions
```

**Dependency direction**: Domain → Application → Infrastructure. Domain has zero framework dependencies.

### Entity Conventions

- All domain entities inherit from `BaseAuditableEntity` (which extends `BaseEntity`)
- Use **private setters** on all entity properties
- Create entities via **static `Create()` factory methods**, not constructors
- Audit fields (`CreatedAt`, `CreatedBy`, `UpdatedAt`, `UpdatedBy`) are set by `AuditInterceptor`, never manually
- `Version` property is the optimistic concurrency token
- Soft delete via `SoftDelete(userId)` / `Restore()` — never hard-delete business entities

### Result Pattern

Return `Result` or `Result<T>` from service methods — do not throw exceptions for business errors.

```csharp
// ✅ Correct
public async Task<Result<CustomerResponse>> CreateAsync(CreateCustomerRequest request) { ... }

// ❌ Wrong — don't throw for domain/business errors
throw new InvalidOperationException("Customer already exists");
```

### DTOs

- Use C# `record` types for all request/response/query models
- Naming: `Create{Entity}Request`, `Update{Entity}Request`, `{Entity}Response`, `Get{Entity}Query`

### Module Registration

Each module exposes `Add{Module}{Layer}()` extension methods (e.g., `AddSalesInfrastructure()`).

### Namespaces

Follow `ErpSuite.Modules.{ModuleName}.{Layer}.{Feature}` or `ErpSuite.BuildingBlocks.{Layer}`.

### Module Boundaries

Modules must not directly access another module's database tables or internal services. Cross-module communication goes through defined contracts.

---

## Frontend (React / TypeScript)

### Stack

React 19, Vite, React Router, TanStack React Query, React Hook Form + Zod, shadcn/ui, Tailwind CSS.

### Structure

```
src/modules/{module-name}/
  pages/        → Page components (PascalCase .tsx)
  components/   → Module-local components
  api/          → API client functions ({module}Api.ts)
  types.ts      → Module TypeScript interfaces
```

### Conventions

- **Server state**: Use React Query — no Redux for API data
- **Forms**: React Hook Form + Zod schemas for validation
- **Styling**: Tailwind utility classes + shadcn/ui components — no custom CSS unless necessary
- **Folders**: kebab-case (`master-data`, `finance`)
- **Components/Pages**: PascalCase filenames (`CustomersPage.tsx`)
- **Hooks**: `use` prefix, camelCase (`useCustomerForm.ts`)
- **TypeScript interfaces** must match backend DTOs exactly

---

## Database / EF Core

### Naming

- **Tables**: `snake_case` (e.g., `customers`, `tax_codes`, `audit_logs`)
- **Columns**: `snake_case` (e.g., `created_at`, `is_deleted`)

### Patterns

- All entities get audit fields and version — configured in `BaseDbContext`
- Global query filters enforce soft delete: deleted records are excluded by default
- Use explicit transactions for financial, inventory, and posting operations
- Entity configurations go in `Infrastructure` layer via `IEntityTypeConfiguration<T>`

---

## API Design

### Endpoints

- Minimal API endpoints defined in `Presentation/` layer
- Group endpoints logically by feature/entity
- Validate requests with FluentValidation before processing
- Return `Result<T>` from services, map to appropriate HTTP status codes in endpoints

### Authentication

- JWT bearer tokens + cookie auth
- Role-based authorization
- Protect all non-public endpoints

---

## Testing

- **Framework**: xUnit + FluentAssertions
- **Folder structure**: `tests/Unit/ErpSuite.Tests.Unit/{ModuleName}/`
- **Style**: Arrange-Act-Assert with `[Fact]` attributes
- **Database tests**: Use `TestDbContextFactory.Create()` for in-memory contexts
- **Naming**: `{MethodUnderTest}_Should{ExpectedBehavior}_When{Condition}`

---

## General Rules

- `Nullable` is enabled — respect null-reference safety
- Business logic lives in Domain or Application layers, never in endpoints or infrastructure
- One concern per class/file — no god classes
- Prefer explicit over implicit — no magic strings or untyped dictionaries for domain concepts
