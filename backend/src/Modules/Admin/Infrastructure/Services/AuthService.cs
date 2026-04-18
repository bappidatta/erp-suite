using ErpSuite.Modules.Admin.Application.Auth;
using ErpSuite.Modules.Admin.Domain.Entities;
using ErpSuite.Modules.Admin.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ErpSuite.Modules.Admin.Infrastructure.Services;

public sealed class AuthService : IAuthService
{
    private readonly ErpDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly ITokenRevocationService _tokenRevocationService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        ErpDbContext dbContext,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        ITokenRevocationService tokenRevocationService,
        ILogger<AuthService> logger)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _tokenRevocationService = tokenRevocationService;
        _logger = logger;
    }

    public async Task<LoginResult?> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var company = await ResolveActiveCompanyAsync(cancellationToken);
        if (company is null)
        {
            _logger.LogWarning("Login attempt rejected because no active company is configured.");
            return null;
        }

        var user = await _dbContext.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail, cancellationToken);

        if (user is null || user.Status == UserStatus.Inactive || user.Status == UserStatus.Suspended)
        {
            return null;
        }

        if (user.Status == UserStatus.Locked && user.LockedUntil > DateTime.UtcNow)
        {
            _logger.LogWarning("Login attempt for locked account: {Email}", request.Email);
            return null;
        }

        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            user.RecordFailedLogin();
            await _dbContext.SaveChangesAsync(cancellationToken);
            return null;
        }

        user.RecordSuccessfulLogin();
        await _dbContext.SaveChangesAsync(cancellationToken);

        var authUser = new AuthUser(
            user.Id,
            user.Email,
            $"{user.FirstName} {user.LastName}".Trim(),
            user.Role.Name,
            company.Value.Id,
            company.Value.Name);
        return _tokenService.CreateToken(authUser);
    }

    public async Task<LoginResult?> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var company = await ResolveActiveCompanyAsync(cancellationToken);
        if (company is null)
        {
            _logger.LogWarning("Registration attempt rejected because no active company is configured.");
            return null;
        }

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

        var authUser = new AuthUser(
            user.Id,
            user.Email,
            $"{user.FirstName} {user.LastName}".Trim(),
            userRole.Name,
            company.Value.Id,
            company.Value.Name);
        return _tokenService.CreateToken(authUser);
    }

    public async Task<LoginResult?> RefreshAsync(long userId, CancellationToken cancellationToken)
    {
        var company = await ResolveActiveCompanyAsync(cancellationToken);
        if (company is null)
        {
            _logger.LogWarning("Token refresh rejected because no active company is configured.");
            return null;
        }

        var user = await _dbContext.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user is null || user.Status is UserStatus.Inactive or UserStatus.Suspended)
        {
            return null;
        }

        var authUser = new AuthUser(
            user.Id,
            user.Email,
            $"{user.FirstName} {user.LastName}".Trim(),
            user.Role.Name,
            company.Value.Id,
            company.Value.Name);
        return _tokenService.CreateToken(authUser);
    }

    public async Task LogoutAsync(long userId, string? jti, DateTime? tokenExpiry, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(jti) && tokenExpiry.HasValue)
        {
            await _tokenRevocationService.RevokeAsync(jti, userId, tokenExpiry.Value, cancellationToken);
        }
    }

    private async Task<(long Id, string Name)?> ResolveActiveCompanyAsync(CancellationToken cancellationToken)
    {
        var company = await _dbContext.Companies
            .AsNoTracking()
            .Where(c => c.IsActive)
            .OrderBy(c => c.Id)
            .Select(c => new { c.Id, c.Name })
            .FirstOrDefaultAsync(cancellationToken);

        return company is null ? null : (company.Id, company.Name);
    }
}
