using LitXusCount.Application.Tenants;

namespace LitXusCount.API.Middleware;

public class TenantGuardMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, ITenantContext tenantContext)
    {
        if (tenantContext.IsDeactivated)
        {
            await WriteJsonAsync(context, StatusCodes.Status403Forbidden, "Tenant is inactive.");
            return;
        }

        // If there is no tenant connection string and the user is not SuperAdmin,
        // block early to avoid a DI resolution exception downstream.
        if (tenantContext.ConnectionString is null && !tenantContext.IsSuperAdmin
            && context.User.Identity?.IsAuthenticated == true)
        {
            await WriteJsonAsync(context, StatusCodes.Status403Forbidden,
                "This endpoint requires a tenant context. Ensure your account is assigned to a tenant.");
            return;
        }

        try
        {
            await next(context);
        }
        catch (InvalidOperationException ex)
            when (ex.Message.StartsWith("No tenant connection string is available"))
        {
            // A controller injected ApplicationDbContext but no tenant was resolved
            // (typically a SuperAdmin hitting a tenant-scoped endpoint).
            if (!context.Response.HasStarted)
                await WriteJsonAsync(context, StatusCodes.Status403Forbidden,
                    "This endpoint requires a tenant context. Include a tenant_id claim in your JWT.");
        }
    }

    private static async Task WriteJsonAsync(HttpContext context, int statusCode, string message)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync($"{{\"message\":\"{message}\"}}");
    }
}
