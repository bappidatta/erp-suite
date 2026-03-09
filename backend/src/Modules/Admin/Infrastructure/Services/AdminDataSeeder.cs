using ErpSuite.Modules.Admin.Application.Auth;
using ErpSuite.Modules.Admin.Domain.Entities;
using ErpSuite.Modules.Admin.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ErpSuite.Modules.Admin.Infrastructure.Services;

public sealed class AdminDataSeeder
{
    private readonly ErpDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<AdminDataSeeder> _logger;

    public AdminDataSeeder(ErpDbContext dbContext, IPasswordHasher passwordHasher, ILogger<AdminDataSeeder> logger)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        // Seed roles if not present
        if (!await _dbContext.Roles.AnyAsync(cancellationToken))
        {
            var adminRole = Role.Create("Admin", "Full system administrator");
            var userRole = Role.Create("User", "Standard user");

            _dbContext.Roles.Add(adminRole);
            _dbContext.Roles.Add(userRole);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Seeded default roles: Admin, User");
        }

        if (await _dbContext.Users.AnyAsync(cancellationToken))
        {
            return;
        }

        var adminRoleEntity = await _dbContext.Roles
            .FirstAsync(r => r.Name == "Admin", cancellationToken);

        var defaultAdmin = User.Create(
            "admin@erpsuite.local",
            _passwordHasher.Hash("Admin@123"),
            "System",
            "Admin",
            adminRoleEntity.Id,
            mustChangePassword: true);

        var defaultCompany = Company.Create(
            "ERP Suite",
            "ERP",
            "N/A",
            "admin@erpsuite.local",
            "+10000000000");

        _dbContext.Users.Add(defaultAdmin);
        _dbContext.Companies.Add(defaultCompany);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Seeded default admin user: {Email} (password change required on first login)", "admin@erpsuite.local");
    }
}
