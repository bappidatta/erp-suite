namespace ErpSuite.Modules.Admin.Application.Auth;

public interface ITokenRevocationService
{
    Task RevokeAsync(string jti, long userId, DateTime expiresAt, CancellationToken cancellationToken);
    Task<bool> IsRevokedAsync(string jti, CancellationToken cancellationToken);
}
