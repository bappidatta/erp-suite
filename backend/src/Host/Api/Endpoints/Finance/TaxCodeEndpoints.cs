using ErpSuite.Modules.Finance.Application.TaxCodes;
using ErpSuite.Modules.Finance.Application.TaxCodes.Dtos;
using Api.Filters;

namespace Api.Endpoints.Finance;

public static class TaxCodeEndpoints
{
    public static IEndpointRouteBuilder MapTaxCodeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/finance/tax-codes")
            .WithTags("Finance - Tax Codes")
            .RequireAuthorization("AuthenticatedUser");

        group.MapGet("", GetTaxCodes);

        group.MapGet("{id:long}", GetTaxCode)
            .WithName("GetTaxCode");

        group.MapPost("", CreateTaxCode)
            .AddEndpointFilter<ValidationFilter<CreateTaxCodeRequest>>();

        group.MapPut("{id:long}", UpdateTaxCode)
            .AddEndpointFilter<ValidationFilter<UpdateTaxCodeRequest>>();

        group.MapDelete("{id:long}", DeleteTaxCode);

        group.MapPost("{id:long}/activate", ActivateTaxCode);

        group.MapPost("{id:long}/deactivate", DeactivateTaxCode);

        return app;
    }

    private static async Task<IResult> GetTaxCodes([AsParameters] GetTaxCodesQuery query, ITaxCodeService taxCodeService, CancellationToken cancellationToken)
    {
        var result = await taxCodeService.GetTaxCodesAsync(query, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetTaxCode(long id, ITaxCodeService taxCodeService, CancellationToken cancellationToken)
    {
        var taxCode = await taxCodeService.GetTaxCodeByIdAsync(id, cancellationToken);
        return taxCode is null ? Results.NotFound(new { message = "Tax code not found." }) : Results.Ok(taxCode);
    }

    private static async Task<IResult> CreateTaxCode(CreateTaxCodeRequest request, ITaxCodeService taxCodeService, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await taxCodeService.CreateTaxCodeAsync(request, currentUserId, cancellationToken);
        if (result.IsFailure) return Results.BadRequest(new { message = result.Error });
        return Results.CreatedAtRoute("GetTaxCode", new { id = result.Value.Id }, result.Value);
    }

    private static async Task<IResult> UpdateTaxCode(long id, UpdateTaxCodeRequest request, ITaxCodeService taxCodeService, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await taxCodeService.UpdateTaxCodeAsync(id, request, currentUserId, cancellationToken);
        if (result.IsFailure) return Results.BadRequest(new { message = result.Error });
        return Results.Ok(result.Value);
    }

    private static async Task<IResult> DeleteTaxCode(long id, ITaxCodeService taxCodeService, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await taxCodeService.DeleteTaxCodeAsync(id, currentUserId, cancellationToken);
        if (result.IsFailure) return Results.BadRequest(new { message = result.Error });
        return Results.NoContent();
    }

    private static async Task<IResult> ActivateTaxCode(long id, ITaxCodeService taxCodeService, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await taxCodeService.ActivateTaxCodeAsync(id, currentUserId, cancellationToken);
        if (result.IsFailure) return Results.BadRequest(new { message = result.Error });
        return Results.Ok(new { message = "Tax code activated successfully." });
    }

    private static async Task<IResult> DeactivateTaxCode(long id, ITaxCodeService taxCodeService, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await taxCodeService.DeactivateTaxCodeAsync(id, currentUserId, cancellationToken);
        if (result.IsFailure) return Results.BadRequest(new { message = result.Error });
        return Results.Ok(new { message = "Tax code deactivated successfully." });
    }
}
