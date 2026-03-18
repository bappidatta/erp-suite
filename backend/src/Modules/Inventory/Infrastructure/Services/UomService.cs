using ErpSuite.BuildingBlocks.Application.Common;
using ErpSuite.BuildingBlocks.Domain.Results;
using ErpSuite.Modules.Admin.Infrastructure.Persistence;
using ErpSuite.Modules.Inventory.Application.UOMs;
using ErpSuite.Modules.Inventory.Application.UOMs.Dtos;
using ErpSuite.Modules.Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ErpSuite.Modules.Inventory.Infrastructure.Services;

public sealed class UomService : IUomService
{
    private readonly ErpDbContext _dbContext;

    public UomService(ErpDbContext dbContext) => _dbContext = dbContext;

    public async Task<PagedResult<UomResponse>> GetUomsAsync(GetUomsQuery query, CancellationToken cancellationToken = default)
    {
        var queryable = _dbContext.UnitsOfMeasure.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var term = query.SearchTerm.ToLower();
            queryable = queryable.Where(u => u.Code.ToLower().Contains(term) || u.Name.ToLower().Contains(term));
        }

        if (query.IsActive.HasValue)
            queryable = queryable.Where(u => u.IsActive == query.IsActive.Value);

        queryable = queryable.OrderBy(u => u.Name);

        var totalCount = await queryable.CountAsync(cancellationToken);
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 200);

        var items = await queryable.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

        return new PagedResult<UomResponse>(items.Select(MapToResponse).ToList(), totalCount, page, pageSize);
    }

    public async Task<UomResponse?> GetUomByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var uom = await _dbContext.UnitsOfMeasure.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        return uom is null ? null : MapToResponse(uom);
    }

    public async Task<Result<UomResponse>> CreateUomAsync(CreateUomRequest request, string currentUserId, CancellationToken cancellationToken = default)
    {
        var normalizedCode = request.Code.Trim().ToUpperInvariant();

        if (await _dbContext.UnitsOfMeasure.AnyAsync(u => u.Code.ToUpper() == normalizedCode, cancellationToken))
            return Result.Failure<UomResponse>("A UOM with this code already exists.");

        var uom = UnitOfMeasure.Create(normalizedCode, request.Name.Trim(), request.Description?.Trim());
        uom.SetAudit(currentUserId);
        _dbContext.UnitsOfMeasure.Add(uom);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(MapToResponse(uom));
    }

    public async Task<Result<UomResponse>> UpdateUomAsync(long id, UpdateUomRequest request, string currentUserId, CancellationToken cancellationToken = default)
    {
        var uom = await _dbContext.UnitsOfMeasure.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        if (uom is null) return Result.Failure<UomResponse>("UOM not found.");

        uom.Update(request.Name.Trim(), request.Description?.Trim());
        uom.SetAudit(currentUserId);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(MapToResponse(uom));
    }

    public async Task<Result> DeleteUomAsync(long id, string currentUserId, CancellationToken cancellationToken = default)
    {
        var uom = await _dbContext.UnitsOfMeasure.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        if (uom is null) return Result.Failure("UOM not found.");

        var hasItems = await _dbContext.Items.AnyAsync(i => i.UomId == id, cancellationToken);
        if (hasItems) return Result.Failure("Cannot delete a UOM that is in use by items.");

        uom.SoftDelete(currentUserId);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> ActivateUomAsync(long id, string currentUserId, CancellationToken cancellationToken = default)
    {
        var uom = await _dbContext.UnitsOfMeasure.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        if (uom is null) return Result.Failure("UOM not found.");
        uom.Activate();
        uom.SetAudit(currentUserId);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> DeactivateUomAsync(long id, string currentUserId, CancellationToken cancellationToken = default)
    {
        var uom = await _dbContext.UnitsOfMeasure.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        if (uom is null) return Result.Failure("UOM not found.");
        uom.Deactivate();
        uom.SetAudit(currentUserId);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private static UomResponse MapToResponse(UnitOfMeasure u) => new(
        u.Id, u.Code, u.Name, u.Description, u.IsActive, u.CreatedAt, u.CreatedBy, u.UpdatedAt);
}
