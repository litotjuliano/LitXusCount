namespace LitXusCount.Domain.Entities;

public class TaxCode : AuditableEntity
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Rate { get; set; }
    public bool IsExempt { get; set; }
    public bool IsDefault { get; set; }
}
