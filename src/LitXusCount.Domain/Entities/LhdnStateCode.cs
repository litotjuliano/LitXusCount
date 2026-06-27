namespace LitXusCount.Domain.Entities;

public class LhdnStateCode : AuditableEntity
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
}
