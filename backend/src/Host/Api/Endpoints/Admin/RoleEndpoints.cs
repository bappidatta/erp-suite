using ErpSuite.Modules.Admin.Application.Roles;
using ErpSuite.Modules.Admin.Application.Roles.Dtos;
using Api.Filters;

namespace Api.Endpoints.Admin;

public static class RoleEndpoints
{
    public static IEndpointRouteBuilder MapRoleEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/admin/roles")
            .WithTags("Admin - Roles")
            .RequireAuthorization("AdminOnly");

        group.MapGet("", GetRoles);

        group.MapGet("permissions", GetAllPermissions);

        group.MapGet("{id:long}", GetRole)
            .WithName("GetRole");

        group.MapPost("", CreateRole)
            .AddEndpointFilter<ValidationFilter<CreateRoleRequest>>();

        group.MapPut("{id:long}", UpdateRole)
            .AddEndpointFilter<ValidationFilter<UpdateRoleRequest>>();

        group.MapDelete("{id:long}", DeleteRole);

        group.MapGet("{id:long}/permissions", GetRolePermissions);

        group.MapPost("{id:long}/permissions", AssignPermissions)
            .AddEndpointFilter<ValidationFilter<AssignPermissionsRequest>>();

        return app;
    }

    private static async Task<IResult> GetRoles(IRoleService roleService, CancellationToken cancellationToken)
    {
        var roles = await roleService.GetRolesAsync(cancellationToken);
        return Results.Ok(roles);
    }

    private static async Task<IResult> GetRole(long id, IRoleService roleService, CancellationToken cancellationToken)
    {
        var role = await roleService.GetRoleByIdAsync(id, cancellationToken);
        return role is null ? Results.NotFound(new { message = "Role not found." }) : Results.Ok(role);
    }

    private static async Task<IResult> CreateRole(CreateRoleRequest request, IRoleService roleService, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await roleService.CreateRoleAsync(request, currentUserId, cancellationToken);
        if (result.IsFailure) return Results.BadRequest(new { message = result.Error });
        return Results.CreatedAtRoute("GetRole", new { id = result.Value.Id }, result.Value);
    }

    private static async Task<IResult> UpdateRole(long id, UpdateRoleRequest request, IRoleService roleService, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await roleService.UpdateRoleAsync(id, request, currentUserId, cancellationToken);
        if (result.IsFailure) return Results.BadRequest(new { message = result.Error });
        return Results.Ok(result.Value);
    }

    private static async Task<IResult> DeleteRole(long id, IRoleService roleService, CancellationToken cancellationToken)
    {
        var result = await roleService.DeleteRoleAsync(id, cancellationToken);
        if (result.IsFailure) return Results.BadRequest(new { message = result.Error });
        return Results.NoContent();
    }

    private static async Task<IResult> GetRolePermissions(long id, IRoleService roleService, CancellationToken cancellationToken)
    {
        var role = await roleService.GetRoleByIdAsync(id, cancellationToken);
        if (role is null) return Results.NotFound(new { message = "Role not found." });
        return Results.Ok(role.Permissions);
    }

    private static async Task<IResult> AssignPermissions(long id, AssignPermissionsRequest request, IRoleService roleService, CancellationToken cancellationToken)
    {
        var result = await roleService.AssignPermissionsAsync(id, request, cancellationToken);
        if (result.IsFailure) return Results.BadRequest(new { message = result.Error });
        return Results.Ok(new { message = "Permissions assigned successfully." });
    }

    private static async Task<IResult> GetAllPermissions(IRoleService roleService, CancellationToken cancellationToken)
    {
        var permissions = await roleService.GetPermissionsAsync(cancellationToken);
        return Results.Ok(permissions);
    }
}
