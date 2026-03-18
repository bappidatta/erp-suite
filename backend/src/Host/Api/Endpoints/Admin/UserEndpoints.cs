using ErpSuite.Modules.Admin.Application.Users;
using ErpSuite.Modules.Admin.Application.Users.Dtos;
using Api.Filters;

namespace Api.Endpoints.Admin;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/admin/users")
            .WithTags("Admin - Users")
            .RequireAuthorization("AdminOnly");

        group.MapGet("", GetUsers);

        group.MapGet("{id:long}", GetUser)
            .WithName("GetUser");

        group.MapPost("", CreateUser)
            .AddEndpointFilter<ValidationFilter<CreateUserRequest>>();

        group.MapPut("{id:long}", UpdateUser)
            .AddEndpointFilter<ValidationFilter<UpdateUserRequest>>();

        group.MapDelete("{id:long}", DeleteUser);

        group.MapPost("{id:long}/activate", ActivateUser);

        group.MapPost("{id:long}/deactivate", DeactivateUser);

        return app;
    }

    private static async Task<IResult> GetUsers([AsParameters] GetUsersQuery query, IUserService userService, CancellationToken cancellationToken)
    {
        var result = await userService.GetUsersAsync(query, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetUser(long id, IUserService userService, CancellationToken cancellationToken)
    {
        var user = await userService.GetUserByIdAsync(id, cancellationToken);
        return user is null ? Results.NotFound(new { message = "User not found." }) : Results.Ok(user);
    }

    private static async Task<IResult> CreateUser(CreateUserRequest request, IUserService userService, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await userService.CreateUserAsync(request, currentUserId, cancellationToken);
        if (result.IsFailure) return Results.BadRequest(new { message = result.Error });
        return Results.CreatedAtRoute("GetUser", new { id = result.Value.Id }, result.Value);
    }

    private static async Task<IResult> UpdateUser(long id, UpdateUserRequest request, IUserService userService, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await userService.UpdateUserAsync(id, request, currentUserId, cancellationToken);
        if (result.IsFailure) return Results.BadRequest(new { message = result.Error });
        return Results.Ok(result.Value);
    }

    private static async Task<IResult> DeleteUser(long id, IUserService userService, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await userService.DeleteUserAsync(id, currentUserId, cancellationToken);
        if (result.IsFailure) return Results.BadRequest(new { message = result.Error });
        return Results.NoContent();
    }

    private static async Task<IResult> ActivateUser(long id, IUserService userService, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await userService.ActivateUserAsync(id, currentUserId, cancellationToken);
        if (result.IsFailure) return Results.BadRequest(new { message = result.Error });
        return Results.Ok(new { message = "User activated successfully." });
    }

    private static async Task<IResult> DeactivateUser(long id, IUserService userService, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await userService.DeactivateUserAsync(id, currentUserId, cancellationToken);
        if (result.IsFailure) return Results.BadRequest(new { message = result.Error });
        return Results.Ok(new { message = "User deactivated successfully." });
    }
}
