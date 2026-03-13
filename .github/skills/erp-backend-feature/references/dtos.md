# DTO Templates

## Create Request

```csharp
namespace ErpSuite.Modules.{Module}.Application.{Feature}.Dtos;

public record Create{Entity}Request(
    string Code,
    string Name,
    // Required fields as positional parameters
    // Optional fields with defaults:
    string? OptionalField = null,
    decimal NumericField = 0,
    bool BoolField = true);
```

## Update Request

```csharp
namespace ErpSuite.Modules.{Module}.Application.{Feature}.Dtos;

public record Update{Entity}Request(
    string Name,
    // Mutable fields only — no Code (immutable after creation)
    string? OptionalField = null,
    decimal NumericField = 0);
```

## Response

```csharp
namespace ErpSuite.Modules.{Module}.Application.{Feature}.Dtos;

public record {Entity}Response(
    long Id,
    string Code,
    string Name,
    // All readable fields
    bool IsActive,
    DateTime CreatedAt,
    string CreatedBy,
    DateTime? UpdatedAt,
    string? UpdatedBy);
```

## Query

```csharp
namespace ErpSuite.Modules.{Module}.Application.{Feature}.Dtos;

public class Get{Entities}Query
{
    public string? SearchTerm { get; init; }
    public bool? IsActive { get; init; }
    // Entity-specific filters
    public string? SortBy { get; init; } = "name";
    public bool SortDescending { get; init; } = false;
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
```

## Rules

- All DTOs use `record` type except Query (which uses `class` with `init` properties for `[AsParameters]` binding)
- Request records: required fields are positional, optional use named parameters with defaults
- Response records include audit fields: `CreatedAt`, `CreatedBy`, `UpdatedAt`, `UpdatedBy`
- Query class always includes: `SearchTerm`, `SortBy`, `SortDescending`, `Page`, `PageSize`
