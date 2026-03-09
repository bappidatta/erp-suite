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
        long companyId = 1L;
        var companyName = "ERP Suite";

        // Derive tenant from authenticated user claims when available
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var companyIdClaim = context.User.FindFirst("company_id")?.Value;
            var companyNameClaim = context.User.FindFirst("company_name")?.Value;

            if (long.TryParse(companyIdClaim, out var parsedId))
            {
                companyId = parsedId;
            }

            if (!string.IsNullOrWhiteSpace(companyNameClaim))
            {
                companyName = companyNameClaim;
            }
        }

        context.Items[TenantItemKey] = new TenantContext(companyId, companyName);

        await _next(context);
    }
}
