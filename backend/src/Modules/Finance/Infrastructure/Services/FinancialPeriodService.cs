using ErpSuite.BuildingBlocks.Application.Common;
using ErpSuite.BuildingBlocks.Domain.Results;
using ErpSuite.Modules.Admin.Infrastructure.Persistence;
using ErpSuite.Modules.Finance.Application.FinancialPeriods;
using ErpSuite.Modules.Finance.Application.FinancialPeriods.Dtos;
using ErpSuite.Modules.Finance.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ErpSuite.Modules.Finance.Infrastructure.Services;

public sealed class FinancialPeriodService : IFinancialPeriodService
{
    private readonly ErpDbContext _dbContext;

    public FinancialPeriodService(ErpDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<FinancialPeriodResponse>> GetFinancialPeriodsAsync(GetFinancialPeriodsQuery query, CancellationToken cancellationToken = default)
    {
        var queryable = _dbContext.FinancialPeriods.AsNoTracking().AsQueryable();

        if (query.Year.HasValue)
        {
            queryable = queryable.Where(x => x.StartDate.Year == query.Year.Value || x.EndDate.Year == query.Year.Value);
        }

        if (query.Status.HasValue)
        {
            queryable = queryable.Where(x => x.Status == query.Status.Value);
        }

        queryable = query.SortBy?.ToLowerInvariant() switch
        {
            "name" => query.SortDescending ? queryable.OrderByDescending(x => x.Name) : queryable.OrderBy(x => x.Name),
            _ => query.SortDescending ? queryable.OrderByDescending(x => x.StartDate) : queryable.OrderBy(x => x.StartDate)
        };

        var totalCount = await queryable.CountAsync(cancellationToken);
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);
        var items = await queryable.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

        return new PagedResult<FinancialPeriodResponse>(
            items.Select(MapToResponse).ToList(),
            totalCount,
            page,
            pageSize);
    }

    public async Task<FinancialPeriodResponse?> GetFinancialPeriodByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var period = await _dbContext.FinancialPeriods.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return period is null ? null : MapToResponse(period);
    }

    public async Task<Result<FinancialPeriodResponse>> CreateFinancialPeriodAsync(CreateFinancialPeriodRequest request, string currentUserId, CancellationToken cancellationToken = default)
    {
        var overlap = await HasOverlapAsync(request.StartDate, request.EndDate, null, cancellationToken);
        if (overlap)
        {
            return Result.Failure<FinancialPeriodResponse>("Financial period overlaps with an existing period.");
        }

        var period = FinancialPeriod.Create(request.Name.Trim(), request.StartDate, request.EndDate);
        period.SetAudit(currentUserId);
        _dbContext.FinancialPeriods.Add(period);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(MapToResponse(period));
    }

    public async Task<Result<FinancialPeriodResponse>> UpdateFinancialPeriodAsync(long id, UpdateFinancialPeriodRequest request, string currentUserId, CancellationToken cancellationToken = default)
    {
        var period = await _dbContext.FinancialPeriods.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (period is null)
        {
            return Result.Failure<FinancialPeriodResponse>("Financial period not found.");
        }

        if (period.Status == FinancialPeriodStatus.Closed)
        {
            return Result.Failure<FinancialPeriodResponse>("Closed periods cannot be edited.");
        }

        var overlap = await HasOverlapAsync(request.StartDate, request.EndDate, id, cancellationToken);
        if (overlap)
        {
            return Result.Failure<FinancialPeriodResponse>("Financial period overlaps with an existing period.");
        }

        period.Update(request.Name.Trim(), request.StartDate, request.EndDate);
        period.SetAudit(currentUserId);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(MapToResponse(period));
    }

    public async Task<Result> DeleteFinancialPeriodAsync(long id, string currentUserId, CancellationToken cancellationToken = default)
    {
        var period = await _dbContext.FinancialPeriods.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (period is null)
        {
            return Result.Failure("Financial period not found.");
        }

        if (period.Status == FinancialPeriodStatus.Closed)
        {
            return Result.Failure("Closed periods cannot be deleted.");
        }

        period.SoftDelete(currentUserId);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result<FinancialPeriodResponse>> CloseFinancialPeriodAsync(long id, string currentUserId, CancellationToken cancellationToken = default)
    {
        var period = await _dbContext.FinancialPeriods.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (period is null)
        {
            return Result.Failure<FinancialPeriodResponse>("Financial period not found.");
        }

        period.Close(currentUserId);
        period.SetAudit(currentUserId);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success(MapToResponse(period));
    }

    public async Task<Result<FinancialPeriodResponse>> ReopenFinancialPeriodAsync(long id, string currentUserId, CancellationToken cancellationToken = default)
    {
        var period = await _dbContext.FinancialPeriods.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (period is null)
        {
            return Result.Failure<FinancialPeriodResponse>("Financial period not found.");
        }

        period.Reopen();
        period.SetAudit(currentUserId);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success(MapToResponse(period));
    }

    private async Task<bool> HasOverlapAsync(DateTime startDate, DateTime endDate, long? excludeId, CancellationToken cancellationToken)
    {
        var start = startDate.Date;
        var end = endDate.Date;

        return await _dbContext.FinancialPeriods.AnyAsync(
            x => x.Id != excludeId &&
                x.StartDate <= end &&
                x.EndDate >= start,
            cancellationToken);
    }

    private static FinancialPeriodResponse MapToResponse(FinancialPeriod period) => new(
        period.Id,
        period.Name,
        period.StartDate,
        period.EndDate,
        period.Status,
        period.Status.ToString(),
        period.ClosedAt,
        period.ClosedBy,
        period.CreatedAt,
        period.CreatedBy,
        period.UpdatedAt);
}
