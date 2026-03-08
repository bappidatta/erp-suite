using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ErpSuite.Modules.Admin.Infrastructure.Persistence;

public sealed class ErpDbContextFactory : IDesignTimeDbContextFactory<ErpDbContext>
{
    public ErpDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ErpDbContext>();

        var connectionString = Environment.GetEnvironmentVariable("ERP_DB_CONNECTION")
            ?? "Host=localhost;Port=5432;Database=erp_suite;Username=postgres;Password=postgres";

        optionsBuilder.UseNpgsql(connectionString);

        return new ErpDbContext(optionsBuilder.Options);
    }
}
