namespace LitXusCount.Domain.Entities;

public class AccAccount : AuditableEntity
{
    public string Code { get; set; } = null!;
    public string AccountName { get; set; } = null!;
    public string? AccountNumber { get; set; }
    public string? Description { get; set; }
    public decimal Credit { get; set; }
    public decimal Debit { get; set; }
    public decimal Balance { get; set; }
}
