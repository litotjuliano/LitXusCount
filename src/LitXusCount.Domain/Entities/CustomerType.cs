namespace LitXusCount.Domain.Entities;

public class CustomerType : AuditableEntity, ISimpleLookupEntity
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}
