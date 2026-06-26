using LitXusCount.Application.Settings.EmailConfigs;
using LitXusCount.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LitXusCount.Infrastructure.Persistence.Seeding;

public static class SystemSettingsSeeder
{
    public static async Task SeedAsync(
        ApplicationDbContext db,
        IEmailConfigEncryptor encryptor,
        CancellationToken ct = default)
    {
        if (await db.Currencies.AnyAsync(ct)) return;

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

        var glAccounts = new[]
        {
            new GlAccount { Code = "11000", Name = "Accounts Receivable", AccountType = GlAccountType.Asset, IsActive = true, CreatedAt = now },
            new GlAccount { Code = "21000", Name = "Accounts Payable", AccountType = GlAccountType.Liability, IsActive = true, CreatedAt = now },
            new GlAccount { Code = "41000", Name = "Sales Revenue", AccountType = GlAccountType.Revenue, IsActive = true, CreatedAt = now },
            new GlAccount { Code = "51000", Name = "Cost of Goods Sold", AccountType = GlAccountType.Cogs, IsActive = true, CreatedAt = now },
            new GlAccount { Code = "52000", Name = "Purchase Cost Account", AccountType = GlAccountType.Expense, IsActive = true, CreatedAt = now }
        };
        db.GlAccounts.AddRange(glAccounts);
        await db.SaveChangesAsync(ct);

        var arAcc = glAccounts.First(a => a.Code == "11000");
        var apAcc = glAccounts.First(a => a.Code == "21000");
        var revenueAcc = glAccounts.First(a => a.Code == "41000");
        var cogsAcc = glAccounts.First(a => a.Code == "51000");
        var purchaseCostAcc = glAccounts.First(a => a.Code == "52000");

        var customers = new[]
        {
            new Customer { Code = "CUST001", Name = "Apex Retailers Ltd", GlAccountId = arAcc.Id, PaymentTermsDays = 30, CreditLimit = 50000, IsLocked = false, Address1 = "12 Main St", City = "Ipoh", Country = "Malaysia", IsActive = true, CreatedAt = now },
            new Customer { Code = "CUST002", Name = "Global Distributors", GlAccountId = arAcc.Id, PaymentTermsDays = 60, CreditLimit = 100000, IsLocked = false, Address1 = "45 Industrial Ave", City = "Kuala Lumpur", Country = "Malaysia", IsActive = true, CreatedAt = now }
        };
        db.Customers.AddRange(customers);

        var suppliers = new[]
        {
            new Supplier { Code = "SUPP001", Name = "Prime Tech Wholesalers", GlAccountId = apAcc.Id, PaymentTermsDays = 30, DefaultCurrencyId = myr.Id, Address1 = "88 Trade Center", City = "Penang", Country = "Malaysia", IsActive = true, CreatedAt = now },
            new Supplier { Code = "SUPP002", Name = "Euro Supply Corp", GlAccountId = apAcc.Id, PaymentTermsDays = 45, DefaultCurrencyId = usd.Id, Address1 = "5 Avenue Du Port", City = "Brussels", Country = "Belgium", IsActive = true, CreatedAt = now }
        };
        db.Suppliers.AddRange(suppliers);
        await db.SaveChangesAsync(ct);

        var fruitCat = await db.Categories.FirstAsync(c => c.Name == "Fruits", ct);
        var commonCat = await db.Categories.FirstAsync(c => c.Name == "Common", ct);
        var pieceUom = await db.UnitsOfMeasure.FirstAsync(u => u.Name == "Piece", ct);
        var kgUom = await db.UnitsOfMeasure.FirstAsync(u => u.Name == "Kg", ct);
        var primeSupp = suppliers.First(s => s.Code == "SUPP001");

        db.Products.AddRange(
            new Product
            {
                Code = "PROD-APPLE-01", Code2 = "501234567890", Description = "Fresh Red Apples",
                CategoryId = fruitCat.Id, SalesCogsAccountId = cogsAcc.Id, SalesRevenueAccountId = revenueAcc.Id,
                PurchaseCostAccountId = purchaseCostAcc.Id, PurchaseAccountId = apAcc.Id,
                DefaultSupplierId = primeSupp.Id, MainUnitOfMeasureId = kgUom.Id, ConversionFactor = 1,
                UnitCostPrice = 2.50m, UnitSellingPrice = 5.00m, IsActive = true, CreatedAt = now
            },
            new Product
            {
                Code = "PROD-WIDGET-02", Code2 = "501234567891", Description = "Common Steel Widget",
                CategoryId = commonCat.Id, SalesCogsAccountId = cogsAcc.Id, SalesRevenueAccountId = revenueAcc.Id,
                PurchaseCostAccountId = purchaseCostAcc.Id, PurchaseAccountId = apAcc.Id,
                DefaultSupplierId = primeSupp.Id, MainUnitOfMeasureId = pieceUom.Id, ConversionFactor = 1,
                UnitCostPrice = 10.00m, UnitSellingPrice = 22.50m, IsActive = true, CreatedAt = now
            });
        await db.SaveChangesAsync(ct);

        if (!await db.AccAccounts.AnyAsync(ct))
        {
            db.AccAccounts.AddRange(
                new AccAccount { Code = "CASH", AccountName = "Cash", AccountNumber = "001", Description = "Default cash account", Credit = 0, Debit = 0, Balance = 0, IsActive = true, CreatedAt = now },
                new AccAccount { Code = "BANK", AccountName = "Bank", AccountNumber = "002", Description = "Default bank account", Credit = 0, Debit = 0, Balance = 0, IsActive = true, CreatedAt = now });
            await db.SaveChangesAsync(ct);
        }
    }
}
