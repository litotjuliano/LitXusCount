using LitXusCount.Domain.Entities;
using LitXusCount.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LitXusCount.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    public DbSet<CompanyInfo> CompanyInfos => Set<CompanyInfo>();
    public DbSet<Currency> Currencies => Set<Currency>();
    public DbSet<VatPercentage> VatPercentages => Set<VatPercentage>();
    public DbSet<EmailConfig> EmailConfigs => Set<EmailConfig>();
    public DbSet<PaymentType> PaymentTypes => Set<PaymentType>();
    public DbSet<PaymentStatus> PaymentStatuses => Set<PaymentStatus>();
    public DbSet<CustomerType> CustomerTypes => Set<CustomerType>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<UnitOfMeasure> UnitsOfMeasure => Set<UnitOfMeasure>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Token).HasMaxLength(512).IsRequired();
            entity.HasIndex(x => x.Token).IsUnique();
        });

        builder.Entity<Currency>(entity =>
        {
            entity.Property(x => x.Code).HasMaxLength(8).IsRequired();
            entity.HasIndex(x => x.Code).IsUnique().HasFilter("\"IsActive\" = true");
        });

        builder.Entity<VatPercentage>(entity =>
        {
            entity.Property(x => x.Name).IsRequired();
        });

        builder.Entity<EmailConfig>(entity =>
        {
            entity.Property(x => x.Email).IsRequired();
        });

        builder.Entity<PaymentType>(entity =>
        {
            entity.Property(x => x.Name).IsRequired();
            entity.HasIndex(x => x.Name).IsUnique().HasFilter("\"IsActive\" = true");
        });

        builder.Entity<PaymentStatus>(entity =>
        {
            entity.Property(x => x.Name).IsRequired();
            entity.HasIndex(x => x.Name).IsUnique().HasFilter("\"IsActive\" = true");
        });

        builder.Entity<CustomerType>(entity =>
        {
            entity.Property(x => x.Name).IsRequired();
            entity.HasIndex(x => x.Name).IsUnique().HasFilter("\"IsActive\" = true");
        });

        builder.Entity<Category>(entity =>
        {
            entity.Property(x => x.Name).IsRequired();
            entity.HasIndex(x => x.Name).IsUnique().HasFilter("\"IsActive\" = true");
        });

        builder.Entity<UnitOfMeasure>(entity =>
        {
            entity.Property(x => x.Name).IsRequired();
            entity.HasIndex(x => x.Name).IsUnique().HasFilter("\"IsActive\" = true");
        });

        builder.Entity<CompanyInfo>(entity =>
        {
            entity.Property(x => x.Name).IsRequired();
            entity.HasOne(x => x.Currency).WithMany().HasForeignKey(x => x.CurrencyId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.VatPercentage).WithMany().HasForeignKey(x => x.VatPercentageId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.EmailConfig).WithMany().HasForeignKey(x => x.EmailConfigId).OnDelete(DeleteBehavior.Restrict);
            entity.HasData(new CompanyInfo { Id = 1, Name = "My Company" });
        });
    }
}
