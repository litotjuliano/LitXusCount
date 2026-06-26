using LitXusCount.Application.Common;

namespace LitXusCount.Application.Sales;

public interface ISalesInvoiceLineService
{
    Task<ServiceResult<SalesInvoiceLineDto>> AddLineAsync(SalesInvoiceLineCreateDto request, CancellationToken ct = default);
    Task<bool> RemoveLineAsync(long lineId, CancellationToken ct = default);
    Task<IReadOnlyList<SalesInvoiceLineDto>> GetByInvoiceAsync(long invoiceId, CancellationToken ct = default);
}
