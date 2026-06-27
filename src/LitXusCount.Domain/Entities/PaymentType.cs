namespace LitXusCount.Domain.Entities;

public class PaymentCode : AuditableEntity, ISimpleLookupEntity
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? Code { get; set; }
}
