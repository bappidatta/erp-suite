using ErpSuite.Modules.Finance.Application.Accounts;
using ErpSuite.Modules.Finance.Application.Accounts.Dtos;
using Microsoft.AspNetCore.Mvc;
using Api.Filters;

namespace Api.Endpoints.Finance;

public static class AccountEndpoints
{
    public static IEndpointRouteBuilder MapAccountEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/finance/accounts")
            .WithTags("Finance - Accounts")
            .RequireAuthorization("AuthenticatedUser");

        group.MapGet("", GetAccounts);

        group.MapGet("tree", GetAccountTree);

        group.MapGet("{id:long}", GetAccount)
            .WithName("GetAccount");

        group.MapPost("", CreateAccount)
            .AddEndpointFilter<ValidationFilter<CreateAccountRequest>>();

        group.MapPut("{id:long}", UpdateAccount)
            .AddEndpointFilter<ValidationFilter<UpdateAccountRequest>>();

        group.MapDelete("{id:long}", DeleteAccount);

        group.MapPost("{id:long}/activate", ActivateAccount);

        group.MapPost("{id:long}/deactivate", DeactivateAccount);

        return app;
    }

    private static async Task<IResult> GetAccounts([AsParameters] GetAccountsQuery query, IAccountService accountService, CancellationToken cancellationToken)
    {
        var result = await accountService.GetAccountsAsync(query, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetAccountTree(IAccountService accountService, CancellationToken cancellationToken)
    {
        var result = await accountService.GetAccountTreeAsync(cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetAccount(long id, IAccountService accountService, CancellationToken cancellationToken)
    {
        var account = await accountService.GetAccountByIdAsync(id, cancellationToken);
        return account is null ? Results.NotFound(new { message = "Account not found." }) : Results.Ok(account);
    }

    private static async Task<IResult> CreateAccount(CreateAccountRequest request, IAccountService accountService, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await accountService.CreateAccountAsync(request, currentUserId, cancellationToken);
        if (result.IsFailure) return Results.BadRequest(new { message = result.Error });
        return Results.CreatedAtRoute("GetAccount", new { id = result.Value.Id }, result.Value);
    }

    private static async Task<IResult> UpdateAccount(long id, UpdateAccountRequest request, IAccountService accountService, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await accountService.UpdateAccountAsync(id, request, currentUserId, cancellationToken);
        if (result.IsFailure) return Results.BadRequest(new { message = result.Error });
        return Results.Ok(result.Value);
    }

    private static async Task<IResult> DeleteAccount(long id, IAccountService accountService, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await accountService.DeleteAccountAsync(id, currentUserId, cancellationToken);
        if (result.IsFailure) return Results.BadRequest(new { message = result.Error });
        return Results.NoContent();
    }

    private static async Task<IResult> ActivateAccount(long id, IAccountService accountService, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await accountService.ActivateAccountAsync(id, currentUserId, cancellationToken);
        if (result.IsFailure) return Results.BadRequest(new { message = result.Error });
        return Results.Ok(new { message = "Account activated successfully." });
    }

    private static async Task<IResult> DeactivateAccount(long id, IAccountService accountService, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var currentUserId = httpContext.User.FindFirst("user_id")?.Value ?? "system";
        var result = await accountService.DeactivateAccountAsync(id, currentUserId, cancellationToken);
        if (result.IsFailure) return Results.BadRequest(new { message = result.Error });
        return Results.Ok(new { message = "Account deactivated successfully." });
    }
}
