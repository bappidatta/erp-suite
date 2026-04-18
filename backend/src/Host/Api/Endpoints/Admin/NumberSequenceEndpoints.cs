using Api.Filters;
using ErpSuite.Modules.Admin.Application.NumberSequences;
using ErpSuite.Modules.Admin.Application.NumberSequences.Dtos;

namespace Api.Endpoints.Admin;

public static class NumberSequenceEndpoints
{
    public static IEndpointRouteBuilder MapNumberSequenceEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/admin/number-sequences")
            .WithTags("Admin - Number Sequences")
            .RequireAuthorization("AdminOnly");

        group.MapGet("", GetNumberSequences);
        group.MapGet("{id:long}", GetNumberSequence).WithName("GetNumberSequence");
        group.MapGet("{id:long}/preview", GetPreview);
        group.MapPost("", CreateNumberSequence)
            .AddEndpointFilter<ValidationFilter<CreateNumberSequenceRequest>>();
        group.MapPut("{id:long}", UpdateNumberSequence)
            .AddEndpointFilter<ValidationFilter<UpdateNumberSequenceRequest>>();
        group.MapDelete("{id:long}", DeleteNumberSequence);
        group.MapPost("{id:long}/activate", ActivateNumberSequence);
        group.MapPost("{id:long}/deactivate", DeactivateNumberSequence);

        return app;
    }

    private static async Task<IResult> GetNumberSequences([AsParameters] GetNumberSequencesQuery query, INumberSequenceService service, CancellationToken cancellationToken)
    {
        var result = await service.GetNumberSequencesAsync(query, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetNumberSequence(long id, INumberSequenceService service, CancellationToken cancellationToken)
    {
        var result = await service.GetNumberSequenceByIdAsync(id, cancellationToken);
        return result is null
            ? Results.NotFound(new { message = "Number sequence not found." })
            : Results.Ok(result);
    }

    private static async Task<IResult> GetPreview(long id, INumberSequenceService service, CancellationToken cancellationToken)
    {
        var result = await service.GetPreviewAsync(id, cancellationToken);
        return result.IsFailure
            ? Results.BadRequest(new { message = result.Error })
            : Results.Ok(new { preview = result.Value });
    }

    private static async Task<IResult> CreateNumberSequence(CreateNumberSequenceRequest request, INumberSequenceService service, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await service.CreateNumberSequenceAsync(request, currentUserId, cancellationToken);
        return result.IsFailure
            ? Results.BadRequest(new { message = result.Error })
            : Results.CreatedAtRoute("GetNumberSequence", new { id = result.Value.Id }, result.Value);
    }

    private static async Task<IResult> UpdateNumberSequence(long id, UpdateNumberSequenceRequest request, INumberSequenceService service, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await service.UpdateNumberSequenceAsync(id, request, currentUserId, cancellationToken);
        return result.IsFailure
            ? Results.BadRequest(new { message = result.Error })
            : Results.Ok(result.Value);
    }

    private static async Task<IResult> DeleteNumberSequence(long id, INumberSequenceService service, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await service.DeleteNumberSequenceAsync(id, currentUserId, cancellationToken);
        return result.IsFailure
            ? Results.BadRequest(new { message = result.Error })
            : Results.NoContent();
    }

    private static async Task<IResult> ActivateNumberSequence(long id, INumberSequenceService service, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await service.ActivateNumberSequenceAsync(id, currentUserId, cancellationToken);
        return result.IsFailure
            ? Results.BadRequest(new { message = result.Error })
            : Results.Ok(new { message = "Number sequence activated successfully." });
    }

    private static async Task<IResult> DeactivateNumberSequence(long id, INumberSequenceService service, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await service.DeactivateNumberSequenceAsync(id, currentUserId, cancellationToken);
        return result.IsFailure
            ? Results.BadRequest(new { message = result.Error })
            : Results.Ok(new { message = "Number sequence deactivated successfully." });
    }
}
