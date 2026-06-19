namespace LitXusCount.Application.Settings.EmailConfigs;

/// <summary>No password field — never returned to a caller.</summary>
public record EmailConfigDto(long Id, string Email, string Hostname, int Port, bool SslEnabled, string? SenderFullName, bool IsDefault, bool IsActive);

public record EmailConfigUpsertDto(string Email, string Password, string Hostname, int Port, bool SslEnabled, string? SenderFullName);
