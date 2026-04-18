using Api.Filters;
using ErpSuite.Modules.Finance.Application.FinancialPeriods;
using ErpSuite.Modules.Finance.Application.FinancialPeriods.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.Finance;

public static class FinancialPeriodEndpoints
{
    public static IEndpointRouteBuilder MapFinancialPeriodEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/finance/financial-periods")
            .WithTags("Finance - Financial Periods")
            .RequireAuthorization("AuthenticatedUser");

        group.MapGet("", GetFinancialPeriods);
        group.MapGet("{id:long}", GetFinancialPeriod).WithName("GetFinancialPeriod");
        group.MapPost("", CreateFinancialPeriod).AddEndpointFilter<ValidationFilter<CreateFinancialPeriodRequest>>();
        group.MapPut("{id:long}", UpdateFinancialPeriod).AddEndpointFilter<ValidationFilter<UpdateFinancialPeriodRequest>>();
        group.MapDelete("{id:long}", DeleteFinancialPeriod);
        group.MapPost("{id:long}/close", CloseFinancialPeriod);
        group.MapPost("{id:long}/reopen", ReopenFinancialPeriod);

        return app;
    }

    private static async Task<IResult> GetFinancialPeriods([AsParameters] GetFinancialPeriodsQuery query, IFinancialPeriodService service, CancellationToken cancellationToken)
    {
        var result = await service.GetFinancialPeriodsAsync(query, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetFinancialPeriod(long id, IFinancialPeriodService service, CancellationToken cancellationToken)
    {
        var result = await service.GetFinancialPeriodByIdAsync(id, cancellationToken);
        return result is null ? Results.NotFound(new { message = "Financial period not found." }) : Results.Ok(result);
    }

    private static async Task<IResult> CreateFinancialPeriod(CreateFinancialPeriodRequest request, IFinancialPeriodService service, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await service.CreateFinancialPeriodAsync(request, currentUserId, cancellationToken);
        return result.IsFailure
            ? Results.BadRequest(new { message = result.Error })
            : Results.CreatedAtRoute("GetFinancialPeriod", new { id = result.Value.Id }, result.Value);
    }

    private static async Task<IResult> UpdateFinancialPeriod(long id, UpdateFinancialPeriodRequest request, IFinancialPeriodService service, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await service.UpdateFinancialPeriodAsync(id, request, currentUserId, cancellationToken);
        return result.IsFailure ? Results.BadRequest(new { message = result.Error }) : Results.Ok(result.Value);
    }

    private static async Task<IResult> DeleteFinancialPeriod(long id, IFinancialPeriodService service, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await service.DeleteFinancialPeriodAsync(id, currentUserId, cancellationToken);
        return result.IsFailure ? Results.BadRequest(new { message = result.Error }) : Results.NoContent();
    }

    private static async Task<IResult> CloseFinancialPeriod(long id, IFinancialPeriodService service, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await service.CloseFinancialPeriodAsync(id, currentUserId, cancellationToken);
        return result.IsFailure ? Results.BadRequest(new { message = result.Error }) : Results.Ok(result.Value);
    }

    private static async Task<IResult> ReopenFinancialPeriod(long id, IFinancialPeriodService service, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await service.ReopenFinancialPeriodAsync(id, currentUserId, cancellationToken);
        return result.IsFailure ? Results.BadRequest(new { message = result.Error }) : Results.Ok(result.Value);
    }
}
