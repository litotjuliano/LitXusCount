using LitXusCount.Application.Common;

namespace LitXusCount.Application.Settings.VatPercentages;

public interface IVatPercentageService
{
    Task<PagedResult<VatPercentageDto>> ListAsync(PagedQuery query, CancellationToken ct = default);
    Task<IReadOnlyList<VatPercentageDto>> ListAllActiveAsync(CancellationToken ct = default);
    Task<VatPercentageDto?> GetAsync(long id, CancellationToken ct = default);
    Task<ServiceResult<VatPercentageDto>> CreateAsync(VatPercentageUpsertDto request, CancellationToken ct = default);
    Task<ServiceResult<VatPercentageDto>> EditAsync(long id, VatPercentageUpsertDto request, CancellationToken ct = default);
    Task<bool> DeleteAsync(long id, CancellationToken ct = default);
}
