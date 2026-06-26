using LitXusCount.Application.Tenants;
using LitXusCount.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace LitXusCount.Infrastructure.Tenants;

internal sealed class TenantConnectionCache(IConfiguration configuration, IMemoryCache cache)
    : ITenantConnectionCache
{
    private readonly string _masterConnectionString =
        configuration.GetConnectionString("DefaultConnection")!;

    public string? GetConnectionString(long tenantId)
    {
        return cache.GetOrCreate($"tenant-cs:{tenantId}", entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
            var options = new DbContextOptionsBuilder<MasterDbContext>()
                .UseNpgsql(_masterConnectionString)
                .Options;
            using var db = new MasterDbContext(options);
            return db.Tenants
                .Where(t => t.Id == tenantId && t.IsActive)
                .Select(t => t.ConnectionString)
                .FirstOrDefault();
        });
    }

    public void Invalidate(long tenantId) => cache.Remove($"tenant-cs:{tenantId}");
}
