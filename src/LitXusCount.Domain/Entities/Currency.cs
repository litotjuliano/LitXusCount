namespace LitXusCount.Domain.Entities;

public class Currency : AuditableEntity
{
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public string? Symbol { get; set; }
    public string? Country { get; set; }
    public string? Description { get; set; }
    public bool IsDefault { get; set; }
}
