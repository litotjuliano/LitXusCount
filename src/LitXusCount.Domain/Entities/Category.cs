namespace LitXusCount.Domain.Entities;

public class Category : AuditableEntity, ISimpleLookupEntity
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}
