using ErpSuite.Modules.Admin.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ErpSuite.Tests.Unit.Helpers;

public static class TestDbContextFactory
{
    public static ErpDbContext Create()
    {
        var options = new DbContextOptionsBuilder<ErpDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new ErpDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }
}
