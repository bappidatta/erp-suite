using ErpSuite.Modules.Admin.Application.Auth;
using ErpSuite.Modules.Admin.Application.Dashboard;
using ErpSuite.Modules.Admin.Application.NumberSequences;
using ErpSuite.Modules.Admin.Application.Organization;
using ErpSuite.Modules.Admin.Application.Roles;
using ErpSuite.Modules.Admin.Application.Users;
using ErpSuite.Modules.Admin.Infrastructure.Services;
using ErpSuite.Modules.Admin.Infrastructure.Security;
using Microsoft.Extensions.DependencyInjection;

namespace ErpSuite.Modules.Admin.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAdminInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITokenRevocationService, TokenRevocationService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IOrganizationSettingsService, OrganizationSettingsService>();
        services.AddScoped<INumberSequenceService, NumberSequenceService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<AdminDataSeeder>();

        return services;
    }
}
