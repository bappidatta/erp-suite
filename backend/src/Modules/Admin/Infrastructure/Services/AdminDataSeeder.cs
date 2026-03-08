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
        if (await _dbContext.Users.AnyAsync(cancellationToken))
        {
            return;
        }

        var defaultAdmin = User.Create(
            "admin@erpsuite.local",
            _passwordHasher.Hash("Admin@123"),
            "System",
            "Admin");

        var defaultCompany = Company.Create(
            "ERP Suite",
            "ERP",
            "N/A",
            "admin@erpsuite.local",
            "+10000000000");

        _dbContext.Users.Add(defaultAdmin);
        _dbContext.Companies.Add(defaultCompany);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Seeded default admin user: {Email}", "admin@erpsuite.local");
    }
}
