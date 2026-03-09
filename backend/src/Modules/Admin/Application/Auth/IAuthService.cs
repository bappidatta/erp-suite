namespace ErpSuite.Modules.Admin.Application.Auth;

public interface IAuthService
{
    Task<LoginResult?> LoginAsync(LoginRequest request, CancellationToken cancellationToken);
    Task<LoginResult?> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken);
    Task<LoginResult?> RefreshAsync(long userId, CancellationToken cancellationToken);
    Task LogoutAsync(long userId, string? jti, DateTime? tokenExpiry, CancellationToken cancellationToken);
}
