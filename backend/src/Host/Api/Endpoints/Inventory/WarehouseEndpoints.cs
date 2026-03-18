using ErpSuite.Modules.Inventory.Application.Warehouses;
using ErpSuite.Modules.Inventory.Application.Warehouses.Dtos;
using Api.Filters;

namespace Api.Endpoints.Inventory;

public static class WarehouseEndpoints
{
    public static IEndpointRouteBuilder MapWarehouseEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/inventory/warehouses")
            .WithTags("Inventory - Warehouses")
            .RequireAuthorization("AuthenticatedUser");

        group.MapGet("", GetWarehouses);

        group.MapGet("{id:long}", GetWarehouse)
            .WithName("GetWarehouse");

        group.MapPost("", CreateWarehouse)
            .AddEndpointFilter<ValidationFilter<CreateWarehouseRequest>>();

        group.MapPut("{id:long}", UpdateWarehouse)
            .AddEndpointFilter<ValidationFilter<UpdateWarehouseRequest>>();

        group.MapDelete("{id:long}", DeleteWarehouse);

        group.MapPost("{id:long}/activate", ActivateWarehouse);

        group.MapPost("{id:long}/deactivate", DeactivateWarehouse);

        return app;
    }

    private static async Task<IResult> GetWarehouses([AsParameters] GetWarehousesQuery query, IWarehouseService warehouseService, CancellationToken cancellationToken)
    {
        var result = await warehouseService.GetWarehousesAsync(query, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetWarehouse(long id, IWarehouseService warehouseService, CancellationToken cancellationToken)
    {
        var warehouse = await warehouseService.GetWarehouseByIdAsync(id, cancellationToken);
        return warehouse is null ? Results.NotFound(new { message = "Warehouse not found." }) : Results.Ok(warehouse);
    }

    private static async Task<IResult> CreateWarehouse(CreateWarehouseRequest request, IWarehouseService warehouseService, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await warehouseService.CreateWarehouseAsync(request, currentUserId, cancellationToken);
        if (result.IsFailure) return Results.BadRequest(new { message = result.Error });
        return Results.CreatedAtRoute("GetWarehouse", new { id = result.Value.Id }, result.Value);
    }

    private static async Task<IResult> UpdateWarehouse(long id, UpdateWarehouseRequest request, IWarehouseService warehouseService, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await warehouseService.UpdateWarehouseAsync(id, request, currentUserId, cancellationToken);
        if (result.IsFailure) return Results.BadRequest(new { message = result.Error });
        return Results.Ok(result.Value);
    }

    private static async Task<IResult> DeleteWarehouse(long id, IWarehouseService warehouseService, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await warehouseService.DeleteWarehouseAsync(id, currentUserId, cancellationToken);
        if (result.IsFailure) return Results.BadRequest(new { message = result.Error });
        return Results.NoContent();
    }

    private static async Task<IResult> ActivateWarehouse(long id, IWarehouseService warehouseService, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await warehouseService.ActivateWarehouseAsync(id, currentUserId, cancellationToken);
        if (result.IsFailure) return Results.BadRequest(new { message = result.Error });
        return Results.Ok(new { message = "Warehouse activated successfully." });
    }

    private static async Task<IResult> DeactivateWarehouse(long id, IWarehouseService warehouseService, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await warehouseService.DeactivateWarehouseAsync(id, currentUserId, cancellationToken);
        if (result.IsFailure) return Results.BadRequest(new { message = result.Error });
        return Results.Ok(new { message = "Warehouse deactivated successfully." });
    }
}
