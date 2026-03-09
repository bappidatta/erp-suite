using ErpSuite.Modules.Admin.Application.Auth;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(request, cancellationToken);
        if (result is null)
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        return Ok(result);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterAsync(request, cancellationToken);
        if (result is null)
        {
            return BadRequest(new { message = "User with this email already exists." });
        }

        return Ok(result);
    }

    [Authorize]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst("user_id")?.Value;
        if (!long.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "Invalid token." });
        }

        var result = await _authService.RefreshAsync(userId, cancellationToken);
        if (result is null)
        {
            return Unauthorized(new { message = "User not found or inactive." });
        }

        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("admin-access")]
    public IActionResult AdminAccess()
    {
        return Ok(new { message = "Admin access granted." });
    }
}
