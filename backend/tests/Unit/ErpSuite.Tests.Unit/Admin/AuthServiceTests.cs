using ErpSuite.Modules.Admin.Application.Auth;
using ErpSuite.Modules.Admin.Domain.Entities;
using ErpSuite.Modules.Admin.Infrastructure.Services;
using ErpSuite.Tests.Unit.Helpers;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;

namespace ErpSuite.Tests.Unit.Admin;

public sealed class AuthServiceTests
{
    [Fact]
    public async Task LoginAsync_ShouldReturnNull_WhenNoActiveCompanyExists()
    {
        var dbContext = TestDbContextFactory.Create();
        var role = Role.Create("Admin", isSystem: true);
        var passwordHasher = new StubPasswordHasher();
        var user = User.Create("admin@erpsuite.local", passwordHasher.Hash("Admin@123"), "System", "Admin", 1);

        dbContext.Roles.Add(role);
        await dbContext.SaveChangesAsync();
        user.AssignRole(role.Id);
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        var sut = new AuthService(
            dbContext,
            passwordHasher,
            new StubTokenService(),
            new StubTokenRevocationService(),
            NullLogger<AuthService>.Instance);

        var result = await sut.LoginAsync(new LoginRequest("admin@erpsuite.local", "Admin@123"), CancellationToken.None);

        result.Should().BeNull();
    }

    private sealed class StubPasswordHasher : IPasswordHasher
    {
        public string Hash(string password) => $"HASH::{password}";
        public bool Verify(string password, string passwordHash) => passwordHash == Hash(password);
    }

    private sealed class StubTokenService : ITokenService
    {
        public LoginResult CreateToken(AuthUser user) => new("token", DateTime.UtcNow.AddHours(1), user, "jti");
    }

    private sealed class StubTokenRevocationService : ITokenRevocationService
    {
        public Task RevokeAsync(string jti, long userId, DateTime expiresAt, CancellationToken cancellationToken) => Task.CompletedTask;
        public Task<bool> IsRevokedAsync(string jti, CancellationToken cancellationToken) => Task.FromResult(false);
    }
}
