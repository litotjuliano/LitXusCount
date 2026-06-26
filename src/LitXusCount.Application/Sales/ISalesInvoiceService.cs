using LitXusCount.Application.Common;
using LitXusCount.Domain.Enums;

namespace LitXusCount.Application.Sales;

public interface ISalesInvoiceService
{
    Task<ServiceResult<SalesInvoiceDto>> CreateDraftAsync(SalesInvoiceCreateDto request, CancellationToken ct = default);
    Task<ServiceResult<SalesInvoiceDto>> UpdateHeaderAsync(long invoiceId, SalesInvoiceHeaderUpdateDto request, CancellationToken ct = default);
    Task<ServiceResult<SalesInvoiceDto>> PromoteAsync(long invoiceId, SalesInvoiceCategory targetCategory, CancellationToken ct = default);
    Task<PagedResult<SalesInvoiceDto>> GetPagedAsync(PagedQuery query, SalesInvoiceCategory? categoryFilter = null, CancellationToken ct = default);
    Task<SalesInvoiceDto?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<bool> DeleteAsync(long id, CancellationToken ct = default);
}
