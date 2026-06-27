namespace LitXusCount.Domain.Entities;

public class UnitOfMeasure : AuditableEntity, ISimpleLookupEntity
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? UnCefactCode { get; set; }
}
