using ErpSuite.BuildingBlocks.Application.Common;
using ErpSuite.BuildingBlocks.Domain.Results;
using ErpSuite.Modules.Admin.Infrastructure.Persistence;
using ErpSuite.Modules.Inventory.Application.Warehouses;
using ErpSuite.Modules.Inventory.Application.Warehouses.Dtos;
using ErpSuite.Modules.Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ErpSuite.Modules.Inventory.Infrastructure.Services;

public sealed class WarehouseService : IWarehouseService
{
    private readonly ErpDbContext _dbContext;

    public WarehouseService(ErpDbContext dbContext) => _dbContext = dbContext;

    public async Task<PagedResult<WarehouseResponse>> GetWarehousesAsync(GetWarehousesQuery query, CancellationToken cancellationToken = default)
    {
        var queryable = _dbContext.Warehouses.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var term = query.SearchTerm.ToLower();
            queryable = queryable.Where(w => w.Code.ToLower().Contains(term) || w.Name.ToLower().Contains(term));
        }

        if (query.IsActive.HasValue)
            queryable = queryable.Where(w => w.IsActive == query.IsActive.Value);

        queryable = query.SortBy?.ToLower() switch
        {
            "code" => query.SortDescending ? queryable.OrderByDescending(w => w.Code) : queryable.OrderBy(w => w.Code),
            _ => query.SortDescending ? queryable.OrderByDescending(w => w.Name) : queryable.OrderBy(w => w.Name)
        };

        var totalCount = await queryable.CountAsync(cancellationToken);
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var items = await queryable.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

        return new PagedResult<WarehouseResponse>(items.Select(MapToResponse).ToList(), totalCount, page, pageSize);
    }

    public async Task<WarehouseResponse?> GetWarehouseByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var warehouse = await _dbContext.Warehouses.AsNoTracking().FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
        return warehouse is null ? null : MapToResponse(warehouse);
    }

    public async Task<Result<WarehouseResponse>> CreateWarehouseAsync(CreateWarehouseRequest request, string currentUserId, CancellationToken cancellationToken = default)
    {
        var normalizedCode = request.Code.Trim().ToUpperInvariant();

        if (await _dbContext.Warehouses.AnyAsync(w => w.Code.ToUpper() == normalizedCode, cancellationToken))
            return Result.Failure<WarehouseResponse>("A warehouse with this code already exists.");

        var warehouse = Warehouse.Create(normalizedCode, request.Name.Trim(), request.Location?.Trim(),
            request.Address?.Trim(), request.ContactPerson?.Trim(), request.Phone?.Trim(), request.Notes?.Trim());
        warehouse.SetAudit(currentUserId);
        _dbContext.Warehouses.Add(warehouse);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(MapToResponse(warehouse));
    }

    public async Task<Result<WarehouseResponse>> UpdateWarehouseAsync(long id, UpdateWarehouseRequest request, string currentUserId, CancellationToken cancellationToken = default)
    {
        var warehouse = await _dbContext.Warehouses.FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
        if (warehouse is null) return Result.Failure<WarehouseResponse>("Warehouse not found.");

        warehouse.Update(request.Name.Trim(), request.Location?.Trim(), request.Address?.Trim(),
            request.ContactPerson?.Trim(), request.Phone?.Trim(), request.Notes?.Trim());
        warehouse.SetAudit(currentUserId);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(MapToResponse(warehouse));
    }

    public async Task<Result> DeleteWarehouseAsync(long id, string currentUserId, CancellationToken cancellationToken = default)
    {
        var warehouse = await _dbContext.Warehouses.FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
        if (warehouse is null) return Result.Failure("Warehouse not found.");

        warehouse.SoftDelete(currentUserId);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> ActivateWarehouseAsync(long id, string currentUserId, CancellationToken cancellationToken = default)
    {
        var warehouse = await _dbContext.Warehouses.FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
        if (warehouse is null) return Result.Failure("Warehouse not found.");
        warehouse.Activate();
        warehouse.SetAudit(currentUserId);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> DeactivateWarehouseAsync(long id, string currentUserId, CancellationToken cancellationToken = default)
    {
        var warehouse = await _dbContext.Warehouses.FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
        if (warehouse is null) return Result.Failure("Warehouse not found.");
        warehouse.Deactivate();
        warehouse.SetAudit(currentUserId);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private static WarehouseResponse MapToResponse(Warehouse w) => new(
        w.Id, w.Code, w.Name, w.Location, w.Address, w.ContactPerson,
        w.Phone, w.IsActive, w.Notes, w.CreatedAt, w.CreatedBy, w.UpdatedAt);
}
