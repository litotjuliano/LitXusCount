namespace LitXusCount.Domain.Entities;

public class VatPercentage : AuditableEntity
{
    public string Name { get; set; } = null!;
    public decimal Percentage { get; set; }
    public bool IsDefault { get; set; }
}
