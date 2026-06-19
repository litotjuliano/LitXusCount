using LitXusCount.Application.Settings.EmailConfigs;
using LitXusCount.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LitXusCount.Infrastructure.Persistence.Seeding;

/// <summary>
/// Dev-only reference data ported from InventoryMSNV's SeedData.cs, adapted to LitXusCount's
/// schema (IsActive audit flag, encrypted EmailConfig password). Skips entirely if Currencies
/// already has rows, so it only ever runs once against a fresh database.
/// </summary>
public static class SystemSettingsSeeder
{
    public static async Task SeedAsync(IServiceProvider services, CancellationToken ct = default)
    {
        var db = services.GetRequiredService<ApplicationDbContext>();

        if (await db.Currencies.AnyAsync(ct))
        {
            return;
        }

        var now = DateTime.UtcNow;

        var myr = new Currency { Name = "Ringgit Malaysia", Code = "MYR", Symbol = "RM", Country = "Malaysia", Description = "Ringgit Malaysia", IsDefault = true, IsActive = true, CreatedAt = now };
        var usd = new Currency { Name = "US Dollar", Code = "USD", Symbol = "$", Country = "United States", Description = "US Dollar", IsDefault = false, IsActive = true, CreatedAt = now };
        var sgd = new Currency { Name = "SGD Singapore", Code = "SGD", Symbol = "SGD", Country = "Singapore", Description = "SGD Singapore", IsDefault = false, IsActive = true, CreatedAt = now };
        db.Currencies.AddRange(myr, usd, sgd);

        var vatPercentages = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 20, 30, 40, 50 }
            .Select(p => new VatPercentage { Name = $"VAT: {p}%", Percentage = p, IsDefault = p == 5, IsActive = true, CreatedAt = now })
            .ToList();
        db.VatPercentages.AddRange(vatPercentages);
        var defaultVat = vatPercentages.Single(v => v.IsDefault);

        db.PaymentTypes.AddRange(
            new[] { "Cash", "Bank", "POS", "Mobile-Banking", "Credit Card", "Debit Card", "Other" }
                .Select(name => new PaymentType { Name = name, Description = name, IsActive = true, CreatedAt = now }));

        db.PaymentStatuses.AddRange(
            new[] { "Paid", "UnPaid", "Partially Paid", "Deposit", "Pay within 7 Days", "Pay within 14 Days", "Pay within 30 Days", "Custom Date" }
                .Select(name => new PaymentStatus { Name = name, Description = name, IsActive = true, CreatedAt = now }));

        db.CustomerTypes.AddRange(
            new[] { "Normal", "Premium", "Trader", "Other" }
                .Select(name => new CustomerType { Name = name, Description = name, IsActive = true, CreatedAt = now }));

        db.Categories.AddRange(new[]
        {
            ("Fruits", "Fruits Item"),
            ("Dairy Products", "Dairy Products"),
            ("Beverages", "Soft drinks, coffees, teas, etc"),
            ("Freezer", "Freezer"),
            ("Meat & Fish", "Meat & Fish"),
            ("Vegetables", "Vegetables"),
            ("Beauty and Cosmetic", "Beauty and Cosmetic"),
            ("IT", "IT"),
            ("Electronics", "Electronics"),
            ("Steels", "Coated Steel Sheet"),
            ("Common", "For common all items"),
        }.Select(c => new Category { Name = c.Item1, Description = c.Item2, IsActive = true, CreatedAt = now }));

        db.UnitsOfMeasure.AddRange(new[]
        {
            ("Kg", "Kilogram"), ("gr", "Milligram"), ("qt", "Quintal"), ("t", "Tonne"),
            ("L", "Liter"), ("ML", "Milliliter"), ("bbl", "Barrel"), ("gl", "Gallon"),
            ("Meter", "Meter"), ("Centimeter", "Centimeter"), ("Kilometer", "Kilometer"),
            ("Foot", "Foot"), ("Inch", "Inch"), ("Piece", "Piece"),
        }.Select(u => new UnitOfMeasure { Name = u.Item1, Description = u.Item2, IsActive = true, CreatedAt = now }));

        var encryptor = services.GetRequiredService<IEmailConfigEncryptor>();
        var primaryEmailConfig = new EmailConfig
        {
            Email = "dev01@gmail.com",
            PasswordEncrypted = encryptor.Encrypt("ChangeMe123!"),
            Hostname = "smtp.gmail.com",
            Port = 587,
            SslEnabled = true,
            SenderFullName = "LitXusCount",
            IsDefault = true,
            IsActive = true,
            CreatedAt = now,
        };
        db.EmailConfigs.AddRange(
            primaryEmailConfig,
            new EmailConfig { Email = "dev02@gmail.com", PasswordEncrypted = encryptor.Encrypt("ChangeMe123!"), Hostname = "smtp.gmail.com", Port = 587, SslEnabled = false, SenderFullName = "LitXusCount", IsDefault = false, IsActive = true, CreatedAt = now },
            new EmailConfig { Email = "dev03@gmail.com", PasswordEncrypted = encryptor.Encrypt("ChangeMe123!"), Hostname = "smtp.gmail.com", Port = 587, SslEnabled = false, SenderFullName = "LitXusCount", IsDefault = false, IsActive = true, CreatedAt = now },
            new EmailConfig { Email = "dev04@gmail.com", PasswordEncrypted = encryptor.Encrypt("ChangeMe123!"), Hostname = "smtp.gmail.com", Port = 587, SslEnabled = false, SenderFullName = "LitXusCount", IsDefault = false, IsActive = true, CreatedAt = now });

        await db.SaveChangesAsync(ct);

        var companyInfo = await db.CompanyInfos.SingleAsync(x => x.Id == 1, ct);
        companyInfo.Name = "XYZ Company Limited";
        companyInfo.LogoUrl = "/images/apple-touch-icon.png";
        companyInfo.Address = "Ipoh";
        companyInfo.City = "Perak";
        companyInfo.Country = "Malaysia";
        companyInfo.PostCode = "P123";
        companyInfo.Phone = "132546789";
        companyInfo.Mobile = "123456789";
        companyInfo.Fax = "123";
        companyInfo.Website = "www.allconnect.com.my";
        companyInfo.CompanyRegistrationNumber = "831207125546";
        companyInfo.VatRegistrationNumber = "V123456";
        companyInfo.InvoiceNumberPrefix = "INV";
        companyInfo.QuoteNumberPrefix = "QTO";
        companyInfo.TermsAndConditions = "Terms and Conditions apply to all sales made through this system. " +
            "By placing an order, the customer agrees to these terms. Prices are subject to change without notice.";
        companyInfo.IsVatEnabled = true;
        companyInfo.VatTitle = "VAT(%)";
        companyInfo.IsItemDiscountPercentage = true;
        companyInfo.CurrencyId = myr.Id;
        companyInfo.VatPercentageId = defaultVat.Id;
        companyInfo.EmailConfigId = primaryEmailConfig.Id;
        companyInfo.ModifiedAt = now;

        await db.SaveChangesAsync(ct);
    }
}
