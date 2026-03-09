using ErpSuite.BuildingBlocks.Application.Common;
using ErpSuite.Modules.Admin.Application.Dashboard;
using ErpSuite.Modules.Admin.Application.Dashboard.Dtos;
using ErpSuite.Modules.Admin.Domain.Entities;
using ErpSuite.Modules.Admin.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ErpSuite.Modules.Admin.Infrastructure.Services;

public sealed class DashboardService : IDashboardService
{
    private readonly ErpDbContext _dbContext;

    public DashboardService(ErpDbContext dbContext) => _dbContext = dbContext;

    public async Task<DashboardStatsResponse> GetStatsAsync(CancellationToken cancellationToken = default)
    {
        var totalUsers = await _dbContext.Users.CountAsync(cancellationToken);
        var activeUsers = await _dbContext.Users.CountAsync(u => u.Status == UserStatus.Active, cancellationToken);
        var totalRoles = await _dbContext.Roles.CountAsync(cancellationToken);
        var totalPermissions = await _dbContext.Permissions.CountAsync(cancellationToken);

        var lastActivity = await _dbContext.AuditLogs
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => (DateTime?)a.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        return new DashboardStatsResponse(
            totalUsers,
            activeUsers,
            totalRoles,
            totalPermissions,
            lastActivity,
            "Healthy");
    }

    public async Task<PagedResult<AuditLogResponse>> GetAuditLogsAsync(GetAuditLogsQuery query, CancellationToken cancellationToken = default)
    {
        var queryable = _dbContext.AuditLogs
            .Join(_dbContext.Users.IgnoreQueryFilters().Select(u => new { u.Id, u.FirstName, u.LastName }),
                a => a.UserId,
                u => (long?)u.Id,
                (a, u) => new { AuditLog = a, UserName = u == null ? null : u.FirstName + " " + u.LastName })
            .AsQueryable();

        // Simpler approach without join to avoid nullable issues
        var logsQuery = _dbContext.AuditLogs.AsQueryable();

        if (!string.IsNullOrEmpty(query.Module))
            logsQuery = logsQuery.Where(a => a.Module == query.Module);

        if (query.UserId.HasValue)
            logsQuery = logsQuery.Where(a => a.UserId == query.UserId.Value);

        if (query.DateFrom.HasValue)
            logsQuery = logsQuery.Where(a => a.CreatedAt >= query.DateFrom.Value);

        if (query.DateTo.HasValue)
            logsQuery = logsQuery.Where(a => a.CreatedAt <= query.DateTo.Value);

        var totalCount = await logsQuery.CountAsync(cancellationToken);
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 200);

        var logs = await logsQuery
            .OrderByDescending(a => a.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var userIds = logs.Where(l => l.UserId.HasValue).Select(l => l.UserId!.Value).Distinct().ToList();
        var users = await _dbContext.Users.IgnoreQueryFilters()
            .Where(u => userIds.Contains(u.Id))
            .Select(u => new { u.Id, u.FirstName, u.LastName })
            .ToListAsync(cancellationToken);

        var userMap = users.ToDictionary(u => u.Id, u => $"{u.FirstName} {u.LastName}".Trim());

        var items = logs.Select(l => new AuditLogResponse(
            l.Id,
            l.UserId,
            l.UserId.HasValue ? userMap.GetValueOrDefault(l.UserId.Value) : null,
            l.Action,
            l.Module,
            l.EntityId,
            l.OldValues,
            l.NewValues,
            l.IpAddress,
            l.CreatedAt)).ToList();

        return new PagedResult<AuditLogResponse>(items, totalCount, page, pageSize);
    }
}
