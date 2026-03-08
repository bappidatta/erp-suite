using ErpSuite.Modules.Admin.Application.Auth;
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
        services.AddScoped<AdminDataSeeder>();

        return services;
    }
}
