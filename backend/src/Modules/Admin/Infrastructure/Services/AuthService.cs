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

    public AuthService(ErpDbContext dbContext, IPasswordHasher passwordHasher, ITokenService tokenService)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<LoginResult?> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var user = await _dbContext.Users
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

        var authUser = new AuthUser(user.Id, user.Email, $"{user.FirstName} {user.LastName}".Trim(), ResolveRole(user));
        return _tokenService.CreateToken(authUser);
    }

    public async Task<LoginResult?> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        // Check if user already exists
        var existingUser = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail, cancellationToken);

        if (existingUser is not null)
        {
            return null; // User already exists
        }

        // Hash password
        var passwordHash = _passwordHasher.Hash(request.Password);

        // Create new user
        var user = User.Create(request.Email, passwordHash, request.FirstName, request.LastName);
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(cancellationToken);

        // Generate token
        var authUser = new AuthUser(user.Id, user.Email, $"{user.FirstName} {user.LastName}".Trim(), ResolveRole(user));
        return _tokenService.CreateToken(authUser);
    }

    public async Task<LoginResult?> RefreshAsync(long userId, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user is null || !user.IsActive)
        {
            return null;
        }

        var authUser = new AuthUser(user.Id, user.Email, $"{user.FirstName} {user.LastName}".Trim(), ResolveRole(user));
        return _tokenService.CreateToken(authUser);
    }

    public Task LogoutAsync(long userId, CancellationToken cancellationToken)
    {
        // JWT is currently stateless. Logout is handled on the client by clearing token state.
        // This endpoint exists to preserve API contract and support future token revocation.
        return Task.CompletedTask;
    }

    private static string ResolveRole(User user)
    {
        return user.Email.Equals("admin@erpsuite.local", StringComparison.OrdinalIgnoreCase)
            ? "Admin"
            : "User";
    }
}
