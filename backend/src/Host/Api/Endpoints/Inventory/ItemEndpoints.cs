using ErpSuite.Modules.Inventory.Application.Items;
using ErpSuite.Modules.Inventory.Application.Items.Dtos;
using Api.Filters;

namespace Api.Endpoints.Inventory;

public static class ItemEndpoints
{
    public static IEndpointRouteBuilder MapItemEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/inventory/items")
            .WithTags("Inventory - Items")
            .RequireAuthorization("AuthenticatedUser");

        group.MapGet("", GetItems);

        group.MapGet("{id:long}", GetItem)
            .WithName("GetItem");

        group.MapPost("", CreateItem)
            .AddEndpointFilter<ValidationFilter<CreateItemRequest>>();

        group.MapPut("{id:long}", UpdateItem)
            .AddEndpointFilter<ValidationFilter<UpdateItemRequest>>();

        group.MapDelete("{id:long}", DeleteItem);

        group.MapPost("{id:long}/activate", ActivateItem);

        group.MapPost("{id:long}/deactivate", DeactivateItem);

        return app;
    }

    private static async Task<IResult> GetItems([AsParameters] GetItemsQuery query, IItemService itemService, CancellationToken cancellationToken)
    {
        var result = await itemService.GetItemsAsync(query, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetItem(long id, IItemService itemService, CancellationToken cancellationToken)
    {
        var item = await itemService.GetItemByIdAsync(id, cancellationToken);
        return item is null ? Results.NotFound(new { message = "Item not found." }) : Results.Ok(item);
    }

    private static async Task<IResult> CreateItem(CreateItemRequest request, IItemService itemService, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await itemService.CreateItemAsync(request, currentUserId, cancellationToken);
        if (result.IsFailure) return Results.BadRequest(new { message = result.Error });
        return Results.CreatedAtRoute("GetItem", new { id = result.Value.Id }, result.Value);
    }

    private static async Task<IResult> UpdateItem(long id, UpdateItemRequest request, IItemService itemService, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await itemService.UpdateItemAsync(id, request, currentUserId, cancellationToken);
        if (result.IsFailure) return Results.BadRequest(new { message = result.Error });
        return Results.Ok(result.Value);
    }

    private static async Task<IResult> DeleteItem(long id, IItemService itemService, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await itemService.DeleteItemAsync(id, currentUserId, cancellationToken);
        if (result.IsFailure) return Results.BadRequest(new { message = result.Error });
        return Results.NoContent();
    }

    private static async Task<IResult> ActivateItem(long id, IItemService itemService, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await itemService.ActivateItemAsync(id, currentUserId, cancellationToken);
        if (result.IsFailure) return Results.BadRequest(new { message = result.Error });
        return Results.Ok(new { message = "Item activated successfully." });
    }

    private static async Task<IResult> DeactivateItem(long id, IItemService itemService, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await itemService.DeactivateItemAsync(id, currentUserId, cancellationToken);
        if (result.IsFailure) return Results.BadRequest(new { message = result.Error });
        return Results.Ok(new { message = "Item deactivated successfully." });
    }
}
