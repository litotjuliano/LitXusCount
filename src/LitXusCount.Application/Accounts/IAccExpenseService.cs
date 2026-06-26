using LitXusCount.Application.Common;

namespace LitXusCount.Application.Accounts;

public interface IAccExpenseService
{
    Task<PagedResult<AccExpenseDto>> ListAsync(PagedQuery query, long? accountId = null, CancellationToken ct = default);
    Task<AccExpenseDto?> GetAsync(long id, CancellationToken ct = default);
    Task<ServiceResult<AccExpenseDto>> CreateAsync(AccExpenseCreateDto request, CancellationToken ct = default);
    Task<bool> DeleteAsync(long id, CancellationToken ct = default);
}
