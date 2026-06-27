namespace LitXusCount.Domain.Entities;

public class LhdnMsicCode : AuditableEntity
{
    public string Code { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string? Category { get; set; }
}
