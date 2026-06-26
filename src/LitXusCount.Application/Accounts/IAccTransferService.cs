using LitXusCount.Application.Common;

namespace LitXusCount.Application.Accounts;

public interface IAccTransferService
{
    Task<PagedResult<AccTransferDto>> ListAsync(PagedQuery query, CancellationToken ct = default);
    Task<AccTransferDto?> GetAsync(long id, CancellationToken ct = default);
    Task<ServiceResult<AccTransferDto>> CreateAsync(AccTransferCreateDto request, CancellationToken ct = default);
    Task<bool> DeleteAsync(long id, CancellationToken ct = default);
}
