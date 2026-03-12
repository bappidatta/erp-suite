using ErpSuite.Modules.Procurement.Application.Vendors;
using ErpSuite.Modules.Procurement.Application.Vendors.Dtos;
using Microsoft.AspNetCore.Mvc;
using Api.Filters;

namespace Api.Endpoints.Procurement;

public static class VendorEndpoints
{
    public static IEndpointRouteBuilder MapVendorEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/procurement/vendors")
            .WithTags("Procurement - Vendors")
            .RequireAuthorization("AuthenticatedUser");

        group.MapGet("", GetVendors);

        group.MapGet("{id:long}", GetVendor)
            .WithName("GetVendor");

        group.MapPost("", CreateVendor)
            .AddEndpointFilter<ValidationFilter<CreateVendorRequest>>();

        group.MapPut("{id:long}", UpdateVendor)
            .AddEndpointFilter<ValidationFilter<UpdateVendorRequest>>();

        group.MapDelete("{id:long}", DeleteVendor);

        group.MapPost("{id:long}/activate", ActivateVendor);

        group.MapPost("{id:long}/deactivate", DeactivateVendor);

        return app;
    }

    private static async Task<IResult> GetVendors([AsParameters] GetVendorsQuery query, IVendorService vendorService, CancellationToken cancellationToken)
    {
        var result = await vendorService.GetVendorsAsync(query, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetVendor(long id, IVendorService vendorService, CancellationToken cancellationToken)
    {
        var vendor = await vendorService.GetVendorByIdAsync(id, cancellationToken);
        return vendor is null ? Results.NotFound(new { message = "Vendor not found." }) : Results.Ok(vendor);
    }

    private static async Task<IResult> CreateVendor(CreateVendorRequest request, IVendorService vendorService, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await vendorService.CreateVendorAsync(request, currentUserId, cancellationToken);
        if (result.IsFailure) return Results.BadRequest(new { message = result.Error });
        return Results.CreatedAtRoute("GetVendor", new { id = result.Value.Id }, result.Value);
    }

    private static async Task<IResult> UpdateVendor(long id, UpdateVendorRequest request, IVendorService vendorService, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await vendorService.UpdateVendorAsync(id, request, currentUserId, cancellationToken);
        if (result.IsFailure) return Results.BadRequest(new { message = result.Error });
        return Results.Ok(result.Value);
    }

    private static async Task<IResult> DeleteVendor(long id, IVendorService vendorService, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await vendorService.DeleteVendorAsync(id, currentUserId, cancellationToken);
        if (result.IsFailure) return Results.BadRequest(new { message = result.Error });
        return Results.NoContent();
    }

    private static async Task<IResult> ActivateVendor(long id, IVendorService vendorService, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await vendorService.ActivateVendorAsync(id, currentUserId, cancellationToken);
        if (result.IsFailure) return Results.BadRequest(new { message = result.Error });
        return Results.Ok(new { message = "Vendor activated successfully." });
    }

    private static async Task<IResult> DeactivateVendor(long id, IVendorService vendorService, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await vendorService.DeactivateVendorAsync(id, currentUserId, cancellationToken);
        if (result.IsFailure) return Results.BadRequest(new { message = result.Error });
        return Results.Ok(new { message = "Vendor deactivated successfully." });
    }
}
