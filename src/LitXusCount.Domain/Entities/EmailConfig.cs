namespace LitXusCount.Domain.Entities;

public class EmailConfig : AuditableEntity
{
    public string Email { get; set; } = null!;
    public string PasswordEncrypted { get; set; } = null!;
    public string Hostname { get; set; } = null!;
    public int Port { get; set; }
    public bool SslEnabled { get; set; }
    public string? SenderFullName { get; set; }
    public bool IsDefault { get; set; }
}
