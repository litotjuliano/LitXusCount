namespace LitXusCount.Domain.Entities;

public class ReturnLog : AuditableEntity
{
    public long SalesInvoiceId { get; set; }
    public SalesInvoice SalesInvoice { get; set; } = null!;

    public string InvoiceNo { get; set; } = null!;
    public long CustomerId { get; set; }
    public string? TranType { get; set; }
    public string? Note { get; set; }
}
