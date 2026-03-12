using ErpSuite.Modules.Admin.Application.Dashboard;
using ErpSuite.Modules.Admin.Application.Dashboard.Dtos;

namespace Api.Endpoints.Admin;

public static class AdminDashboardEndpoints
{
    public static IEndpointRouteBuilder MapAdminDashboardEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/admin")
            .WithTags("Admin - Dashboard")
            .RequireAuthorization("AdminOnly");

        group.MapGet("dashboard/stats", GetStats);

        group.MapGet("audit-logs", GetAuditLogs);

        return app;
    }

    private static async Task<IResult> GetStats(IDashboardService dashboardService, CancellationToken cancellationToken)
    {
        var stats = await dashboardService.GetStatsAsync(cancellationToken);
        return Results.Ok(stats);
    }

    private static async Task<IResult> GetAuditLogs([AsParameters] GetAuditLogsQuery query, IDashboardService dashboardService, CancellationToken cancellationToken)
    {
        var result = await dashboardService.GetAuditLogsAsync(query, cancellationToken);
        return Results.Ok(result);
    }
}
