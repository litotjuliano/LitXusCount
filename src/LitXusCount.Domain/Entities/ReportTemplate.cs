namespace LitXusCount.Domain.Entities;

public class ReportTemplate
{
    public long Id { get; set; }
    public string DocumentType { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string BodyHtml { get; set; } = null!;
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? ModifiedBy { get; set; }
}
