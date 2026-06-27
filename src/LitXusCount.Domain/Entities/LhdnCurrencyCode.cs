namespace LitXusCount.Domain.Entities;

public class LhdnCurrencyCode : AuditableEntity
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
}
