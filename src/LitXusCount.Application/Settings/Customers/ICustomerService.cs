using LitXusCount.Application.Common;

namespace LitXusCount.Application.Settings.Customers;

public interface ICustomerService
{
    Task<PagedResult<CustomerDto>> ListAsync(PagedQuery query, CancellationToken ct = default);
    Task<IReadOnlyList<CustomerDto>> ListAllActiveAsync(CancellationToken ct = default);
    Task<CustomerDto?> GetAsync(long id, CancellationToken ct = default);
    Task<ServiceResult<CustomerDto>> CreateAsync(CustomerUpsertDto request, CancellationToken ct = default);
    Task<ServiceResult<CustomerDto>> EditAsync(long id, CustomerUpsertDto request, CancellationToken ct = default);
    Task<bool> DeleteAsync(long id, CancellationToken ct = default);
}
