---
name: erp-backend-feature
description: "Scaffold and implement a complete backend CRUD feature in the ERP Suite modular monolith. Use when adding a new entity, service, endpoints, or tests to any module (Sales, Finance, Procurement, HR, Inventory, Admin, Reporting). Covers domain entity, DTOs, validation, service interface/implementation, EF Core configuration, DI registration, Minimal API endpoints, and unit tests."
argument-hint: "Describe the feature, entity name, and target module (e.g., 'Add Product entity to Inventory module')"
---

# ERP Backend Feature Implementation

## When to Use

- Adding a new business entity with full CRUD to any backend module
- Implementing a new feature that requires domain → application → infrastructure → presentation layers
- Scaffolding the boilerplate for a new entity following established patterns

## Pre-Flight

1. Confirm with the user: **entity name**, **target module**, and **key properties**
2. If the user hasn't specified properties, ask what fields the entity needs
3. Verify the module exists under `backend/src/Modules/{ModuleName}/`

## Procedure

Follow these steps in order. Each step produces one or more files.

### Step 1 — Domain Entity

Create `backend/src/Modules/{Module}/Domain/Entities/{Entity}.cs`

Rules:
- Inherit from `BaseAuditableEntity`
- **Private setters** on all properties
- **Private parameterless constructor** for EF Core: `private {Entity}() { }`
- **Static `Create()` factory method** — returns a new instance with all required fields
- **`Update()` method** — mutates mutable fields via private setters
- **`Activate()` / `Deactivate()`** methods if the entity has an `IsActive` flag
- Add domain methods for state transitions (avoid anemic model)
- Namespace: `ErpSuite.Modules.{Module}.Domain.Entities`

Reference: [Domain entity template](./references/domain-entity.md)

### Step 2 — Domain Enums (if needed)

Create `backend/src/Modules/{Module}/Domain/Entities/{EnumName}.cs` (or `Enums/` subfolder)

- Simple C# enums for status, type, or category fields

### Step 3 — Application DTOs

Create these files under `backend/src/Modules/{Module}/Application/{Feature}/Dtos/`:

| File | Type | Notes |
|------|------|-------|
| `Create{Entity}Request.cs` | `record` | Required fields as positional params, optional as named with defaults |
| `Update{Entity}Request.cs` | `record` | Mutable fields only (no Code), optional params with defaults |
| `{Entity}Response.cs` | `record` | All readable fields including `Id`, audit fields (`CreatedAt`, `CreatedBy`, etc.) |
| `Get{Entity}Query.cs` | `class` | Query parameters: `SearchTerm`, filters, `SortBy`, `SortDescending`, `Page`, `PageSize` |

- Namespace: `ErpSuite.Modules.{Module}.Application.{Feature}.Dtos`

Reference: [DTO templates](./references/dtos.md)

### Step 4 — Application Service Interface

Create `backend/src/Modules/{Module}/Application/{Feature}/I{Entity}Service.cs`

Standard CRUD methods:
```csharp
Task<PagedResult<{Entity}Response>> Get{Entities}Async(Get{Entities}Query query, CancellationToken cancellationToken = default);
Task<{Entity}Response?> Get{Entity}ByIdAsync(long id, CancellationToken cancellationToken = default);
Task<Result<{Entity}Response>> Create{Entity}Async(Create{Entity}Request request, string currentUserId, CancellationToken cancellationToken = default);
Task<Result<{Entity}Response>> Update{Entity}Async(long id, Update{Entity}Request request, string currentUserId, CancellationToken cancellationToken = default);
Task<Result> Delete{Entity}Async(long id, string currentUserId, CancellationToken cancellationToken = default);
Task<Result> Activate{Entity}Async(long id, string currentUserId, CancellationToken cancellationToken = default);
Task<Result> Deactivate{Entity}Async(long id, string currentUserId, CancellationToken cancellationToken = default);
```

- Use `Result` / `Result<T>` from `ErpSuite.BuildingBlocks.Domain.Results`
- Use `PagedResult<T>` from `ErpSuite.BuildingBlocks.Application.Common`

### Step 5 — FluentValidation Validators

Create `backend/src/Modules/{Module}/Application/{Feature}/Validators/Create{Entity}RequestValidator.cs` and `Update{Entity}RequestValidator.cs`

Rules:
- Inherit from `AbstractValidator<T>`
- `NotEmpty()` for required strings
- `MaximumLength()` matching EF column config
- `EmailAddress()` for email fields (with `.When()` guard for optional)
- `GreaterThan(0)` for required FK references
- `InclusiveBetween()` for bounded numerics (e.g., rates 0–100)

### Step 6 — Infrastructure Service Implementation

Create `backend/src/Modules/{Module}/Infrastructure/Services/{Entity}Service.cs`

Rules:
- `sealed class` implementing `I{Entity}Service`
- Constructor-inject `ErpDbContext`
- **Create**: normalize code to `UpperInvariant`, check duplicates with `AnyAsync`, use `Entity.Create()`, call `SetAudit(currentUserId)`
- **Update**: fetch entity, call `entity.Update()`, call `SetAudit(currentUserId)`
- **Delete**: soft delete via `entity.SoftDelete(currentUserId)`
- **List**: `AsNoTracking()`, apply search/filter/sort via switch expression, paginate with `Skip/Take`, return `PagedResult<T>`
- **GetById**: `AsNoTracking().FirstOrDefaultAsync()`
- Include a `private static {Entity}Response MapToResponse({Entity} entity)` method

