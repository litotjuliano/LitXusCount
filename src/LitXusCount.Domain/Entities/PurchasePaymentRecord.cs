namespace LitXusCount.Domain.Entities;

public class PurchasePaymentRecord : AuditableEntity
{
    public long PurchaseInvoiceId { get; set; }
    public PurchaseInvoice PurchaseInvoice { get; set; } = null!;

    public long AccAccountId { get; set; }
    public AccAccount AccAccount { get; set; } = null!;

    public string ModeOfPayment { get; set; } = null!;
    public decimal Amount { get; set; }
    public string? ReferenceNo { get; set; }
}
