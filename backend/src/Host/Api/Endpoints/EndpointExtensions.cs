using Api.Endpoints.Admin;
using Api.Endpoints.Finance;
using Api.Endpoints.HR;
using Api.Endpoints.Inventory;
using Api.Endpoints.Procurement;
using Api.Endpoints.Sales;

namespace Api.Endpoints;

public static class EndpointExtensions
{
    public static IEndpointRouteBuilder MapApiEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapAuthEndpoints();
        app.MapUserEndpoints();
        app.MapRoleEndpoints();
        app.MapAdminDashboardEndpoints();
        app.MapOrganizationEndpoints();
        app.MapAccountEndpoints();
        app.MapTaxCodeEndpoints();
        app.MapCustomerEndpoints();
        app.MapVendorEndpoints();
        app.MapCategoryEndpoints();
        app.MapUomEndpoints();
        app.MapWarehouseEndpoints();
        app.MapItemEndpoints();
        app.MapDepartmentEndpoints();
        app.MapEmployeeEndpoints();

        return app;
    }
}
