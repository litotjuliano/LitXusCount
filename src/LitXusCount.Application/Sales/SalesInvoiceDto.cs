using LitXusCount.Domain.Enums;

namespace LitXusCount.Application.Sales;

public record SalesInvoiceDto(
    long Id,
    long CustomerId,
    string CustomerName,
    SalesInvoiceCategory Category,
    string InvoiceNo,
    decimal SubTotal,
    decimal DiscountAmount,
    decimal VATAmount,
    decimal GrandTotal,
    decimal PaidAmount,
    decimal DueAmount,
    SalesPaymentStatus PaymentStatus,
    string? PurchaseOrderNumber,
    string? Notes,
    long? CurrencyId,
    bool IsActive,
    DateTime CreatedAt,
    string InvoiceTypeCode
);

public record SalesInvoiceCreateDto(
    long CustomerId,
    long? CurrencyId
);

public record SalesInvoiceHeaderUpdateDto(
    long CustomerId,
    string? PurchaseOrderNumber,
    string? Notes,
    long? CurrencyId,
    string? InvoiceTypeCode
);

public record SalesInvoicePromoteDto(
    SalesInvoiceCategory TargetCategory
);
