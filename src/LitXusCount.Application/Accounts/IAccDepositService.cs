using LitXusCount.Application.Common;

namespace LitXusCount.Application.Accounts;

public interface IAccDepositService
{
    Task<PagedResult<AccDepositDto>> ListAsync(PagedQuery query, long? accountId = null, CancellationToken ct = default);
    Task<AccDepositDto?> GetAsync(long id, CancellationToken ct = default);
    Task<ServiceResult<AccDepositDto>> CreateAsync(AccDepositCreateDto request, CancellationToken ct = default);
    Task<bool> DeleteAsync(long id, CancellationToken ct = default);
}
