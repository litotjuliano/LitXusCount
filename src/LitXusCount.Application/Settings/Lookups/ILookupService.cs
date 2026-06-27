using LitXusCount.Application.Common;

namespace LitXusCount.Application.Settings.Lookups;

/// <summary>
/// Shared method shape for the five Name+Description lookup entities (PaymentCode, PaymentStatus,
/// CustomerType, Category, UnitOfMeasure). Each entity gets its own marker interface below so DI
/// registration and controller dependencies stay entity-specific rather than depending on a
/// generic, type-parameterized repository/service.
/// </summary>
public interface ILookupService
{
    Task<PagedResult<LookupItemDto>> ListAsync(PagedQuery query, CancellationToken ct = default);
    Task<IReadOnlyList<LookupItemDto>> ListAllActiveAsync(CancellationToken ct = default);
    Task<LookupItemDto?> GetAsync(long id, CancellationToken ct = default);
    Task<ServiceResult<LookupItemDto>> CreateAsync(LookupItemUpsertDto request, CancellationToken ct = default);
    Task<ServiceResult<LookupItemDto>> EditAsync(long id, LookupItemUpsertDto request, CancellationToken ct = default);
    Task<bool> DeleteAsync(long id, CancellationToken ct = default);
}

public interface IPaymentCodeService : ILookupService;

public interface IPaymentStatusService : ILookupService;

public interface ICustomerTypeService : ILookupService;

public interface ICategoryService : ILookupService;

public interface IUnitOfMeasureService : ILookupService;
