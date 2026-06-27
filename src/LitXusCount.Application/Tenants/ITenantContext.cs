namespace LitXusCount.Application.Tenants;

public interface ITenantContext
{
    long? CurrentTenantId { get; }
    bool IsSuperAdmin { get; }
    string? ConnectionString { get; }
    bool IsDeactivated { get; }
}
