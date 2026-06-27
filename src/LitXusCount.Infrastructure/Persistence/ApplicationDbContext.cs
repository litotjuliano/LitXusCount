using LitXusCount.Domain.Entities;
using LitXusCount.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LitXusCount.Infrastructure.Persistence;

/// <summary>
/// Tenant-scoped operational context. Connection string is resolved per-request from
/// ITenantContext and injected via the DI factory in DependencyInjection.cs.
/// Each tenant has its own isolated PostgreSQL database.
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<CompanyInfo> CompanyInfos => Set<CompanyInfo>();
    public DbSet<Currency> Currencies => Set<Currency>();
    public DbSet<VatPercentage> VatPercentages => Set<VatPercentage>();
    public DbSet<EmailConfig> EmailConfigs => Set<EmailConfig>();
    public DbSet<PaymentType> PaymentTypes => Set<PaymentType>();
    public DbSet<PaymentStatus> PaymentStatuses => Set<PaymentStatus>();
    public DbSet<CustomerType> CustomerTypes => Set<CustomerType>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<UnitOfMeasure> UnitsOfMeasure => Set<UnitOfMeasure>();
    public DbSet<GlAccount> GlAccounts => Set<GlAccount>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Product> Products => Set<Product>();

    public DbSet<TaxCode> TaxCodes => Set<TaxCode>();
    public DbSet<EInvoiceSubmission> EInvoiceSubmissions => Set<EInvoiceSubmission>();
    public DbSet<EInvoiceValidationError> EInvoiceValidationErrors => Set<EInvoiceValidationError>();
    public DbSet<TenantReportTemplate> TenantReportTemplates => Set<TenantReportTemplate>();
    public DbSet<InvoiceSequence> InvoiceSequences => Set<InvoiceSequence>();
    public DbSet<SalesInvoice> SalesInvoices => Set<SalesInvoice>();
    public DbSet<SalesInvoiceLine> SalesInvoiceLines => Set<SalesInvoiceLine>();
    public DbSet<SalesPaymentRecord> SalesPaymentRecords => Set<SalesPaymentRecord>();
    public DbSet<ReturnLog> ReturnLogs => Set<ReturnLog>();

    public DbSet<AccAccount> AccAccounts => Set<AccAccount>();
    public DbSet<AccTransaction> AccTransactions => Set<AccTransaction>();
    public DbSet<AccDeposit> AccDeposits => Set<AccDeposit>();
    public DbSet<AccExpense> AccExpenses => Set<AccExpense>();
    public DbSet<AccTransfer> AccTransfers => Set<AccTransfer>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // ── Shared lookups ───────────────────────────────────────────────────
        builder.Entity<Currency>(entity =>
        {
            entity.Property(x => x.Code).HasMaxLength(8).IsRequired();
            entity.HasIndex(x => x.Code).IsUnique().HasFilter("\"IsActive\" = true");
        });

        builder.Entity<VatPercentage>(entity =>
        {
            entity.Property(x => x.Name).IsRequired();
            entity.Property(x => x.Percentage).HasPrecision(8, 4);
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

        // ── CompanyInfo (singleton per tenant DB, Id = 1) ───────────────────
        builder.Entity<CompanyInfo>(entity =>
        {
            entity.Property(x => x.Name).IsRequired();
            entity.HasOne(x => x.Currency).WithMany().HasForeignKey(x => x.CurrencyId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.VatPercentage).WithMany().HasForeignKey(x => x.VatPercentageId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.EmailConfig).WithMany().HasForeignKey(x => x.EmailConfigId).OnDelete(DeleteBehavior.Restrict);
            entity.HasData(new CompanyInfo { Id = 1, Name = "My Company" });
        });

        // ── EmailConfig ──────────────────────────────────────────────────────
        builder.Entity<EmailConfig>(entity =>
        {
            entity.Property(x => x.Email).IsRequired();
        });

        // ── GlAccount ────────────────────────────────────────────────────────
        builder.Entity<GlAccount>(entity =>
        {
            entity.Property(x => x.Code).HasMaxLength(32).IsRequired();
            entity.Property(x => x.Name).HasMaxLength(128).IsRequired();
            entity.HasIndex(x => x.Code).IsUnique().HasFilter("\"IsActive\" = true");
            entity.HasOne(x => x.Parent).WithMany().HasForeignKey(x => x.ParentId).OnDelete(DeleteBehavior.Restrict);
        });

        // ── Customer ─────────────────────────────────────────────────────────
        builder.Entity<Customer>(entity =>
        {
            entity.Property(x => x.Code).HasMaxLength(32).IsRequired();
            entity.Property(x => x.Name).HasMaxLength(128).IsRequired();
            entity.HasIndex(x => x.Code).IsUnique().HasFilter("\"IsActive\" = true");
            entity.HasOne(x => x.GlAccount).WithMany().HasForeignKey(x => x.GlAccountId).OnDelete(DeleteBehavior.Restrict);
        });

        // ── Supplier ─────────────────────────────────────────────────────────
        builder.Entity<Supplier>(entity =>
        {
            entity.Property(x => x.Code).HasMaxLength(32).IsRequired();
            entity.Property(x => x.Name).HasMaxLength(128).IsRequired();
            entity.HasIndex(x => x.Code).IsUnique().HasFilter("\"IsActive\" = true");
            entity.HasOne(x => x.GlAccount).WithMany().HasForeignKey(x => x.GlAccountId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.DefaultCurrency).WithMany().HasForeignKey(x => x.DefaultCurrencyId).OnDelete(DeleteBehavior.Restrict);
        });

        // ── Product ──────────────────────────────────────────────────────────
        builder.Entity<Product>(entity =>
        {
            entity.Property(x => x.Code).HasMaxLength(64).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(256);
            entity.HasIndex(x => x.Code).IsUnique().HasFilter("\"IsActive\" = true");
            entity.HasOne(x => x.Category).WithMany().HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.SalesCogsAccount).WithMany().HasForeignKey(x => x.SalesCogsAccountId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.SalesRevenueAccount).WithMany().HasForeignKey(x => x.SalesRevenueAccountId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.PurchaseCostAccount).WithMany().HasForeignKey(x => x.PurchaseCostAccountId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.PurchaseAccount).WithMany().HasForeignKey(x => x.PurchaseAccountId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.DefaultSupplier).WithMany().HasForeignKey(x => x.DefaultSupplierId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.MainUnitOfMeasure).WithMany().HasForeignKey(x => x.MainUnitOfMeasureId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.AltUnitOfMeasure).WithMany().HasForeignKey(x => x.AltUnitOfMeasureId).OnDelete(DeleteBehavior.Restrict);
        });

        // ── E-Invoice audit trail ─────────────────────────────────────────────
        builder.Entity<EInvoiceSubmission>(entity =>
        {
            entity.HasOne(x => x.SalesInvoice).WithMany().HasForeignKey(x => x.SalesInvoiceId).OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(x => x.SalesInvoiceId);
        });

        builder.Entity<EInvoiceValidationError>(entity =>
        {
            entity.Property(x => x.ErrorCode).HasMaxLength(64).IsRequired();
            entity.Property(x => x.ErrorMessage).HasMaxLength(512).IsRequired();
            entity.Property(x => x.PropertyPath).HasMaxLength(512);
            entity.HasOne(x => x.EInvoiceSubmission).WithMany(x => x.ValidationErrors)
                .HasForeignKey(x => x.EInvoiceSubmissionId).OnDelete(DeleteBehavior.Cascade);
        });

        // ── TaxCode (LHDN tax type codes 01–07, tenant-scoped) ───────────────
        builder.Entity<TaxCode>(entity =>
        {
            entity.Property(x => x.Code).HasMaxLength(2).IsRequired();
            entity.Property(x => x.Name).HasMaxLength(128).IsRequired();
            entity.Property(x => x.Rate).HasPrecision(8, 4);
            entity.HasIndex(x => x.Code).IsUnique().HasFilter("\"IsActive\" = true");
        });

        // ── Tenant report template overrides ─────────────────────────────────
        builder.Entity<TenantReportTemplate>(entity =>
        {
            entity.Property(x => x.DocumentType).HasMaxLength(64).IsRequired();
            entity.Property(x => x.Name).HasMaxLength(128).IsRequired();
            entity.HasIndex(x => new { x.DocumentType, x.IsDefault })
                .HasFilter("\"IsDefault\" = true AND \"IsActive\" = true");
        });

        // ── Invoice sequences (serialised number generation per category) ────
        builder.Entity<InvoiceSequence>(entity =>
        {
            entity.HasKey(x => x.Category);
        });

        // ── Sales ────────────────────────────────────────────────────────────
        builder.Entity<SalesInvoice>(entity =>
        {
            entity.Property(x => x.InvoiceNo).HasMaxLength(64).IsRequired();
            entity.HasIndex(x => x.InvoiceNo).IsUnique().HasFilter("\"IsActive\" = true");
            entity.Property(x => x.SubTotal).HasPrecision(18, 4);
            entity.Property(x => x.DiscountAmount).HasPrecision(18, 4);
            entity.Property(x => x.VATAmount).HasPrecision(18, 4);
            entity.Property(x => x.GrandTotal).HasPrecision(18, 4);
            entity.Property(x => x.PaidAmount).HasPrecision(18, 4);
            entity.Property(x => x.DueAmount).HasPrecision(18, 4);
            entity.HasIndex(x => x.Category);
            entity.HasIndex(x => x.CreatedAt);
            entity.HasOne(x => x.Customer).WithMany().HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.Currency).WithMany().HasForeignKey(x => x.CurrencyId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<SalesInvoiceLine>(entity =>
        {
            entity.Property(x => x.ItemName).HasMaxLength(256).IsRequired();
            entity.Property(x => x.Quantity).HasPrecision(18, 4);
            entity.Property(x => x.UnitPrice).HasPrecision(18, 4);
            entity.Property(x => x.ItemVAT).HasPrecision(18, 4);
            entity.Property(x => x.ItemVATAmount).HasPrecision(18, 4);
            entity.Property(x => x.ItemDiscount).HasPrecision(18, 4);
            entity.Property(x => x.ItemDiscountAmount).HasPrecision(18, 4);
            entity.Property(x => x.TotalAmount).HasPrecision(18, 4);
            entity.HasOne(x => x.SalesInvoice).WithMany(x => x.Lines).HasForeignKey(x => x.SalesInvoiceId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.Product).WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<SalesPaymentRecord>(entity =>
        {
            entity.Property(x => x.ModeOfPayment).HasMaxLength(64).IsRequired();
            entity.Property(x => x.Amount).HasPrecision(18, 4);
            entity.Property(x => x.ReferenceNo).HasMaxLength(128);
            entity.HasOne(x => x.SalesInvoice).WithMany(x => x.Payments).HasForeignKey(x => x.SalesInvoiceId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.AccAccount).WithMany().HasForeignKey(x => x.AccAccountId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<ReturnLog>(entity =>
        {
            entity.Property(x => x.InvoiceNo).HasMaxLength(64).IsRequired();
            entity.HasOne(x => x.SalesInvoice).WithMany().HasForeignKey(x => x.SalesInvoiceId).OnDelete(DeleteBehavior.Restrict);
        });

        // ── Accounts ─────────────────────────────────────────────────────────
        builder.Entity<AccAccount>(entity =>
        {
            entity.Property(x => x.Code).HasMaxLength(32).IsRequired();
            entity.Property(x => x.AccountName).HasMaxLength(128).IsRequired();
            entity.Property(x => x.AccountNumber).HasMaxLength(64);
            entity.Property(x => x.Credit).HasPrecision(18, 4);
            entity.Property(x => x.Debit).HasPrecision(18, 4);
            entity.Property(x => x.Balance).HasPrecision(18, 4);
            entity.HasIndex(x => x.Code).IsUnique().HasFilter("\"IsActive\" = true");
        });

        builder.Entity<AccTransaction>(entity =>
        {
            entity.Property(x => x.Amount).HasPrecision(18, 4);
            entity.Property(x => x.Credit).HasPrecision(18, 4);
            entity.Property(x => x.Debit).HasPrecision(18, 4);
            entity.Property(x => x.Reference).HasMaxLength(64);
            entity.HasOne(x => x.AccAccount).WithMany().HasForeignKey(x => x.AccAccountId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<AccDeposit>(entity =>
        {
            entity.Property(x => x.Amount).HasPrecision(18, 4);
            entity.HasOne(x => x.AccAccount).WithMany().HasForeignKey(x => x.AccAccountId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<AccExpense>(entity =>
        {
            entity.Property(x => x.Name).HasMaxLength(128).IsRequired();
            entity.Property(x => x.Amount).HasPrecision(18, 4);
            entity.HasOne(x => x.AccAccount).WithMany().HasForeignKey(x => x.AccAccountId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<AccTransfer>(entity =>
        {
            entity.Property(x => x.Amount).HasPrecision(18, 4);
            entity.HasOne(x => x.SenderAccount).WithMany().HasForeignKey(x => x.SenderAccountId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.ReceiverAccount).WithMany().HasForeignKey(x => x.ReceiverAccountId).OnDelete(DeleteBehavior.Restrict);
        });
    }
}
