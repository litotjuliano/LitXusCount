namespace LitXusCount.Domain.Entities;

public class Tenant : AuditableEntity
{
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? ContactEmail { get; set; }
    public string? Notes { get; set; }
    public string ConnectionString { get; set; } = null!;
}
