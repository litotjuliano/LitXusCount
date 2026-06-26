namespace LitXusCount.Domain.Entities;

public class AccDeposit : AuditableEntity
{
    public long AccAccountId { get; set; }
    public AccAccount AccAccount { get; set; } = null!;
    public DateTime DepositDate { get; set; }
    public decimal Amount { get; set; }
    public string? Note { get; set; }
}
