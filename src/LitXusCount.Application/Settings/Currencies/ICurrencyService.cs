using LitXusCount.Application.Common;

namespace LitXusCount.Application.Settings.Currencies;

public interface ICurrencyService
{
    Task<PagedResult<CurrencyDto>> ListAsync(PagedQuery query, CancellationToken ct = default);
    Task<IReadOnlyList<CurrencyDto>> ListAllActiveAsync(CancellationToken ct = default);
    Task<CurrencyDto?> GetAsync(long id, CancellationToken ct = default);
    Task<ServiceResult<CurrencyDto>> CreateAsync(CurrencyUpsertDto request, CancellationToken ct = default);
    Task<ServiceResult<CurrencyDto>> EditAsync(long id, CurrencyUpsertDto request, CancellationToken ct = default);
    Task<bool> DeleteAsync(long id, CancellationToken ct = default);
}
