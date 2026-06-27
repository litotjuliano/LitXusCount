namespace LitXusCount.Domain.Entities;

public class EInvoiceSubmission
{
    public long Id { get; set; }
    public long SalesInvoiceId { get; set; }
    public SalesInvoice SalesInvoice { get; set; } = null!;

    public string? SubmissionUid { get; set; }
    public string? ResponseStatus { get; set; }
    public string? ResponseUUID { get; set; }
    public string? ResponseLongId { get; set; }
    public string? RawResponseJson { get; set; }

    public DateTime SubmittedAt { get; set; }
    public DateTime? RespondedAt { get; set; }

    public ICollection<EInvoiceValidationError> ValidationErrors { get; set; } = new List<EInvoiceValidationError>();
}
