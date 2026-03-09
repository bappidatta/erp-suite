using ErpSuite.Modules.Admin.Application.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private const string CookieName = "erp_access_token";

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

        SetTokenCookie(result.AccessToken, result.ExpiresAtUtc);
        return Ok(new AuthResponse(result.ExpiresAtUtc, result.User));
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterAsync(request, cancellationToken);
        if (result is null)
        {
            return BadRequest(new { message = "User with this email already exists." });
        }

        SetTokenCookie(result.AccessToken, result.ExpiresAtUtc);
        return Ok(new AuthResponse(result.ExpiresAtUtc, result.User));
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

        SetTokenCookie(result.AccessToken, result.ExpiresAtUtc);
        return Ok(new AuthResponse(result.ExpiresAtUtc, result.User));
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        var userIdClaim = User.FindFirst("user_id")?.Value;
        var emailClaim = User.FindFirst(ClaimTypes.Email)?.Value ?? User.FindFirst(JwtRegisteredClaimNames.Email)?.Value;
        var nameClaim = User.FindFirst(ClaimTypes.Name)?.Value ?? User.FindFirst(JwtRegisteredClaimNames.Name)?.Value;
        var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

        if (!long.TryParse(userIdClaim, out var userId) || emailClaim is null)
        {
            return Unauthorized(new { message = "Invalid token." });
        }

        var authUser = new AuthUser(userId, emailClaim, nameClaim ?? "", roleClaim ?? "User");
        return Ok(authUser);
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpGet("admin-access")]
    public IActionResult AdminAccess()
    {
        return Ok(new { message = "Admin access granted." });
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst("user_id")?.Value;
        if (!long.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "Invalid token." });
        }

        var jti = User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
        var expClaim = User.FindFirst(JwtRegisteredClaimNames.Exp)?.Value;
        DateTime? tokenExpiry = null;
        if (long.TryParse(expClaim, out var expUnix))
        {
            tokenExpiry = DateTimeOffset.FromUnixTimeSeconds(expUnix).UtcDateTime;
        }

        await _authService.LogoutAsync(userId, jti, tokenExpiry, cancellationToken);

        ClearTokenCookie();
        return NoContent();
    }

    private void SetTokenCookie(string token, DateTime expiresAtUtc)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = !HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment(),
            SameSite = SameSiteMode.Lax,
            Expires = expiresAtUtc,
            Path = "/"
        };

        Response.Cookies.Append(CookieName, token, cookieOptions);
    }

    private void ClearTokenCookie()
    {
        Response.Cookies.Delete(CookieName, new CookieOptions
        {
            HttpOnly = true,
            Secure = !HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment(),
            SameSite = SameSiteMode.Lax,
            Path = "/"
        });
    }
}
