namespace LitXusCount.Domain.Entities;

public class AccExpense : AuditableEntity
{
    public long AccAccountId { get; set; }
    public AccAccount AccAccount { get; set; } = null!;
    public string Name { get; set; } = null!;
    public DateTime ExpenseDate { get; set; }
    public decimal Amount { get; set; }
    public string? Note { get; set; }
}
