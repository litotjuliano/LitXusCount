using Microsoft.AspNetCore.Identity;

namespace LitXusCount.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public string? DisplayName { get; set; }
    public long? TenantId { get; set; }
}
