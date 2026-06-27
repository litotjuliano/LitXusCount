namespace LitXusCount.Domain.Entities;

public class LhdnClassificationCode : AuditableEntity
{
    public string Code { get; set; } = null!;
    public string Description { get; set; } = null!;
}
