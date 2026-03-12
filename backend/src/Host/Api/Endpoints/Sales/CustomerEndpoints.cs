using ErpSuite.Modules.Sales.Application.Customers;
using ErpSuite.Modules.Sales.Application.Customers.Dtos;
using Api.Filters;

namespace Api.Endpoints.Sales;

public static class CustomerEndpoints
{
    public static IEndpointRouteBuilder MapCustomerEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/sales/customers")
            .WithTags("Sales - Customers")
            .RequireAuthorization("AuthenticatedUser");

        group.MapGet("", GetCustomers);

        group.MapGet("{id:long}", GetCustomer)
            .WithName("GetCustomer");

        group.MapPost("", CreateCustomer)
            .AddEndpointFilter<ValidationFilter<CreateCustomerRequest>>();

        group.MapPut("{id:long}", UpdateCustomer)
            .AddEndpointFilter<ValidationFilter<UpdateCustomerRequest>>();

        group.MapDelete("{id:long}", DeleteCustomer);

        group.MapPost("{id:long}/activate", ActivateCustomer);

        group.MapPost("{id:long}/deactivate", DeactivateCustomer);

        return app;
    }

    private static async Task<IResult> GetCustomers([AsParameters] GetCustomersQuery query, ICustomerService customerService, CancellationToken cancellationToken)
    {
        var result = await customerService.GetCustomersAsync(query, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetCustomer(long id, ICustomerService customerService, CancellationToken cancellationToken)
    {
        var customer = await customerService.GetCustomerByIdAsync(id, cancellationToken);
        return customer is null ? Results.NotFound(new { message = "Customer not found." }) : Results.Ok(customer);
    }

    private static async Task<IResult> CreateCustomer(CreateCustomerRequest request, ICustomerService customerService, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await customerService.CreateCustomerAsync(request, currentUserId, cancellationToken);
        if (result.IsFailure) return Results.BadRequest(new { message = result.Error });
        return Results.CreatedAtRoute("GetCustomer", new { id = result.Value.Id }, result.Value);
    }

    private static async Task<IResult> UpdateCustomer(long id, UpdateCustomerRequest request, ICustomerService customerService, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await customerService.UpdateCustomerAsync(id, request, currentUserId, cancellationToken);
        if (result.IsFailure) return Results.BadRequest(new { message = result.Error });
        return Results.Ok(result.Value);
    }

    private static async Task<IResult> DeleteCustomer(long id, ICustomerService customerService, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await customerService.DeleteCustomerAsync(id, currentUserId, cancellationToken);
        if (result.IsFailure) return Results.BadRequest(new { message = result.Error });
        return Results.NoContent();
    }

    private static async Task<IResult> ActivateCustomer(long id, ICustomerService customerService, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await customerService.ActivateCustomerAsync(id, currentUserId, cancellationToken);
        if (result.IsFailure) return Results.BadRequest(new { message = result.Error });
        return Results.Ok(new { message = "Customer activated successfully." });
    }

    private static async Task<IResult> DeactivateCustomer(long id, ICustomerService customerService, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await customerService.DeactivateCustomerAsync(id, currentUserId, cancellationToken);
        if (result.IsFailure) return Results.BadRequest(new { message = result.Error });
        return Results.Ok(new { message = "Customer deactivated successfully." });
    }
}
