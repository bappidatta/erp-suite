using ErpSuite.Modules.Admin.Application.Auth;
using ErpSuite.Modules.Admin.Domain.Entities;
using ErpSuite.Modules.Admin.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ErpSuite.Modules.Admin.Infrastructure.Services;

public sealed class AuthService : IAuthService
{
    private readonly ErpDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly ITokenRevocationService _tokenRevocationService;

    public AuthService(
        ErpDbContext dbContext,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        ITokenRevocationService tokenRevocationService)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _tokenRevocationService = tokenRevocationService;
    }

    public async Task<LoginResult?> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var user = await _dbContext.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail, cancellationToken);

        if (user is null || !user.IsActive)
        {
            return null;
        }

        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            return null;
        }

        user.RecordLogin();
        await _dbContext.SaveChangesAsync(cancellationToken);

        var authUser = new AuthUser(user.Id, user.Email, $"{user.FirstName} {user.LastName}".Trim(), user.Role.Name);
        return _tokenService.CreateToken(authUser);
    }

    public async Task<LoginResult?> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var existingUser = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail, cancellationToken);

        if (existingUser is not null)
        {
            return null;
        }

        var userRole = await _dbContext.Roles
            .FirstOrDefaultAsync(r => r.Name == "User", cancellationToken);

        if (userRole is null)
        {
            return null;
        }

        var passwordHash = _passwordHasher.Hash(request.Password);

        var user = User.Create(request.Email, passwordHash, request.FirstName, request.LastName, userRole.Id);
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var authUser = new AuthUser(user.Id, user.Email, $"{user.FirstName} {user.LastName}".Trim(), userRole.Name);
        return _tokenService.CreateToken(authUser);
    }

    public async Task<LoginResult?> RefreshAsync(long userId, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user is null || !user.IsActive)
        {
            return null;
        }

        var authUser = new AuthUser(user.Id, user.Email, $"{user.FirstName} {user.LastName}".Trim(), user.Role.Name);
        return _tokenService.CreateToken(authUser);
    }

    public async Task LogoutAsync(long userId, string? jti, DateTime? tokenExpiry, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(jti) && tokenExpiry.HasValue)
        {
            await _tokenRevocationService.RevokeAsync(jti, userId, tokenExpiry.Value, cancellationToken);
        }
    }
}
