namespace LitXusCount.Application.Sales;

public record SalesPaymentRecordDto(
    long Id,
    long SalesInvoiceId,
    long AccAccountId,
    string AccountName,
    string ModeOfPayment,
    decimal Amount,
    string? ReferenceNo,
    bool IsActive
);

public record SalesPaymentRecordCreateDto(
    long SalesInvoiceId,
    long AccAccountId,
    string ModeOfPayment,
    decimal Amount,
    string? ReferenceNo
);
