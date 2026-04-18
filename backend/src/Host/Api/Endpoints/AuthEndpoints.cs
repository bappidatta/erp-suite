using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ErpSuite.Modules.Admin.Application.Auth;
using Api.Filters;

namespace Api.Endpoints;

public static class AuthEndpoints
{
    private const string CookieName = "erp_access_token";

    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/v1/auth")
            .WithTags("Auth");

        group.MapPost("login", Login)
            .AddEndpointFilter<ValidationFilter<LoginRequest>>();

        group.MapPost("register", Register)
            .AddEndpointFilter<ValidationFilter<RegisterRequest>>();

        group.MapPost("refresh", Refresh)
            .RequireAuthorization();

        group.MapGet("me", Me)
            .RequireAuthorization();

        group.MapGet("admin-access", AdminAccess)
            .RequireAuthorization("AdminOnly");

        group.MapPost("logout", Logout)
            .RequireAuthorization();

        return app;
    }

    private static async Task<IResult> Login(LoginRequest request, IAuthService authService, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var result = await authService.LoginAsync(request, cancellationToken);
        if (result is null)
        {
            return Results.Json(new { message = "Invalid email or password." }, statusCode: StatusCodes.Status401Unauthorized);
        }

        SetTokenCookie(httpContext, result.AccessToken, result.ExpiresAtUtc);
        return Results.Ok(new AuthResponse(result.ExpiresAtUtc, result.User));
    }

    private static async Task<IResult> Register(RegisterRequest request, IAuthService authService, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var result = await authService.RegisterAsync(request, cancellationToken);
        if (result is null)
        {
            return Results.BadRequest(new { message = "User with this email already exists." });
        }

        SetTokenCookie(httpContext, result.AccessToken, result.ExpiresAtUtc);
        return Results.Ok(new AuthResponse(result.ExpiresAtUtc, result.User));
    }

    private static async Task<IResult> Refresh(IAuthService authService, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var userIdClaim = httpContext.User.FindFirst("user_id")?.Value;
        if (!long.TryParse(userIdClaim, out var userId))
        {
            return Results.Json(new { message = "Invalid token." }, statusCode: StatusCodes.Status401Unauthorized);
        }

        var result = await authService.RefreshAsync(userId, cancellationToken);
        if (result is null)
        {
            return Results.Json(new { message = "User not found or inactive." }, statusCode: StatusCodes.Status401Unauthorized);
        }

        SetTokenCookie(httpContext, result.AccessToken, result.ExpiresAtUtc);
        return Results.Ok(new AuthResponse(result.ExpiresAtUtc, result.User));
    }

    private static IResult Me(HttpContext httpContext)
    {
        var userIdClaim = httpContext.User.FindFirst("user_id")?.Value;
        var emailClaim = httpContext.User.FindFirst(ClaimTypes.Email)?.Value ?? httpContext.User.FindFirst(JwtRegisteredClaimNames.Email)?.Value;
        var nameClaim = httpContext.User.FindFirst(ClaimTypes.Name)?.Value ?? httpContext.User.FindFirst(JwtRegisteredClaimNames.Name)?.Value;
        var roleClaim = httpContext.User.FindFirst(ClaimTypes.Role)?.Value;
        var companyIdClaim = httpContext.User.FindFirst("company_id")?.Value;
        var companyNameClaim = httpContext.User.FindFirst("company_name")?.Value;

        if (!long.TryParse(userIdClaim, out var userId) || emailClaim is null)
        {
            return Results.Json(new { message = "Invalid token." }, statusCode: StatusCodes.Status401Unauthorized);
        }

        var companyId = long.TryParse(companyIdClaim, out var parsedCompanyId) ? parsedCompanyId : 1L;
        var authUser = new AuthUser(userId, emailClaim, nameClaim ?? "", roleClaim ?? "User", companyId, companyNameClaim ?? "ERP Suite");
        return Results.Ok(authUser);
    }

    private static IResult AdminAccess()
    {
        return Results.Ok(new { message = "Admin access granted." });
    }

    private static async Task<IResult> Logout(IAuthService authService, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var userIdClaim = httpContext.User.FindFirst("user_id")?.Value;
        if (!long.TryParse(userIdClaim, out var userId))
        {
            return Results.Json(new { message = "Invalid token." }, statusCode: StatusCodes.Status401Unauthorized);
        }

        var jti = httpContext.User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
        var expClaim = httpContext.User.FindFirst(JwtRegisteredClaimNames.Exp)?.Value;
        DateTime? tokenExpiry = null;
        if (long.TryParse(expClaim, out var expUnix))
        {
            tokenExpiry = DateTimeOffset.FromUnixTimeSeconds(expUnix).UtcDateTime;
        }

        await authService.LogoutAsync(userId, jti, tokenExpiry, cancellationToken);

        ClearTokenCookie(httpContext);
        return Results.NoContent();
    }

    private static void SetTokenCookie(HttpContext httpContext, string token, DateTime expiresAtUtc)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = !httpContext.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment(),
            SameSite = SameSiteMode.Lax,
            Expires = expiresAtUtc,
            Path = "/"
        };

        httpContext.Response.Cookies.Append(CookieName, token, cookieOptions);
    }

    private static void ClearTokenCookie(HttpContext httpContext)
    {
        httpContext.Response.Cookies.Delete(CookieName, new CookieOptions
        {
            HttpOnly = true,
            Secure = !httpContext.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment(),
            SameSite = SameSiteMode.Lax,
            Path = "/"
        });
    }
}
