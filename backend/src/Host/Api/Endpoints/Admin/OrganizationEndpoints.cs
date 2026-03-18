using ErpSuite.Modules.Admin.Application.Organization;
using ErpSuite.Modules.Admin.Application.Organization.Dtos;
using Api.Filters;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.Admin;

public static class OrganizationEndpoints
{
    private const long MaxLogoSizeBytes = 5 * 1024 * 1024;

    public static IEndpointRouteBuilder MapOrganizationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/admin/organization")
            .WithTags("Admin - Organization")
            .RequireAuthorization("AdminOnly");

        group.MapGet("", GetSettings);

        group.MapPut("", UpdateSettings)
            .AddEndpointFilter<ValidationFilter<UpdateOrganizationSettingsRequest>>();

        group.MapPost("logo", UploadLogo)
            .DisableAntiforgery()
            .WithMetadata(new RequestSizeLimitAttribute(MaxLogoSizeBytes));

        return app;
    }

    private static async Task<IResult> GetSettings(IOrganizationSettingsService organizationService, CancellationToken cancellationToken)
    {
        var settings = await organizationService.GetAsync(cancellationToken);
        if (settings is null)
            return Results.Ok(new OrganizationSettingsResponse(0, "My Organization", null, null, null, null, null, null, null, "USD", null, null, null, null));
        return Results.Ok(settings);
    }

    private static async Task<IResult> UpdateSettings(UpdateOrganizationSettingsRequest request, IOrganizationSettingsService organizationService, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await organizationService.UpdateAsync(request, currentUserId, cancellationToken);
        if (result.IsFailure) return Results.BadRequest(new { message = result.Error });
        return Results.Ok(result.Value);
    }

    private static async Task<IResult> UploadLogo(IFormFile file, IOrganizationSettingsService organizationService, HttpContext httpContext, CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
            return Results.BadRequest(new { message = "No file provided." });

        if (file.Length > MaxLogoSizeBytes)
            return Results.BadRequest(new { message = "File size exceeds the 5MB limit." });

        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";

        await using var stream = file.OpenReadStream();
        var result = await organizationService.UploadLogoAsync(stream, file.FileName, currentUserId, cancellationToken);

        if (result.IsFailure) return Results.BadRequest(new { message = result.Error });
        return Results.Ok(new { logoPath = result.Value });
    }
}
