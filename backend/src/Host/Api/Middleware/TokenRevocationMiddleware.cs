using System.IdentityModel.Tokens.Jwt;
using ErpSuite.Modules.Admin.Application.Auth;

namespace Api.Middleware;

public sealed class TokenRevocationMiddleware
{
    private readonly RequestDelegate _next;

    public TokenRevocationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITokenRevocationService tokenRevocationService)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var jti = context.User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

            if (!string.IsNullOrEmpty(jti))
            {
                var isRevoked = await tokenRevocationService.IsRevokedAsync(jti, context.RequestAborted);
                if (isRevoked)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }
            }
        }

        await _next(context);
    }
}
