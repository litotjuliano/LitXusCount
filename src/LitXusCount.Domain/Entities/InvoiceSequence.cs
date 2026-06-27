using LitXusCount.Domain.Enums;

namespace LitXusCount.Domain.Entities;

public class InvoiceSequence
{
    public SalesInvoiceCategory Category { get; set; }
    public int LastNumber { get; set; }
}
