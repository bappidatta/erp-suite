using ErpSuite.Modules.Finance.Application.Accounts;
using ErpSuite.Modules.Finance.Application.Accounts.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Finance;

[ApiController]
[Route("api/finance/accounts")]
[Authorize(Policy = "AuthenticatedUser")]
public sealed class AccountsController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountsController(IAccountService accountService) => _accountService = accountService;

    [HttpGet]
    public async Task<IActionResult> GetAccounts([FromQuery] GetAccountsQuery query, CancellationToken cancellationToken)
    {
        var result = await _accountService.GetAccountsAsync(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("tree")]
    public async Task<IActionResult> GetAccountTree(CancellationToken cancellationToken)
    {
        var result = await _accountService.GetAccountTreeAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetAccount(long id, CancellationToken cancellationToken)
    {
        var account = await _accountService.GetAccountByIdAsync(id, cancellationToken);
        return account is null ? NotFound(new { message = "Account not found." }) : Ok(account);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = User.FindFirst("user_id")?.Value ?? "system";
        var result = await _accountService.CreateAccountAsync(request, currentUserId, cancellationToken);
        if (result.IsFailure) return BadRequest(new { message = result.Error });
        return CreatedAtAction(nameof(GetAccount), new { id = result.Value.Id }, result.Value);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> UpdateAccount(long id, [FromBody] UpdateAccountRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = User.FindFirst("user_id")?.Value ?? "system";
        var result = await _accountService.UpdateAccountAsync(id, request, currentUserId, cancellationToken);
        if (result.IsFailure) return BadRequest(new { message = result.Error });
        return Ok(result.Value);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteAccount(long id, CancellationToken cancellationToken)
    {
        var currentUserId = User.FindFirst("user_id")?.Value ?? "system";
        var result = await _accountService.DeleteAccountAsync(id, currentUserId, cancellationToken);
        if (result.IsFailure) return BadRequest(new { message = result.Error });
        return NoContent();
    }

    [HttpPost("{id:long}/activate")]
    public async Task<IActionResult> ActivateAccount(long id, CancellationToken cancellationToken)
    {
        var currentUserId = User.FindFirst("user_id")?.Value ?? "system";
        var result = await _accountService.ActivateAccountAsync(id, currentUserId, cancellationToken);
        if (result.IsFailure) return BadRequest(new { message = result.Error });
        return Ok(new { message = "Account activated successfully." });
    }

    [HttpPost("{id:long}/deactivate")]
    public async Task<IActionResult> DeactivateAccount(long id, CancellationToken cancellationToken)
    {
        var currentUserId = User.FindFirst("user_id")?.Value ?? "system";
        var result = await _accountService.DeactivateAccountAsync(id, currentUserId, cancellationToken);
        if (result.IsFailure) return BadRequest(new { message = result.Error });
        return Ok(new { message = "Account deactivated successfully." });
    }
}
