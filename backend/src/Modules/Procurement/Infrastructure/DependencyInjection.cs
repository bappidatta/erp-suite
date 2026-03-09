using ErpSuite.Modules.Procurement.Application.Vendors;
using ErpSuite.Modules.Procurement.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ErpSuite.Modules.Procurement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddProcurementInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IVendorService, VendorService>();

        return services;
    }
}
