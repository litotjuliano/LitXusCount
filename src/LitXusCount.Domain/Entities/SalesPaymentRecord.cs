namespace LitXusCount.Domain.Entities;

public class SalesPaymentRecord : AuditableEntity
{
    public long SalesInvoiceId { get; set; }
    public SalesInvoice SalesInvoice { get; set; } = null!;

    public long AccAccountId { get; set; }
    public AccAccount AccAccount { get; set; } = null!;

    public string ModeOfPayment { get; set; } = null!;
    public decimal Amount { get; set; }
    public string? ReferenceNo { get; set; }
}
