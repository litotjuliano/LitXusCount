namespace LitXusCount.Application.Tenants;

public record TenantDto(
    long Id,
    string Name,
    string Slug,
    string? ContactEmail,
    string? Notes,
    bool IsActive,
    DateTime CreatedAt);

public record TenantUpsertDto(
    string Name,
    string Slug,
    string? ContactEmail,
    string? Notes,
    string? AdminEmail,
    string? AdminPassword);
