# Service Implementation Template

```csharp
using ErpSuite.BuildingBlocks.Domain.Results;
using ErpSuite.Modules.Admin.Infrastructure.Persistence;
using ErpSuite.BuildingBlocks.Application.Common;
using ErpSuite.Modules.{Module}.Application.{Feature};
using ErpSuite.Modules.{Module}.Application.{Feature}.Dtos;
using ErpSuite.Modules.{Module}.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ErpSuite.Modules.{Module}.Infrastructure.Services;

public sealed class {Entity}Service : I{Entity}Service
{
    private readonly ErpDbContext _dbContext;

    public {Entity}Service(ErpDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<{Entity}Response>> Get{Entities}Async(
        Get{Entities}Query query, CancellationToken cancellationToken = default)
    {
        var queryable = _dbContext.{Entities}.AsNoTracking().AsQueryable();

        // Search
        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var term = query.SearchTerm.ToLower();
            queryable = queryable.Where(e =>
                e.Code.ToLower().Contains(term) ||
                e.Name.ToLower().Contains(term));
        }

        // Filters
        if (query.IsActive.HasValue)
            queryable = queryable.Where(e => e.IsActive == query.IsActive.Value);

        // Sort
        queryable = query.SortBy?.ToLower() switch
        {
            "code" => query.SortDescending ? queryable.OrderByDescending(e => e.Code) : queryable.OrderBy(e => e.Code),
            "createdat" => query.SortDescending ? queryable.OrderByDescending(e => e.CreatedAt) : queryable.OrderBy(e => e.CreatedAt),
            _ => query.SortDescending ? queryable.OrderByDescending(e => e.Name) : queryable.OrderBy(e => e.Name)
        };

        // Paginate
        var totalCount = await queryable.CountAsync(cancellationToken);
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var items = await queryable
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<{Entity}Response>(
            items.Select(MapToResponse).ToList(),
            totalCount, page, pageSize);
    }

    public async Task<{Entity}Response?> Get{Entity}ByIdAsync(
        long id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.{Entities}
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        return entity is null ? null : MapToResponse(entity);
    }

    public async Task<Result<{Entity}Response>> Create{Entity}Async(
        Create{Entity}Request request, string currentUserId,
        CancellationToken cancellationToken = default)
    {
        var normalizedCode = request.Code.Trim().ToUpperInvariant();

        if (await _dbContext.{Entities}.AnyAsync(
            e => e.Code.ToUpper() == normalizedCode, cancellationToken))
            return Result.Failure<{Entity}Response>(
                "A {entity} with this code already exists.");

        var entity = {Entity}.Create(normalizedCode, request.Name /* , ... */);

        entity.SetAudit(currentUserId);
        _dbContext.{Entities}.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(MapToResponse(entity));
    }

    public async Task<Result<{Entity}Response>> Update{Entity}Async(
        long id, Update{Entity}Request request, string currentUserId,
        CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.{Entities}
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (entity is null)
            return Result.Failure<{Entity}Response>("{Entity} not found.");

        entity.Update(request.Name /* , ... */);
        entity.SetAudit(currentUserId);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(MapToResponse(entity));
    }

    public async Task<Result> Delete{Entity}Async(
        long id, string currentUserId,
        CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.{Entities}
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (entity is null)
            return Result.Failure("{Entity} not found.");

        entity.SoftDelete(currentUserId);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> Activate{Entity}Async(
        long id, string currentUserId,
        CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.{Entities}
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (entity is null)
            return Result.Failure("{Entity} not found.");

        entity.Activate();
        entity.SetAudit(currentUserId);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> Deactivate{Entity}Async(
        long id, string currentUserId,
        CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.{Entities}
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (entity is null)
            return Result.Failure("{Entity} not found.");

        entity.Deactivate();
        entity.SetAudit(currentUserId);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private static {Entity}Response MapToResponse({Entity} entity)
    {
        return new {Entity}Response(
            entity.Id,
            entity.Code,
            entity.Name,
            // Map all fields
            entity.IsActive,
            entity.CreatedAt,
            entity.CreatedBy,
            entity.UpdatedAt,
            entity.UpdatedBy);
    }
}
```

## Rules

- Class is `sealed`
- Constructor injects `ErpDbContext` only (no repository abstraction)
- `Create`: normalize code → check duplicates → factory method → `SetAudit` → `Add` → `SaveChangesAsync`
- `Update`: fetch → null check → `entity.Update()` → `SetAudit` → `SaveChangesAsync`
- `Delete`: soft delete via `entity.SoftDelete(currentUserId)`, never `Remove()`
- List: always `AsNoTracking()`, clamp page size to `[1, 100]`
- `MapToResponse` is a `private static` method
