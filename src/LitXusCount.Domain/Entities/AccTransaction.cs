using LitXusCount.Domain.Enums;

namespace LitXusCount.Domain.Entities;

// Append-only audit log — does NOT inherit AuditableEntity (no soft-delete, no modify fields)
public class AccTransaction
{
    public long Id { get; set; }
    public long AccAccountId { get; set; }
    public AccAccount AccAccount { get; set; } = null!;
    public AccTransactionType Type { get; set; }
    public decimal Credit { get; set; }
    public decimal Debit { get; set; }
    public decimal Amount { get; set; }
    public string? Reference { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}
