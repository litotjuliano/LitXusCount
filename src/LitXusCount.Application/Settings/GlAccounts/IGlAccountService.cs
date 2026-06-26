using LitXusCount.Application.Common;

namespace LitXusCount.Application.Settings.GlAccounts;

public interface IGlAccountService
{
    Task<PagedResult<GlAccountDto>> ListAsync(PagedQuery query, CancellationToken ct = default);
    Task<IReadOnlyList<GlAccountDto>> ListAllActiveAsync(CancellationToken ct = default);
    Task<GlAccountDto?> GetAsync(long id, CancellationToken ct = default);
    Task<ServiceResult<GlAccountDto>> CreateAsync(GlAccountUpsertDto request, CancellationToken ct = default);
    Task<ServiceResult<GlAccountDto>> EditAsync(long id, GlAccountUpsertDto request, CancellationToken ct = default);
    Task<bool> DeleteAsync(long id, CancellationToken ct = default);
}
