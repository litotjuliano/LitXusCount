namespace LitXusCount.Domain.Entities;

public class EInvoiceValidationError
{
    public long Id { get; set; }
    public long EInvoiceSubmissionId { get; set; }
    public EInvoiceSubmission EInvoiceSubmission { get; set; } = null!;

    public string ErrorCode { get; set; } = null!;
    public string ErrorMessage { get; set; } = null!;
    public string? PropertyPath { get; set; }
    public string? Target { get; set; }
}
