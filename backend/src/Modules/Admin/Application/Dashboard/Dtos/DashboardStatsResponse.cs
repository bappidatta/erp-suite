namespace ErpSuite.Modules.Admin.Application.Dashboard.Dtos;

public record DashboardStatsResponse(
    int TotalUsers,
    int ActiveUsers,
    int TotalRoles,
    int TotalPermissions,
    DateTime? LastActivity,
    string SystemHealth);
