using ErpSuite.Modules.Finance.Application.Reporting;
using ErpSuite.Modules.Finance.Application.Reporting.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.Finance;

public static class GeneralLedgerEndpoints
{
    public static IEndpointRouteBuilder MapGeneralLedgerEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/finance")
            .WithTags("Finance - Reports")
            .RequireAuthorization("AuthenticatedUser");

        group.MapGet("trial-balance", GetTrialBalance);
        group.MapGet("ledger", GetLedgerEntries);

        return app;
    }

    private static async Task<IResult> GetTrialBalance([AsParameters] GetTrialBalanceQuery query, IGeneralLedgerReportService service, CancellationToken cancellationToken)
    {
        var result = await service.GetTrialBalanceAsync(query, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetLedgerEntries([AsParameters] GetLedgerEntriesQuery query, IGeneralLedgerReportService service, CancellationToken cancellationToken)
    {
        if (query.AccountId <= 0)
        {
            return Results.BadRequest(new { message = "A valid accountId is required." });
        }

        var result = await service.GetLedgerEntriesAsync(query, cancellationToken);
        return Results.Ok(result);
    }
}
