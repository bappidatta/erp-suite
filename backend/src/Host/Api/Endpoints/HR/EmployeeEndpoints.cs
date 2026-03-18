using ErpSuite.Modules.HR.Application.Employees;
using ErpSuite.Modules.HR.Application.Employees.Dtos;
using Api.Filters;

namespace Api.Endpoints.HR;

public static class EmployeeEndpoints
{
    public static IEndpointRouteBuilder MapEmployeeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/hr/employees")
            .WithTags("HR - Employees")
            .RequireAuthorization("AuthenticatedUser");

        group.MapGet("", GetEmployees);

        group.MapGet("{id:long}", GetEmployee)
            .WithName("GetEmployee");

        group.MapPost("", CreateEmployee)
            .AddEndpointFilter<ValidationFilter<CreateEmployeeRequest>>();

        group.MapPut("{id:long}", UpdateEmployee)
            .AddEndpointFilter<ValidationFilter<UpdateEmployeeRequest>>();

        group.MapDelete("{id:long}", DeleteEmployee);

        return app;
    }

    private static async Task<IResult> GetEmployees([AsParameters] GetEmployeesQuery query, IEmployeeService employeeService, CancellationToken cancellationToken)
    {
        var result = await employeeService.GetEmployeesAsync(query, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetEmployee(long id, IEmployeeService employeeService, CancellationToken cancellationToken)
    {
        var employee = await employeeService.GetEmployeeByIdAsync(id, cancellationToken);
        return employee is null ? Results.NotFound(new { message = "Employee not found." }) : Results.Ok(employee);
    }

    private static async Task<IResult> CreateEmployee(CreateEmployeeRequest request, IEmployeeService employeeService, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await employeeService.CreateEmployeeAsync(request, currentUserId, cancellationToken);
        if (result.IsFailure) return Results.BadRequest(new { message = result.Error });
        return Results.CreatedAtRoute("GetEmployee", new { id = result.Value.Id }, result.Value);
    }

    private static async Task<IResult> UpdateEmployee(long id, UpdateEmployeeRequest request, IEmployeeService employeeService, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await employeeService.UpdateEmployeeAsync(id, request, currentUserId, cancellationToken);
        if (result.IsFailure) return Results.BadRequest(new { message = result.Error });
        return Results.Ok(result.Value);
    }

    private static async Task<IResult> DeleteEmployee(long id, IEmployeeService employeeService, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await employeeService.DeleteEmployeeAsync(id, currentUserId, cancellationToken);
        if (result.IsFailure) return Results.BadRequest(new { message = result.Error });
        return Results.NoContent();
    }
}
