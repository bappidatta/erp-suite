using ErpSuite.Modules.Inventory.Application.Categories;
using ErpSuite.Modules.Inventory.Application.Items;
using ErpSuite.Modules.Inventory.Application.UOMs;
using ErpSuite.Modules.Inventory.Application.Warehouses;
using ErpSuite.Modules.Inventory.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ErpSuite.Modules.Inventory.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInventoryInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IUomService, UomService>();
        services.AddScoped<IWarehouseService, WarehouseService>();
        services.AddScoped<IItemService, ItemService>();

        return services;
    }
}
