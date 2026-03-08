using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ErpSuite.Modules.Admin.Application.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ErpSuite.Modules.Admin.Infrastructure.Security;

public sealed class JwtTokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public LoginResult CreateToken(AuthUser user)
    {
        var issuer = _configuration["Jwt:Issuer"] ?? "ErpSuite";
        var audience = _configuration["Jwt:Audience"] ?? "ErpSuite.Client";
        var secret = _configuration["Jwt:Secret"] ?? "CHANGE_ME_WITH_MIN_32_CHARACTERS_SECRET";
        var expiresMinutes = int.TryParse(_configuration["Jwt:ExpiresInMinutes"], out var parsed) ? parsed : 60;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiresAt = DateTime.UtcNow.AddMinutes(expiresMinutes);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Name, user.FullName),
            new Claim("user_id", user.Id.ToString())
        };

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: expiresAt,
            signingCredentials: credentials
        );

        var serializedToken = new JwtSecurityTokenHandler().WriteToken(token);
        return new LoginResult(serializedToken, expiresAt, user);
    }
}
