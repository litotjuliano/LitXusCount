namespace LitXusCount.Domain.Entities;

public class LhdnCountry : AuditableEntity
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
}
