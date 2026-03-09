using ErpSuite.Modules.Finance.Application.Accounts;
using ErpSuite.Modules.Finance.Application.TaxCodes;
using ErpSuite.Modules.Finance.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ErpSuite.Modules.Finance.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddFinanceInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<ITaxCodeService, TaxCodeService>();
        services.AddScoped<IAccountService, AccountService>();

        return services;
    }
}
