namespace ErpSuite.Modules.Admin.Application.Dashboard.Dtos;

public record DashboardStatsResponse(
    int TotalUsers,
    int ActiveUsers,
    int TotalRoles,
    int TotalPermissions,
    DateTime? LastActivity,
    string SystemHealth);

public record AuditLogResponse(
    long Id,
    long? UserId,
    string? UserName,
    string Action,
    string Module,
    string? EntityId,
    string? OldValues,
    string? NewValues,
    string? IpAddress,
    DateTime CreatedAt);

public class GetAuditLogsQuery
{
    public string? Module { get; init; }
    public long? UserId { get; init; }
    public DateTime? DateFrom { get; init; }
    public DateTime? DateTo { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 50;
}
