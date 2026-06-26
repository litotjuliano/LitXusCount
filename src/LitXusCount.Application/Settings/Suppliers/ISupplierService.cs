using LitXusCount.Application.Common;

namespace LitXusCount.Application.Settings.Suppliers;

public interface ISupplierService
{
    Task<PagedResult<SupplierDto>> ListAsync(PagedQuery query, CancellationToken ct = default);
    Task<IReadOnlyList<SupplierDto>> ListAllActiveAsync(CancellationToken ct = default);
    Task<SupplierDto?> GetAsync(long id, CancellationToken ct = default);
    Task<ServiceResult<SupplierDto>> CreateAsync(SupplierUpsertDto request, CancellationToken ct = default);
    Task<ServiceResult<SupplierDto>> EditAsync(long id, SupplierUpsertDto request, CancellationToken ct = default);
    Task<bool> DeleteAsync(long id, CancellationToken ct = default);
}
