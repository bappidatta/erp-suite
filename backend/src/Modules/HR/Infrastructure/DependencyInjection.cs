using ErpSuite.Modules.HR.Application.Departments;
using ErpSuite.Modules.HR.Application.Employees;
using ErpSuite.Modules.HR.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ErpSuite.Modules.HR.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddHrInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IDepartmentService, DepartmentService>();
        services.AddScoped<IEmployeeService, EmployeeService>();

        return services;
    }
}
