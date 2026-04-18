using Api.Filters;
using ErpSuite.Modules.Finance.Application.JournalEntries;
using ErpSuite.Modules.Finance.Application.JournalEntries.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.Finance;

public static class JournalEntryEndpoints
{
    public static IEndpointRouteBuilder MapJournalEntryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/finance/journal-entries")
            .WithTags("Finance - Journal Entries")
            .RequireAuthorization("AuthenticatedUser");

        group.MapGet("", GetJournalEntries);
        group.MapGet("{id:long}", GetJournalEntry).WithName("GetJournalEntry");
        group.MapPost("", CreateJournalEntry).AddEndpointFilter<ValidationFilter<CreateJournalEntryRequest>>();
        group.MapPut("{id:long}", UpdateJournalEntry).AddEndpointFilter<ValidationFilter<UpdateJournalEntryRequest>>();
        group.MapDelete("{id:long}", DeleteJournalEntry);
        group.MapPost("{id:long}/post", PostJournalEntry);

        return app;
    }

    private static async Task<IResult> GetJournalEntries([AsParameters] GetJournalEntriesQuery query, IJournalEntryService service, CancellationToken cancellationToken)
    {
        var result = await service.GetJournalEntriesAsync(query, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetJournalEntry(long id, IJournalEntryService service, CancellationToken cancellationToken)
    {
        var result = await service.GetJournalEntryByIdAsync(id, cancellationToken);
        return result is null ? Results.NotFound(new { message = "Journal entry not found." }) : Results.Ok(result);
    }

    private static async Task<IResult> CreateJournalEntry(CreateJournalEntryRequest request, IJournalEntryService service, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await service.CreateJournalEntryAsync(request, currentUserId, cancellationToken);
        return result.IsFailure
            ? Results.BadRequest(new { message = result.Error })
            : Results.CreatedAtRoute("GetJournalEntry", new { id = result.Value.Id }, result.Value);
    }

    private static async Task<IResult> UpdateJournalEntry(long id, UpdateJournalEntryRequest request, IJournalEntryService service, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await service.UpdateJournalEntryAsync(id, request, currentUserId, cancellationToken);
        return result.IsFailure ? Results.BadRequest(new { message = result.Error }) : Results.Ok(result.Value);
    }

    private static async Task<IResult> DeleteJournalEntry(long id, IJournalEntryService service, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await service.DeleteJournalEntryAsync(id, currentUserId, cancellationToken);
        return result.IsFailure ? Results.BadRequest(new { message = result.Error }) : Results.NoContent();
    }

    private static async Task<IResult> PostJournalEntry(long id, IJournalEntryService service, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await service.PostJournalEntryAsync(id, currentUserId, cancellationToken);
        return result.IsFailure ? Results.BadRequest(new { message = result.Error }) : Results.Ok(result.Value);
    }
}
