using LitXusCount.Application.Common;

namespace LitXusCount.Application.Sales;

public interface ISalesPaymentService
{
    Task<ServiceResult<SalesPaymentRecordDto>> RecordPaymentAsync(SalesPaymentRecordCreateDto request, CancellationToken ct = default);
    Task<bool> DeletePaymentAsync(long paymentId, CancellationToken ct = default);
    Task<IReadOnlyList<SalesPaymentRecordDto>> GetByInvoiceAsync(long invoiceId, CancellationToken ct = default);
}
