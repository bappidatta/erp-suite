using ErpSuite.BuildingBlocks.Domain.Results;
using ErpSuite.Modules.Admin.Infrastructure.Persistence;
using ErpSuite.BuildingBlocks.Application.Common;
using ErpSuite.Modules.Finance.Application.TaxCodes;
using ErpSuite.Modules.Finance.Application.TaxCodes.Dtos;
using ErpSuite.Modules.Finance.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ErpSuite.Modules.Finance.Infrastructure.Services;

public sealed class TaxCodeService : ITaxCodeService
{
    private readonly ErpDbContext _dbContext;

    public TaxCodeService(ErpDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<TaxCodeResponse>> GetTaxCodesAsync(GetTaxCodesQuery query, CancellationToken cancellationToken = default)
    {
        var queryable = _dbContext.TaxCodes.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var term = query.SearchTerm.ToLower();
            queryable = queryable.Where(t =>
                t.Code.ToLower().Contains(term) ||
                t.Name.ToLower().Contains(term));
        }

        if (query.IsActive.HasValue)
            queryable = queryable.Where(t => t.IsActive == query.IsActive.Value);

        if (query.AppliesToSales.HasValue)
            queryable = queryable.Where(t => t.AppliesToSales == query.AppliesToSales.Value);

        if (query.AppliesToPurchases.HasValue)
            queryable = queryable.Where(t => t.AppliesToPurchases == query.AppliesToPurchases.Value);

        queryable = query.SortBy?.ToLower() switch
        {
            "name" => query.SortDescending ? queryable.OrderByDescending(t => t.Name) : queryable.OrderBy(t => t.Name),
            "rate" => query.SortDescending ? queryable.OrderByDescending(t => t.Rate) : queryable.OrderBy(t => t.Rate),
            _ => query.SortDescending ? queryable.OrderByDescending(t => t.Code) : queryable.OrderBy(t => t.Code)
        };

        var totalCount = await queryable.CountAsync(cancellationToken);
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var items = await queryable
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<TaxCodeResponse>(
            items.Select(MapToResponse).ToList(),
            totalCount, page, pageSize);
    }

    public async Task<TaxCodeResponse?> GetTaxCodeByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var taxCode = await _dbContext.TaxCodes.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        return taxCode is null ? null : MapToResponse(taxCode);
    }

    public async Task<Result<TaxCodeResponse>> CreateTaxCodeAsync(CreateTaxCodeRequest request, string currentUserId, CancellationToken cancellationToken = default)
    {
        var normalizedCode = request.Code.Trim().ToUpperInvariant();

        if (await _dbContext.TaxCodes.AnyAsync(t => t.Code.ToUpper() == normalizedCode, cancellationToken))
            return Result.Failure<TaxCodeResponse>("A tax code with this code already exists.");

        var taxCode = TaxCode.Create(normalizedCode, request.Name, request.Rate, request.Type,
            request.Description, request.AppliesToSales, request.AppliesToPurchases);

        taxCode.SetAudit(currentUserId);
        _dbContext.TaxCodes.Add(taxCode);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(MapToResponse(taxCode));
    }

    public async Task<Result<TaxCodeResponse>> UpdateTaxCodeAsync(long id, UpdateTaxCodeRequest request, string currentUserId, CancellationToken cancellationToken = default)
    {
        var taxCode = await _dbContext.TaxCodes.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        if (taxCode is null)
            return Result.Failure<TaxCodeResponse>("Tax code not found.");

        taxCode.Update(request.Name, request.Rate, request.Type, request.Description,
            request.AppliesToSales, request.AppliesToPurchases);

        taxCode.SetAudit(currentUserId);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(MapToResponse(taxCode));
    }

    public async Task<Result> DeleteTaxCodeAsync(long id, string currentUserId, CancellationToken cancellationToken = default)
    {
        var taxCode = await _dbContext.TaxCodes.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        if (taxCode is null)
            return Result.Failure("Tax code not found.");

        taxCode.SoftDelete(currentUserId);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> ActivateTaxCodeAsync(long id, string currentUserId, CancellationToken cancellationToken = default)
    {
        var taxCode = await _dbContext.TaxCodes.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        if (taxCode is null) return Result.Failure("Tax code not found.");

        taxCode.Activate();
        taxCode.SetAudit(currentUserId);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> DeactivateTaxCodeAsync(long id, string currentUserId, CancellationToken cancellationToken = default)
    {
        var taxCode = await _dbContext.TaxCodes.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        if (taxCode is null) return Result.Failure("Tax code not found.");

        taxCode.Deactivate();
        taxCode.SetAudit(currentUserId);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private static TaxCodeResponse MapToResponse(TaxCode t) => new(
        t.Id, t.Code, t.Name, t.Rate, t.Type, t.Type.ToString(),
        t.Description, t.AppliesToSales, t.AppliesToPurchases, t.IsActive,
        t.CreatedAt, t.CreatedBy, t.UpdatedAt);
}
