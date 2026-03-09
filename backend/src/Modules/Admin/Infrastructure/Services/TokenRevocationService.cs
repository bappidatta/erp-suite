using ErpSuite.Modules.Admin.Application.Auth;
using ErpSuite.Modules.Admin.Domain.Entities;
using ErpSuite.Modules.Admin.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ErpSuite.Modules.Admin.Infrastructure.Services;

public sealed class TokenRevocationService : ITokenRevocationService
{
    private readonly ErpDbContext _dbContext;

    public TokenRevocationService(ErpDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task RevokeAsync(string jti, long userId, DateTime expiresAt, CancellationToken cancellationToken)
    {
        var existing = await _dbContext.RevokedTokens
            .AnyAsync(rt => rt.Jti == jti, cancellationToken);

        if (existing)
        {
            return;
        }

        var revokedToken = RevokedToken.Create(jti, userId, expiresAt);
        _dbContext.RevokedTokens.Add(revokedToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> IsRevokedAsync(string jti, CancellationToken cancellationToken)
    {
        return await _dbContext.RevokedTokens
            .AnyAsync(rt => rt.Jti == jti, cancellationToken);
    }
}
