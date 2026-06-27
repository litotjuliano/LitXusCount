namespace LitXusCount.Application.Sales;

public record SalesInvoiceLineDto(
    long Id,
    long SalesInvoiceId,
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
    bool IsActive,
    string? ClassificationCode,
    string? TaxTypeCode,
    string? UnitCode
);

public record SalesInvoiceLineCreateDto(
    long SalesInvoiceId,
    long ProductId,
    decimal Quantity,
    decimal UnitPrice,
    decimal ItemVAT,
    decimal ItemDiscount,
    string? ClassificationCode = null,
    string? TaxTypeCode = null
);
