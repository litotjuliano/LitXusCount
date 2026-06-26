namespace LitXusCount.Domain.Entities;

public enum GlAccountType
{
    Asset = 1,
    Liability = 2,
    Equity = 3,
    Revenue = 4,
    Expense = 5,
    Cogs = 6
}

public class GlAccount : AuditableEntity
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public GlAccountType AccountType { get; set; }
    public long? ParentId { get; set; }
    public GlAccount? Parent { get; set; }
    public bool IsControl { get; set; }
    public decimal OpeningBalance { get; set; }
}