Reference: [Service template](./references/service-impl.md)

### Step 7 — EF Core Entity Configuration

Add entity configuration in `ErpDbContext.OnModelCreating()` at:
`backend/src/Modules/Admin/Infrastructure/Persistence/ErpDbContext.cs`

Rules:
- `entity.ToTable("{snake_case_plural}")` — e.g., `products`, `purchase_orders`
- `entity.HasKey(e => e.Id)`
- All columns: `.HasColumnName("snake_case")`
- Strings: `.HasMaxLength(N)` matching validator
- Decimals: `.HasPrecision(18, 2)` for money, `.HasPrecision(18, 4)` for rates
- Enums: `.HasConversion<int>()`
- Booleans with defaults: `.HasDefaultValue(true)`
- Unique indexes on `Code` fields: `entity.HasIndex(e => e.Code).IsUnique()`
- Name indexes on searchable fields
- FK relationships: `.HasOne().WithMany().HasForeignKey().OnDelete(DeleteBehavior.Restrict)`

Also add `DbSet<{Entity}>` property to `ErpDbContext`.

### Step 8 — DI Registration

Update `backend/src/Modules/{Module}/Infrastructure/DependencyInjection.cs`:
- Add `services.AddScoped<I{Entity}Service, {Entity}Service>()`

If this is the first feature in the module, also:
- Create the `DependencyInjection.cs` with `Add{Module}Infrastructure()` extension method
- Register it in `Program.cs`

### Step 9 — Validator Registration

Add validator assembly scanning in `Program.cs` if this is the first validator in the module:
```csharp
builder.Services.AddValidatorsFromAssemblyContaining<Create{Entity}RequestValidator>();
```

### Step 10 — Minimal API Endpoints

Create `backend/src/Host/Api/Endpoints/{Module}/{Entity}Endpoints.cs`

Rules:
- Static class with `Map{Entity}Endpoints(this IEndpointRouteBuilder app)` extension
- `app.MapGroup("api/{module}/{entities}")` — lowercase, plural
- `.WithTags("{Module} - {Entities}")` for OpenAPI
- `.RequireAuthorization("AuthenticatedUser")`
- Each endpoint: `GET ""`, `GET "{id:long}"`, `POST ""`, `PUT "{id:long}"`, `DELETE "{id:long}"`, `POST "{id:long}/activate"`, `POST "{id:long}/deactivate"`
- POST/PUT: add `.AddEndpointFilter<ValidationFilter<T>>()`
- GET by ID: add `.WithName("Get{Entity}")` for `CreatedAtRoute`
- Extract `currentUserId` from `httpContext.User.FindFirst("user_id")?.Value ?? "system"`
- Map `Result.IsFailure` → `Results.BadRequest(new { message = result.Error })`
- Create → `Results.CreatedAtRoute("Get{Entity}", new { id = result.Value.Id }, result.Value)`
- Delete → `Results.NoContent()`

Register in `EndpointExtensions.cs`:
```csharp
app.Map{Entity}Endpoints();
```

### Step 11 — Unit Tests

Create `backend/tests/Unit/ErpSuite.Tests.Unit/{Module}/{Entity}ServiceTests.cs`

Required test cases:
1. `Create{Entity}_WithValidRequest_ShouldReturnSuccess` — verify all fields
2. `Create{Entity}_WithDuplicateCode_ShouldReturnFailure` — case-insensitive
3. `Create{Entity}_ShouldNormalizeCodeToUpperCase`
4. `Get{Entity}ById_WhenExists_ShouldReturnEntity`
5. `Get{Entity}ById_WhenNotExists_ShouldReturnNull`
6. `Update{Entity}_WhenExists_ShouldReturnSuccess`
7. `Update{Entity}_WhenNotExists_ShouldReturnFailure`
8. `Delete{Entity}_WhenExists_ShouldSoftDelete`

Test setup:
```csharp
private readonly {Entity}Service _sut;
private readonly ErpDbContext _dbContext;

public {Entity}ServiceTests()
{
    _dbContext = TestDbContextFactory.Create();
    _sut = new {Entity}Service(_dbContext);
}
```

Style: `[Fact]`, FluentAssertions (`.Should()`, `.BeTrue()`, `.Be()`, `.Contain()`), Arrange-Act-Assert

### Step 12 — Build & Verify

Run from `backend/`:
```
dotnet build
dotnet test
```

Fix any compilation errors before finishing.

## Checklist

- [ ] Domain entity with `BaseAuditableEntity`, factory method, private setters
- [ ] DTOs as `record` types with correct naming
- [ ] Service interface returning `Result<T>` / `PagedResult<T>`
- [ ] FluentValidation validators for Create and Update requests
- [ ] Service implementation with duplicate checks, audit, soft delete
- [ ] EF Core configuration with snake_case columns and indexes
- [ ] DbSet added to ErpDbContext
- [ ] DI registration in module's `DependencyInjection.cs`
- [ ] Validator registered in `Program.cs`
- [ ] Minimal API endpoints with auth, validation filters, proper HTTP status codes
- [ ] Endpoints registered in `EndpointExtensions.cs`
- [ ] Unit tests covering happy path and error cases
- [ ] `dotnet build` passes
- [ ] `dotnet test` passes
