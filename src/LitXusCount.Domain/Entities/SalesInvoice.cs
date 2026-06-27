using LitXusCount.Domain.Enums;

namespace LitXusCount.Domain.Entities;

public class SalesInvoice : AuditableEntity
{
    public long CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public SalesInvoiceCategory Category { get; set; }
    public string InvoiceNo { get; set; } = null!;

    public decimal SubTotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal VATAmount { get; set; }
    public decimal GrandTotal { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal DueAmount { get; set; }

    public SalesPaymentStatus PaymentStatus { get; set; }

    public string? PurchaseOrderNumber { get; set; }
    public string? Notes { get; set; }

    public long? CurrencyId { get; set; }
    public Currency? Currency { get; set; }

    // ── Buyer snapshot (frozen at promotion time) ─────────────────────────────
    public string? BuyerName { get; set; }
    public string? BuyerTIN { get; set; }
    public string? BuyerRegistrationType { get; set; }
    public string? BuyerAddress { get; set; }

    // ── Foreign-currency support ──────────────────────────────────────────────
    public decimal? FxRate { get; set; }
    public decimal? GrandTotalMyr { get; set; }

    // ── LHDN MyInvois e-invoice fields ────────────────────────────────────────
    public EInvoiceStatus EInvoiceStatus { get; set; } = EInvoiceStatus.None;
    public string? EInvoiceUUID { get; set; }
    public string? EInvoiceLongId { get; set; }
    public string? EInvoiceSubmissionUid { get; set; }
    public DateTime? EInvoiceDateTimeSubmitted { get; set; }
    public DateTime? EInvoiceDateTimeValidated { get; set; }

    public string InvoiceTypeCode { get; set; } = "01";
    public DateOnly? InvoiceIssueDate { get; set; }
    public TimeOnly? InvoiceIssueTime { get; set; }

    public bool IsConsolidated { get; set; }
    public DateOnly? BillingPeriodStart { get; set; }
    public DateOnly? BillingPeriodEnd { get; set; }

    public string? OriginalInvoiceUUID { get; set; }

    public ICollection<SalesInvoiceLine> Lines { get; set; } = new List<SalesInvoiceLine>();
    public ICollection<SalesPaymentRecord> Payments { get; set; } = new List<SalesPaymentRecord>();
}
