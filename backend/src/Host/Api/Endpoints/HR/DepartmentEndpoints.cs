using ErpSuite.Modules.HR.Application.Departments;
using ErpSuite.Modules.HR.Application.Departments.Dtos;
using Api.Filters;

namespace Api.Endpoints.HR;

public static class DepartmentEndpoints
{
    public static IEndpointRouteBuilder MapDepartmentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/hr/departments")
            .WithTags("HR - Departments")
            .RequireAuthorization("AuthenticatedUser");

        group.MapGet("", GetDepartments);

        group.MapGet("{id:long}", GetDepartment)
            .WithName("GetDepartment");

        group.MapPost("", CreateDepartment)
            .AddEndpointFilter<ValidationFilter<CreateDepartmentRequest>>();

        group.MapPut("{id:long}", UpdateDepartment)
            .AddEndpointFilter<ValidationFilter<UpdateDepartmentRequest>>();

        group.MapDelete("{id:long}", DeleteDepartment);

        group.MapPost("{id:long}/activate", ActivateDepartment);

        group.MapPost("{id:long}/deactivate", DeactivateDepartment);

        return app;
    }

    private static async Task<IResult> GetDepartments([AsParameters] GetDepartmentsQuery query, IDepartmentService departmentService, CancellationToken cancellationToken)
    {
        var result = await departmentService.GetDepartmentsAsync(query, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetDepartment(long id, IDepartmentService departmentService, CancellationToken cancellationToken)
    {
        var department = await departmentService.GetDepartmentByIdAsync(id, cancellationToken);
        return department is null ? Results.NotFound(new { message = "Department not found." }) : Results.Ok(department);
    }

    private static async Task<IResult> CreateDepartment(CreateDepartmentRequest request, IDepartmentService departmentService, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await departmentService.CreateDepartmentAsync(request, currentUserId, cancellationToken);
        if (result.IsFailure) return Results.BadRequest(new { message = result.Error });
        return Results.CreatedAtRoute("GetDepartment", new { id = result.Value.Id }, result.Value);
    }

    private static async Task<IResult> UpdateDepartment(long id, UpdateDepartmentRequest request, IDepartmentService departmentService, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await departmentService.UpdateDepartmentAsync(id, request, currentUserId, cancellationToken);
        if (result.IsFailure) return Results.BadRequest(new { message = result.Error });
        return Results.Ok(result.Value);
    }

    private static async Task<IResult> DeleteDepartment(long id, IDepartmentService departmentService, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await departmentService.DeleteDepartmentAsync(id, currentUserId, cancellationToken);
        if (result.IsFailure) return Results.BadRequest(new { message = result.Error });
        return Results.NoContent();
    }

    private static async Task<IResult> ActivateDepartment(long id, IDepartmentService departmentService, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await departmentService.ActivateDepartmentAsync(id, currentUserId, cancellationToken);
        if (result.IsFailure) return Results.BadRequest(new { message = result.Error });
        return Results.Ok(new { message = "Department activated successfully." });
    }

    private static async Task<IResult> DeactivateDepartment(long id, IDepartmentService departmentService, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await departmentService.DeactivateDepartmentAsync(id, currentUserId, cancellationToken);
        if (result.IsFailure) return Results.BadRequest(new { message = result.Error });
        return Results.Ok(new { message = "Department deactivated successfully." });
    }
}
