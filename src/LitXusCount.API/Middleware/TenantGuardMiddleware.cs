using LitXusCount.Application.Tenants;

namespace LitXusCount.API.Middleware;

public class TenantGuardMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, ITenantContext tenantContext)
    {
        if (tenantContext.IsDeactivated)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync("{\"message\":\"Tenant is inactive.\"}");
            return;
        }

        await next(context);
    }
}
