using ErpSuite.BuildingBlocks.Domain.Entities;

namespace ErpSuite.Modules.Admin.Domain.Entities;

public class RevokedToken : BaseEntity
{
    public string Jti { get; private set; } = string.Empty;
    public long UserId { get; private set; }
    public DateTime ExpiresAt { get; private set; }

    private RevokedToken() { } // EF Core

    public static RevokedToken Create(string jti, long userId, DateTime expiresAt)
    {
        return new RevokedToken
        {
            Jti = jti,
            UserId = userId,
            ExpiresAt = expiresAt
        };
    }
}
