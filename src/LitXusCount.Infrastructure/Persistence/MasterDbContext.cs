using LitXusCount.Domain.Entities;
using LitXusCount.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LitXusCount.Infrastructure.Persistence;

/// <summary>
/// Platform-level context: Identity (users, roles, claims), Tenants, RefreshTokens.
/// Always connects to the master database (DefaultConnection).
/// </summary>
public class MasterDbContext : IdentityDbContext<ApplicationUser>
{
    public MasterDbContext(DbContextOptions<MasterDbContext> options) : base(options) { }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Tenant>(entity =>
        {
            entity.Property(x => x.Name).HasMaxLength(256).IsRequired();
            entity.Property(x => x.Slug).HasMaxLength(64).IsRequired();
            entity.Property(x => x.ConnectionString).IsRequired();
            entity.HasIndex(x => x.Slug).IsUnique();
        });

        builder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Token).HasMaxLength(512).IsRequired();
            entity.HasIndex(x => x.Token).IsUnique();
        });
    }
}
