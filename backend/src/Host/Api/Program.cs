using System.Text;
using Api.Middleware;
using ErpSuite.Modules.Admin.Application.Auth.Validators;
using ErpSuite.Modules.Admin.Infrastructure;
using ErpSuite.Modules.Admin.Infrastructure.Persistence;
using ErpSuite.Modules.Admin.Infrastructure.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext());

var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "ErpSuite";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "ErpSuite.Client";
var jwtSecret = builder.Configuration["Jwt:Secret"] ?? "CHANGE_ME_WITH_MIN_32_CHARACTERS_SECRET";

// Add DbContext
builder.Services.AddDbContext<ErpDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAdminInfrastructure();

// Add FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("frontend", policy =>
        policy.WithOrigins("http://localhost:5173", "http://localhost:5174", "http://localhost:5175")
            .WithHeaders("Content-Type", "Authorization", "Accept")
            .WithMethods("GET", "POST", "PUT", "PATCH", "DELETE", "OPTIONS")
            .AllowCredentials());
});

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (string.IsNullOrEmpty(context.Token) &&
                    context.Request.Cookies.TryGetValue("erp_access_token", out var cookieToken))
                {
                    context.Token = cookieToken;
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("AuthenticatedUser", policy => policy.RequireAuthenticatedUser());
});
builder.Services.AddHealthChecks();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi("v1");

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ErpDbContext>();
    await dbContext.Database.MigrateAsync();

    var seeder = scope.ServiceProvider.GetRequiredService<AdminDataSeeder>();
    await seeder.SeedAsync(CancellationToken.None);
}

app.MapOpenApi("/openapi/{documentName}.json");

app.MapGet("/", () => Results.Ok(new { service = "ERP Suite API", status = "healthy" }));
app.MapHealthChecks("/health");

app.UseMiddleware<SecurityHeadersMiddleware>();
app.UseCors("frontend");
app.UseSerilogRequestLogging();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<TokenRevocationMiddleware>();
app.UseMiddleware<TenantContextMiddleware>();
app.MapControllers();

app.Run();
