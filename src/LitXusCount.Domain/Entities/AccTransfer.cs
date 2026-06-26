namespace LitXusCount.Domain.Entities;

public class AccTransfer : AuditableEntity
{
    public long SenderAccountId { get; set; }
    public AccAccount SenderAccount { get; set; } = null!;
    public long ReceiverAccountId { get; set; }
    public AccAccount ReceiverAccount { get; set; } = null!;
    public DateTime TransferDate { get; set; }
    public decimal Amount { get; set; }
    public string? Note { get; set; }
}
