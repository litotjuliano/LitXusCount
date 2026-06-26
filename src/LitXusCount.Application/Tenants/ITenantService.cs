using LitXusCount.Application.Common;

namespace LitXusCount.Application.Tenants;

public interface ITenantService
{
    Task<PagedResult<TenantDto>> ListAsync(PagedQuery query, CancellationToken ct = default);
    Task<TenantDto?> GetAsync(long id, CancellationToken ct = default);
    Task<ServiceResult<TenantDto>> CreateAsync(TenantUpsertDto request, CancellationToken ct = default);
    Task<ServiceResult<TenantDto>> EditAsync(long id, TenantUpsertDto request, CancellationToken ct = default);
    Task<bool> ToggleActiveAsync(long id, CancellationToken ct = default);
}
