using LitXusCount.Application.Common;

namespace LitXusCount.Application.Settings.EmailConfigs;

public interface IEmailConfigService
{
    Task<PagedResult<EmailConfigDto>> ListAsync(PagedQuery query, CancellationToken ct = default);
    Task<IReadOnlyList<EmailConfigDto>> ListAllActiveAsync(CancellationToken ct = default);
    Task<EmailConfigDto?> GetAsync(long id, CancellationToken ct = default);
    Task<ServiceResult<EmailConfigDto>> CreateAsync(EmailConfigUpsertDto request, CancellationToken ct = default);
    Task<ServiceResult<EmailConfigDto>> EditAsync(long id, EmailConfigUpsertDto request, CancellationToken ct = default);
    Task<bool> DeleteAsync(long id, CancellationToken ct = default);
}
