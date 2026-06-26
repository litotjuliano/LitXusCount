namespace LitXusCount.Application.Tenants;

public interface ITenantConnectionCache
{
    string? GetConnectionString(long tenantId);
    void Invalidate(long tenantId);
}
