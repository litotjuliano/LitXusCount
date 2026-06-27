namespace LitXusCount.Domain.Entities;

public class SalesInvoiceLine : AuditableEntity
{
    public long SalesInvoiceId { get; set; }
    public SalesInvoice SalesInvoice { get; set; } = null!;

    public long ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public string ItemName { get; set; } = null!;

    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal ItemVAT { get; set; }
    public decimal ItemVATAmount { get; set; }
    public decimal ItemDiscount { get; set; }
    public decimal ItemDiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }

    public bool IsReturn { get; set; }

    // ── LHDN UBL line-level fields ────────────────────────────────────────────
    public string? ClassificationCode { get; set; }
    public string? TaxTypeCode { get; set; }
    public string? TaxExemptionReason { get; set; }
    public decimal? TaxExemptionAmount { get; set; }
    public string? UnitCode { get; set; }
}
