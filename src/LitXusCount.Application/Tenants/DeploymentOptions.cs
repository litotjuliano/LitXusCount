namespace LitXusCount.Application.Tenants;

public sealed class DeploymentOptions
{
    public const string SectionName = "Deployment";

    /// <summary>"SaaS" (multi-tenant hosted) or "Dedicated" (single-tenant on-premises/self-hosted).</summary>
    public string Mode { get; set; } = "Dedicated";

    public bool IsSaaS => string.Equals(Mode, "SaaS", StringComparison.OrdinalIgnoreCase);
    public bool IsDedicated => !IsSaaS;
}
