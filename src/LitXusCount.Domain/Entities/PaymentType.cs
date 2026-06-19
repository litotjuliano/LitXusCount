namespace LitXusCount.Domain.Entities;

public class PaymentType : AuditableEntity, ISimpleLookupEntity
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}
