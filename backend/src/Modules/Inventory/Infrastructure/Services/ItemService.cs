using ErpSuite.BuildingBlocks.Application.Common;
using ErpSuite.BuildingBlocks.Domain.Results;
using ErpSuite.Modules.Admin.Infrastructure.Persistence;
using ErpSuite.Modules.Inventory.Application.Items;
using ErpSuite.Modules.Inventory.Application.Items.Dtos;
using ErpSuite.Modules.Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ErpSuite.Modules.Inventory.Infrastructure.Services;

public sealed class ItemService : IItemService
{
    private readonly ErpDbContext _dbContext;

    public ItemService(ErpDbContext dbContext) => _dbContext = dbContext;

    public async Task<PagedResult<ItemResponse>> GetItemsAsync(GetItemsQuery query, CancellationToken cancellationToken = default)
    {
        var queryable = _dbContext.Items
            .AsNoTracking()
            .Include(i => i.Category)
            .Include(i => i.Uom)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var term = query.SearchTerm.ToLower();
            queryable = queryable.Where(i => i.Code.ToLower().Contains(term) || i.Name.ToLower().Contains(term));
        }

        if (query.IsActive.HasValue)
            queryable = queryable.Where(i => i.IsActive == query.IsActive.Value);

        if (query.CategoryId.HasValue)
            queryable = queryable.Where(i => i.CategoryId == query.CategoryId.Value);

        if (query.Type.HasValue)
            queryable = queryable.Where(i => (int)i.Type == query.Type.Value);

        queryable = query.SortBy?.ToLower() switch
        {
            "code" => query.SortDescending ? queryable.OrderByDescending(i => i.Code) : queryable.OrderBy(i => i.Code),
            _ => query.SortDescending ? queryable.OrderByDescending(i => i.Name) : queryable.OrderBy(i => i.Name)
        };

        var totalCount = await queryable.CountAsync(cancellationToken);
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var items = await queryable.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

        return new PagedResult<ItemResponse>(items.Select(MapToResponse).ToList(), totalCount, page, pageSize);
    }

    public async Task<ItemResponse?> GetItemByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var item = await _dbContext.Items
            .AsNoTracking()
            .Include(i => i.Category)
            .Include(i => i.Uom)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
        return item is null ? null : MapToResponse(item);
    }

    public async Task<Result<ItemResponse>> CreateItemAsync(CreateItemRequest request, string currentUserId, CancellationToken cancellationToken = default)
    {
        var normalizedCode = request.Code.Trim().ToUpperInvariant();

        if (await _dbContext.Items.AnyAsync(i => i.Code.ToUpper() == normalizedCode, cancellationToken))
            return Result.Failure<ItemResponse>("An item with this code already exists.");

        if (!await _dbContext.UnitsOfMeasure.AnyAsync(u => u.Id == request.UomId, cancellationToken))
            return Result.Failure<ItemResponse>("The specified UOM does not exist.");

        if (request.CategoryId.HasValue && !await _dbContext.Categories.AnyAsync(c => c.Id == request.CategoryId.Value, cancellationToken))
            return Result.Failure<ItemResponse>("The specified category does not exist.");

        var item = Item.Create(normalizedCode, request.Name.Trim(), request.Description?.Trim(),
            request.CategoryId, request.UomId,
            (ItemType)request.Type, (ValuationMethod)request.ValuationMethod,
            request.StandardCost, request.SalePrice, request.ReorderLevel,
            request.Barcode?.Trim(), request.Notes?.Trim());
        item.SetAudit(currentUserId);
        _dbContext.Items.Add(item);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(MapToResponse(item));
    }

    public async Task<Result<ItemResponse>> UpdateItemAsync(long id, UpdateItemRequest request, string currentUserId, CancellationToken cancellationToken = default)
    {
        var item = await _dbContext.Items
            .Include(i => i.Category)
            .Include(i => i.Uom)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
        if (item is null) return Result.Failure<ItemResponse>("Item not found.");

        if (!await _dbContext.UnitsOfMeasure.AnyAsync(u => u.Id == request.UomId, cancellationToken))
            return Result.Failure<ItemResponse>("The specified UOM does not exist.");

        if (request.CategoryId.HasValue && !await _dbContext.Categories.AnyAsync(c => c.Id == request.CategoryId.Value, cancellationToken))
            return Result.Failure<ItemResponse>("The specified category does not exist.");

        item.Update(request.Name.Trim(), request.Description?.Trim(), request.CategoryId, request.UomId,
            (ItemType)request.Type, (ValuationMethod)request.ValuationMethod,
            request.StandardCost, request.SalePrice, request.ReorderLevel,
            request.Barcode?.Trim(), request.Notes?.Trim());
        item.SetAudit(currentUserId);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(MapToResponse(item));
    }

    public async Task<Result> DeleteItemAsync(long id, string currentUserId, CancellationToken cancellationToken = default)
    {
        var item = await _dbContext.Items.FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
        if (item is null) return Result.Failure("Item not found.");

        item.SoftDelete(currentUserId);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> ActivateItemAsync(long id, string currentUserId, CancellationToken cancellationToken = default)
    {
        var item = await _dbContext.Items.FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
        if (item is null) return Result.Failure("Item not found.");
        item.Activate();
        item.SetAudit(currentUserId);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> DeactivateItemAsync(long id, string currentUserId, CancellationToken cancellationToken = default)
    {
        var item = await _dbContext.Items.FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
        if (item is null) return Result.Failure("Item not found.");
        item.Deactivate();
        item.SetAudit(currentUserId);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private static ItemResponse MapToResponse(Item i) => new(
        i.Id, i.Code, i.Name, i.Description,
        i.CategoryId, i.Category?.Name,
        i.UomId, i.Uom?.Code,
        (int)i.Type, i.Type.ToString(),
        (int)i.ValuationMethod, i.ValuationMethod.ToString(),
        i.StandardCost, i.SalePrice, i.ReorderLevel,
        i.Barcode, i.IsActive, i.Notes, i.ImagePath,
        i.CreatedAt, i.CreatedBy, i.UpdatedAt);
}
