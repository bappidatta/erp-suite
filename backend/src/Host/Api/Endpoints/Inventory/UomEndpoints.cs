using ErpSuite.Modules.Inventory.Application.UOMs;
using ErpSuite.Modules.Inventory.Application.UOMs.Dtos;
using Api.Filters;

namespace Api.Endpoints.Inventory;

public static class UomEndpoints
{
    public static IEndpointRouteBuilder MapUomEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/inventory/uoms")
            .WithTags("Inventory - Units of Measure")
            .RequireAuthorization("AuthenticatedUser");

        group.MapGet("", GetUoms);

        group.MapGet("{id:long}", GetUom)
            .WithName("GetUom");

        group.MapPost("", CreateUom)
            .AddEndpointFilter<ValidationFilter<CreateUomRequest>>();

        group.MapPut("{id:long}", UpdateUom)
            .AddEndpointFilter<ValidationFilter<UpdateUomRequest>>();

        group.MapDelete("{id:long}", DeleteUom);

        group.MapPost("{id:long}/activate", ActivateUom);

        group.MapPost("{id:long}/deactivate", DeactivateUom);

        return app;
    }

    private static async Task<IResult> GetUoms([AsParameters] GetUomsQuery query, IUomService uomService, CancellationToken cancellationToken)
    {
        var result = await uomService.GetUomsAsync(query, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetUom(long id, IUomService uomService, CancellationToken cancellationToken)
    {
        var uom = await uomService.GetUomByIdAsync(id, cancellationToken);
        return uom is null ? Results.NotFound(new { message = "Unit of measure not found." }) : Results.Ok(uom);
    }

    private static async Task<IResult> CreateUom(CreateUomRequest request, IUomService uomService, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await uomService.CreateUomAsync(request, currentUserId, cancellationToken);
        if (result.IsFailure) return Results.BadRequest(new { message = result.Error });
        return Results.CreatedAtRoute("GetUom", new { id = result.Value.Id }, result.Value);
    }

    private static async Task<IResult> UpdateUom(long id, UpdateUomRequest request, IUomService uomService, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await uomService.UpdateUomAsync(id, request, currentUserId, cancellationToken);
        if (result.IsFailure) return Results.BadRequest(new { message = result.Error });
        return Results.Ok(result.Value);
    }

    private static async Task<IResult> DeleteUom(long id, IUomService uomService, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await uomService.DeleteUomAsync(id, currentUserId, cancellationToken);
        if (result.IsFailure) return Results.BadRequest(new { message = result.Error });
        return Results.NoContent();
    }

    private static async Task<IResult> ActivateUom(long id, IUomService uomService, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await uomService.ActivateUomAsync(id, currentUserId, cancellationToken);
        if (result.IsFailure) return Results.BadRequest(new { message = result.Error });
        return Results.Ok(new { message = "Unit of measure activated successfully." });
    }

    private static async Task<IResult> DeactivateUom(long id, IUomService uomService, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await uomService.DeactivateUomAsync(id, currentUserId, cancellationToken);
        if (result.IsFailure) return Results.BadRequest(new { message = result.Error });
        return Results.Ok(new { message = "Unit of measure deactivated successfully." });
    }
}
