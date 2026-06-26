using LitXusCount.Application.Common;

namespace LitXusCount.Application.Settings.Products;

public interface IProductService
{
    Task<PagedResult<ProductDto>> ListAsync(PagedQuery query, CancellationToken ct = default);
    Task<IReadOnlyList<ProductDto>> ListAllActiveAsync(CancellationToken ct = default);
    Task<ProductDto?> GetAsync(long id, CancellationToken ct = default);
    Task<ServiceResult<ProductDto>> CreateAsync(ProductUpsertDto request, CancellationToken ct = default);
    Task<ServiceResult<ProductDto>> EditAsync(long id, ProductUpsertDto request, CancellationToken ct = default);
    Task<bool> DeleteAsync(long id, CancellationToken ct = default);
}
