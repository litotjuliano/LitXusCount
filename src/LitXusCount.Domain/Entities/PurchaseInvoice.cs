using LitXusCount.Domain.Enums;

namespace LitXusCount.Domain.Entities;

public class PurchaseInvoice : AuditableEntity
{
    public long SupplierId { get; set; }
    public Supplier Supplier { get; set; } = null!;

    public PurchaseInvoiceCategory Category { get; set; }
    public string InvoiceNo { get; set; } = null!;

    public decimal SubTotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal VATAmount { get; set; }
    public decimal GrandTotal { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal DueAmount { get; set; }

    public SalesPaymentStatus PaymentStatus { get; set; }

    public string? Notes { get; set; }

    public long? CurrencyId { get; set; }
    public Currency? Currency { get; set; }

    public ICollection<PurchaseInvoiceLine> Lines { get; set; } = new List<PurchaseInvoiceLine>();
    public ICollection<PurchasePaymentRecord> Payments { get; set; } = new List<PurchasePaymentRecord>();
}
