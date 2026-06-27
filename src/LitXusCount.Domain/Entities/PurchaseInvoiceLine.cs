namespace LitXusCount.Domain.Entities;

public class PurchaseInvoiceLine : AuditableEntity
{
    public long PurchaseInvoiceId { get; set; }
    public PurchaseInvoice PurchaseInvoice { get; set; } = null!;

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
}
