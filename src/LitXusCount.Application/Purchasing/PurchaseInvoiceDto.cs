using LitXusCount.Domain.Enums;

namespace LitXusCount.Application.Purchasing;

public record PurchaseInvoiceDto(
    long Id,
    long SupplierId,
    string SupplierName,
    PurchaseInvoiceCategory Category,
    string InvoiceNo,
    decimal SubTotal,
    decimal DiscountAmount,
    decimal VATAmount,
    decimal GrandTotal,
    decimal PaidAmount,
    decimal DueAmount,
    SalesPaymentStatus PaymentStatus,
    string? Notes,
    long? CurrencyId,
    bool IsActive,
    DateTime CreatedAt
);

public record PurchaseInvoiceCreateDto(
    long SupplierId,
    PurchaseInvoiceCategory Category,
    long? CurrencyId
);

public record PurchaseInvoiceHeaderUpdateDto(
    long SupplierId,
    string? Notes,
    long? CurrencyId
);

public record PurchaseInvoiceLineDto(
    long Id,
    long PurchaseInvoiceId,
    long ProductId,
    string ItemName,
    decimal Quantity,
    decimal UnitPrice,
    decimal ItemVAT,
    decimal ItemVATAmount,
    decimal ItemDiscount,
    decimal ItemDiscountAmount,
    decimal TotalAmount,
    bool IsReturn,
    bool IsActive
);

public record PurchaseInvoiceLineCreateDto(
    long PurchaseInvoiceId,
    long ProductId,
    decimal Quantity,
    decimal UnitPrice,
    decimal ItemVAT,
    decimal ItemDiscount
);

public record PurchasePaymentRecordDto(
    long Id,
    long PurchaseInvoiceId,
    long AccAccountId,
    string AccAccountName,
    string ModeOfPayment,
    decimal Amount,
    string? ReferenceNo,
    bool IsActive
);

public record PurchasePaymentRecordCreateDto(
    long PurchaseInvoiceId,
    long AccAccountId,
    string ModeOfPayment,
    decimal Amount,
    string? ReferenceNo
);
