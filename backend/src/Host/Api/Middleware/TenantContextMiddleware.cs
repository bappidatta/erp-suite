using ErpSuite.BuildingBlocks.Domain.ValueObjects;

namespace Api.Middleware;

public sealed class TenantContextMiddleware
{
    private const string TenantItemKey = "TenantContext";
    private readonly RequestDelegate _next;

    public TenantContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var companyIdHeader = context.Request.Headers["X-Company-Id"].FirstOrDefault();
        var companyNameHeader = context.Request.Headers["X-Company-Name"].FirstOrDefault();

        var companyId = long.TryParse(companyIdHeader, out var parsedId) ? parsedId : 1L;
        var companyName = string.IsNullOrWhiteSpace(companyNameHeader) ? "ERP Suite" : companyNameHeader;

        context.Items[TenantItemKey] = new TenantContext(companyId, companyName);

        await _next(context);
    }
}
