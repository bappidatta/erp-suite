using ErpSuite.BuildingBlocks.Application.Common;
using ErpSuite.Modules.Admin.Application.Dashboard.Dtos;

namespace ErpSuite.Modules.Admin.Application.Dashboard;

public interface IDashboardService
{
    Task<DashboardStatsResponse> GetStatsAsync(CancellationToken cancellationToken = default);
    Task<PagedResult<AuditLogResponse>> GetAuditLogsAsync(GetAuditLogsQuery query, CancellationToken cancellationToken = default);
}
