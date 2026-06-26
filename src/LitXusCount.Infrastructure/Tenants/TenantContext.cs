using LitXusCount.Application.Authorization;
using LitXusCount.Application.Tenants;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace LitXusCount.Infrastructure.Tenants;

internal sealed class TenantContext : ITenantContext
{
    public long? CurrentTenantId { get; }
    public bool IsSuperAdmin { get; }
    public string? ConnectionString { get; }

    public TenantContext(
        IHttpContextAccessor accessor,
        IConfiguration configuration,
        IMemoryCache cache)
    {
        var user = accessor.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated != true) return;

        IsSuperAdmin = user.IsInRole(RoleNames.SuperAdmin);

        var tidClaim = user.FindFirst("tenant_id");
        if (!long.TryParse(tidClaim?.Value, out var tenantId)) return;

        CurrentTenantId = tenantId;

        var cacheKey = $"tenant-cs:{tenantId}";
        if (!cache.TryGetValue(cacheKey, out string? cs))
        {
            cs = FetchConnectionString(configuration, tenantId);
            if (cs is not null)
                cache.Set(cacheKey, cs, TimeSpan.FromMinutes(10));
        }

        ConnectionString = cs;
    }

    private static string? FetchConnectionString(IConfiguration configuration, long tenantId)
    {
        var masterCs = configuration.GetConnectionString("DefaultConnection")!;
        using var conn = new NpgsqlConnection(masterCs);
        conn.Open();
        using var cmd = new NpgsqlCommand(
            "SELECT \"ConnectionString\" FROM \"Tenants\" WHERE \"Id\" = @id AND \"IsActive\" = TRUE LIMIT 1",
            conn);
        cmd.Parameters.AddWithValue("id", tenantId);
        var result = cmd.ExecuteScalar();
        return result as string;
    }
}
