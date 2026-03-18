using ErpSuite.Modules.Inventory.Application.Categories;
using ErpSuite.Modules.Inventory.Application.Categories.Dtos;
using Api.Filters;

namespace Api.Endpoints.Inventory;

public static class CategoryEndpoints
{
    public static IEndpointRouteBuilder MapCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/inventory/categories")
            .WithTags("Inventory - Categories")
            .RequireAuthorization("AuthenticatedUser");

        group.MapGet("", GetCategories);

        group.MapGet("{id:long}", GetCategory)
            .WithName("GetCategory");

        group.MapPost("", CreateCategory)
            .AddEndpointFilter<ValidationFilter<CreateCategoryRequest>>();

        group.MapPut("{id:long}", UpdateCategory)
            .AddEndpointFilter<ValidationFilter<UpdateCategoryRequest>>();

        group.MapDelete("{id:long}", DeleteCategory);

        group.MapPost("{id:long}/activate", ActivateCategory);

        group.MapPost("{id:long}/deactivate", DeactivateCategory);

        return app;
    }

    private static async Task<IResult> GetCategories([AsParameters] GetCategoriesQuery query, ICategoryService categoryService, CancellationToken cancellationToken)
    {
        var result = await categoryService.GetCategoriesAsync(query, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetCategory(long id, ICategoryService categoryService, CancellationToken cancellationToken)
    {
        var category = await categoryService.GetCategoryByIdAsync(id, cancellationToken);
        return category is null ? Results.NotFound(new { message = "Category not found." }) : Results.Ok(category);
    }

    private static async Task<IResult> CreateCategory(CreateCategoryRequest request, ICategoryService categoryService, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await categoryService.CreateCategoryAsync(request, currentUserId, cancellationToken);
        if (result.IsFailure) return Results.BadRequest(new { message = result.Error });
        return Results.CreatedAtRoute("GetCategory", new { id = result.Value.Id }, result.Value);
    }

    private static async Task<IResult> UpdateCategory(long id, UpdateCategoryRequest request, ICategoryService categoryService, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await categoryService.UpdateCategoryAsync(id, request, currentUserId, cancellationToken);
        if (result.IsFailure) return Results.BadRequest(new { message = result.Error });
        return Results.Ok(result.Value);
    }

    private static async Task<IResult> DeleteCategory(long id, ICategoryService categoryService, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await categoryService.DeleteCategoryAsync(id, currentUserId, cancellationToken);
        if (result.IsFailure) return Results.BadRequest(new { message = result.Error });
        return Results.NoContent();
    }

    private static async Task<IResult> ActivateCategory(long id, ICategoryService categoryService, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await categoryService.ActivateCategoryAsync(id, currentUserId, cancellationToken);
        if (result.IsFailure) return Results.BadRequest(new { message = result.Error });
        return Results.Ok(new { message = "Category activated successfully." });
    }

    private static async Task<IResult> DeactivateCategory(long id, ICategoryService categoryService, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await categoryService.DeactivateCategoryAsync(id, currentUserId, cancellationToken);
        if (result.IsFailure) return Results.BadRequest(new { message = result.Error });
        return Results.Ok(new { message = "Category deactivated successfully." });
    }
}
