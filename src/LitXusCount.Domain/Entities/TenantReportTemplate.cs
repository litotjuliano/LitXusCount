namespace LitXusCount.Domain.Entities;

public class TenantReportTemplate : AuditableEntity
{
    public string DocumentType { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string BodyHtml { get; set; } = null!;
    public bool IsDefault { get; set; }
}
