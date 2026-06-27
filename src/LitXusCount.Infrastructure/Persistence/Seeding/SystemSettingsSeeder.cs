using LitXusCount.Application.Settings.EmailConfigs;
using LitXusCount.Domain.Entities;
using LitXusCount.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LitXusCount.Infrastructure.Persistence.Seeding;

public static class SystemSettingsSeeder
{
    /// <summary>
    /// Seeds immutable LHDN reference data and core lookup tables.
    /// Safe to call on every startup — each block is independently idempotent.
    /// Call this for ALL tenants (dev and production).
    /// </summary>
    public static async Task SeedReferenceDataAsync(ApplicationDbContext db, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;

        // ── LHDN Tax Type Codes (01–07) ──────────────────────────────────────
        if (!await db.TaxCodes.AnyAsync(ct))
        {
            db.TaxCodes.AddRange(
                new TaxCode { Code = "01", Name = "Sales Tax",            Description = "Malaysian Sales Tax (SST)",          Rate = 10.0m, IsExempt = false, IsDefault = false, IsActive = true, CreatedAt = now },
                new TaxCode { Code = "02", Name = "Service Tax",          Description = "Malaysian Service Tax (SST)",         Rate = 6.0m,  IsExempt = false, IsDefault = true,  IsActive = true, CreatedAt = now },
                new TaxCode { Code = "03", Name = "Tourism Tax",          Description = "Tourism Tax",                         Rate = 10.0m, IsExempt = false, IsDefault = false, IsActive = true, CreatedAt = now },
                new TaxCode { Code = "04", Name = "High-Value Goods Tax", Description = "High-Value Goods Tax (HVGT)",         Rate = 5.0m,  IsExempt = false, IsDefault = false, IsActive = true, CreatedAt = now },
                new TaxCode { Code = "05", Name = "Tax Exemption (NA)",   Description = "Not applicable / exempt",             Rate = 0.0m,  IsExempt = true,  IsDefault = false, IsActive = true, CreatedAt = now },
                new TaxCode { Code = "06", Name = "Zero-Rated (ATO)",     Description = "Approved Trader / Zero-rated",        Rate = 0.0m,  IsExempt = true,  IsDefault = false, IsActive = true, CreatedAt = now },
                new TaxCode { Code = "07", Name = "Exemption (EP)",       Description = "Exemption by special order (EP)",     Rate = 0.0m,  IsExempt = true,  IsDefault = false, IsActive = true, CreatedAt = now });
            await db.SaveChangesAsync(ct);
        }

        // ── LHDN Classification Codes (001–045, source: sdk.myinvois.hasil.gov.my) ─
        if (!await db.LhdnClassificationCodes.AnyAsync(ct))
        {
            db.LhdnClassificationCodes.AddRange(
                new LhdnClassificationCode { Code = "001", Description = "Breastfeeding equipment",                                                                                                                                                                                                                                                                                                                                                          IsActive = true, CreatedAt = now },
                new LhdnClassificationCode { Code = "002", Description = "Child care centres and kindergartens fees",                                                                                                                                                                                                                                                                                                                                         IsActive = true, CreatedAt = now },
                new LhdnClassificationCode { Code = "003", Description = "Computer, smartphone or tablet",                                                                                                                                                                                                                                                                                                                                                    IsActive = true, CreatedAt = now },
                new LhdnClassificationCode { Code = "004", Description = "Consolidated e-Invoice",                                                                                                                                                                                                                                                                                                                                                           IsActive = true, CreatedAt = now },
                new LhdnClassificationCode { Code = "005", Description = "Construction materials (as specified under Fourth Schedule of the Lembaga Pembangunan Industri Pembinaan Malaysia Act 1994)",                                                                                                                                                                                                                                                       IsActive = true, CreatedAt = now },
                new LhdnClassificationCode { Code = "006", Description = "Disbursement",                                                                                                                                                                                                                                                                                                                                                                     IsActive = true, CreatedAt = now },
                new LhdnClassificationCode { Code = "007", Description = "Donation",                                                                                                                                                                                                                                                                                                                                                                         IsActive = true, CreatedAt = now },
                new LhdnClassificationCode { Code = "008", Description = "e-Commerce - e-Invoice to buyer / purchaser",                                                                                                                                                                                                                                                                                                                                      IsActive = true, CreatedAt = now },
                new LhdnClassificationCode { Code = "009", Description = "e-Commerce - Self-billed e-Invoice to seller, logistics, etc.",                                                                                                                                                                                                                                                                                                                    IsActive = true, CreatedAt = now },
                new LhdnClassificationCode { Code = "010", Description = "Education fees",                                                                                                                                                                                                                                                                                                                                                                   IsActive = true, CreatedAt = now },
                new LhdnClassificationCode { Code = "011", Description = "Goods on consignment (Consignor)",                                                                                                                                                                                                                                                                                                                                                 IsActive = true, CreatedAt = now },
                new LhdnClassificationCode { Code = "012", Description = "Goods on consignment (Consignee)",                                                                                                                                                                                                                                                                                                                                                 IsActive = true, CreatedAt = now },
                new LhdnClassificationCode { Code = "013", Description = "Gym membership",                                                                                                                                                                                                                                                                                                                                                                   IsActive = true, CreatedAt = now },
                new LhdnClassificationCode { Code = "014", Description = "Insurance - Education and medical benefits",                                                                                                                                                                                                                                                                                                                                        IsActive = true, CreatedAt = now },
                new LhdnClassificationCode { Code = "015", Description = "Insurance - Takaful or life insurance",                                                                                                                                                                                                                                                                                                                                            IsActive = true, CreatedAt = now },
                new LhdnClassificationCode { Code = "016", Description = "Interest and financing expenses",                                                                                                                                                                                                                                                                                                                                                  IsActive = true, CreatedAt = now },
                new LhdnClassificationCode { Code = "017", Description = "Internet subscription",                                                                                                                                                                                                                                                                                                                                                            IsActive = true, CreatedAt = now },
                new LhdnClassificationCode { Code = "018", Description = "Land and building",                                                                                                                                                                                                                                                                                                                                                                IsActive = true, CreatedAt = now },
                new LhdnClassificationCode { Code = "019", Description = "Medical examination for learning disabilities and early intervention or rehabilitation treatments of learning disabilities",                                                                                                                                                                                                                                                         IsActive = true, CreatedAt = now },
                new LhdnClassificationCode { Code = "020", Description = "Medical examination or vaccination expenses",                                                                                                                                                                                                                                                                                                                                      IsActive = true, CreatedAt = now },
                new LhdnClassificationCode { Code = "021", Description = "Medical expenses for serious diseases",                                                                                                                                                                                                                                                                                                                                            IsActive = true, CreatedAt = now },
                new LhdnClassificationCode { Code = "022", Description = "Others",                                                                                                                                                                                                                                                                                                                                                                           IsActive = true, CreatedAt = now },
                new LhdnClassificationCode { Code = "023", Description = "Petroleum operations (as defined in Petroleum (Income Tax) Act 1967)",                                                                                                                                                                                                                                                                                                             IsActive = true, CreatedAt = now },
                new LhdnClassificationCode { Code = "024", Description = "Private retirement scheme or deferred annuity scheme",                                                                                                                                                                                                                                                                                                                             IsActive = true, CreatedAt = now },
                new LhdnClassificationCode { Code = "025", Description = "Motor vehicle",                                                                                                                                                                                                                                                                                                                                                                    IsActive = true, CreatedAt = now },
                new LhdnClassificationCode { Code = "026", Description = "Subscription of books / journals / magazines / newspapers / other similar publications",                                                                                                                                                                                                                                                                                           IsActive = true, CreatedAt = now },
                new LhdnClassificationCode { Code = "027", Description = "Reimbursement",                                                                                                                                                                                                                                                                                                                                                                    IsActive = true, CreatedAt = now },
                new LhdnClassificationCode { Code = "028", Description = "Rental of motor vehicle",                                                                                                                                                                                                                                                                                                                                                          IsActive = true, CreatedAt = now },
                new LhdnClassificationCode { Code = "029", Description = "EV charging facilities (Installation, rental, sale / purchase or subscription fees)",                                                                                                                                                                                                                                                                                              IsActive = true, CreatedAt = now },
                new LhdnClassificationCode { Code = "030", Description = "Repair and maintenance",                                                                                                                                                                                                                                                                                                                                                           IsActive = true, CreatedAt = now },
                new LhdnClassificationCode { Code = "031", Description = "Research and development",                                                                                                                                                                                                                                                                                                                                                         IsActive = true, CreatedAt = now },
                new LhdnClassificationCode { Code = "032", Description = "Foreign income",                                                                                                                                                                                                                                                                                                                                                                   IsActive = true, CreatedAt = now },
                new LhdnClassificationCode { Code = "033", Description = "Self-billed - Betting and gaming",                                                                                                                                                                                                                                                                                                                                                 IsActive = true, CreatedAt = now },
                new LhdnClassificationCode { Code = "034", Description = "Self-billed - Importation of goods",                                                                                                                                                                                                                                                                                                                                               IsActive = true, CreatedAt = now },
                new LhdnClassificationCode { Code = "035", Description = "Self-billed - Importation of services",                                                                                                                                                                                                                                                                                                                                            IsActive = true, CreatedAt = now },
                new LhdnClassificationCode { Code = "036", Description = "Self-billed - Others",                                                                                                                                                                                                                                                                                                                                                             IsActive = true, CreatedAt = now },
                new LhdnClassificationCode { Code = "037", Description = "Self-billed - Monetary payment to agents, dealers or distributors",                                                                                                                                                                                                                                                                                                                IsActive = true, CreatedAt = now },
                new LhdnClassificationCode { Code = "038", Description = "Sports equipment, rental / entry fees for sports facilities, registration in sports competition or sports training fees imposed by associations / sports clubs / companies registered with the Sports Commissioner or Companies Commission of Malaysia and carrying out sports activities as listed under the Sports Development Act 1997", IsActive = true, CreatedAt = now },
                new LhdnClassificationCode { Code = "039", Description = "Supporting equipment for disabled person",                                                                                                                                                                                                                                                                                                                                          IsActive = true, CreatedAt = now },
                new LhdnClassificationCode { Code = "040", Description = "Voluntary contribution to approved provident fund",                                                                                                                                                                                                                                                                                                                                 IsActive = true, CreatedAt = now },
                new LhdnClassificationCode { Code = "041", Description = "Dental examination or treatment",                                                                                                                                                                                                                                                                                                                                                  IsActive = true, CreatedAt = now },
                new LhdnClassificationCode { Code = "042", Description = "Fertility treatment",                                                                                                                                                                                                                                                                                                                                                              IsActive = true, CreatedAt = now },
                new LhdnClassificationCode { Code = "043", Description = "Treatment and home care nursing, daycare centres and residential care centers",                                                                                                                                                                                                                                                                                                     IsActive = true, CreatedAt = now },
                new LhdnClassificationCode { Code = "044", Description = "Vouchers, gift cards, loyalty points, etc",                                                                                                                                                                                                                                                                                                                                        IsActive = true, CreatedAt = now },
                new LhdnClassificationCode { Code = "045", Description = "Self-billed - Non-monetary payment to agents, dealers or distributors",                                                                                                                                                                                                                                                                                                            IsActive = true, CreatedAt = now });
            await db.SaveChangesAsync(ct);
        }

        // ── Currencies ───────────────────────────────────────────────────────
        // Step 1: Ensure bootstrap currencies exist with correct Name/Symbol/IsDefault.
        //         Also corrects any existing rows that have stale names (e.g. "Ringgit Malaysia" → "Malaysian Ringgit").
        //         Done as a separate SaveChanges so a bulk-seed failure cannot wipe these out.
        {
            var bootstrap = new (string Code, string Name, string Symbol, bool IsDefault)[]
            {
                ("MYR", "Malaysian Ringgit", "RM",  true),
                ("USD", "US Dollar",         "$",   false),
                ("SGD", "Singapore Dollar",  "S$",  false),
            };

            bool myrAdded = false;
            bool dirty = false;
            foreach (var (code, name, symbol, isDefault) in bootstrap)
            {
                var existing = await db.Currencies.FirstOrDefaultAsync(c => c.Code == code, ct);
                if (existing is null)
                {
                    db.Currencies.Add(new Currency { Code = code, Name = name, Symbol = symbol, IsDefault = isDefault, IsActive = true, CreatedAt = now });
                    if (code == "MYR") myrAdded = true;
                    dirty = true;
                }
                else
                {
                    // Correct stale values from older seeder versions
                    if (existing.Name != name)         { existing.Name     = name;     dirty = true; }
                    if (existing.Symbol != symbol)     { existing.Symbol   = symbol;   dirty = true; }
                    if (existing.IsDefault != isDefault) { existing.IsDefault = isDefault; dirty = true; }
                }
            }
            if (dirty) await db.SaveChangesAsync(ct);

            // Wire MYR as the CompanyInfo default whenever it was just added to this tenant,
            // or if CompanyInfo has no currency set yet.
            var myr = await db.Currencies.FirstOrDefaultAsync(c => c.Code == "MYR", ct);
            var info = await db.CompanyInfos.SingleOrDefaultAsync(x => x.Id == 1, ct);
            if (myr is not null && info is not null && (myrAdded || info.CurrencyId is null))
            {
                info.CurrencyId = myr.Id;
                await db.SaveChangesAsync(ct);
            }
        }

        // Step 2: Bulk-seed remaining ISO currencies from the LHDN CSV (excludes bootstrap codes).
        {
            var assembly = typeof(SystemSettingsSeeder).Assembly;
            const string resource = "LitXusCount.Infrastructure.Persistence.Seeding.Data.lhdn-currency-codes.csv";
            using var stream = assembly.GetManifestResourceStream(resource)
                ?? throw new InvalidOperationException($"Embedded resource '{resource}' not found.");
            using var reader = new System.IO.StreamReader(stream);

            var bootstrapCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "MYR", "USD", "SGD" };
            var existingCodes = (await db.Currencies.Select(x => x.Code).ToListAsync(ct)).ToHashSet();

            var toAdd = new List<Currency>();
            while (await reader.ReadLineAsync(ct) is { } line)
            {
                var sep = line.IndexOf('|');
                if (sep < 1) continue;
                var code = line[..sep].Trim();
                if (bootstrapCodes.Contains(code) || existingCodes.Contains(code)) continue;
                toAdd.Add(new Currency { Code = code, Name = line[(sep + 1)..].Trim(), IsDefault = false, IsActive = true, CreatedAt = now });
            }

            if (toAdd.Count > 0)
            {
                db.Currencies.AddRange(toAdd);
                await db.SaveChangesAsync(ct);
            }
        }

        // ── Tax Rates (Malaysian SST) ────────────────────────────────────────
        // Malaysia uses SST (Sales and Service Tax), not VAT.
        // Sales Tax: 10% standard / 5% reduced (food, primary commodities)
        // Service Tax: 8% standard (from 2024) / 6% special (non-commercial services)
        if (!await db.VatPercentages.AnyAsync(ct))
        {
            var sstRows = new (int Rate, string Name, bool IsDefault)[]
            {
                (0,  "Exempt (0%)",             false),
                (5,  "Sales Tax — 5%",          false),
                (6,  "Service Tax — 6% (Special)", false),
                (8,  "Service Tax — 8%",         true),
                (10, "Sales Tax — 10%",          false),
            }
            .Select(r => new VatPercentage { Name = r.Name, Percentage = r.Rate, IsDefault = r.IsDefault, IsActive = true, CreatedAt = now })
            .ToList();
            db.VatPercentages.AddRange(sstRows);
            await db.SaveChangesAsync(ct);

            var defaultSst = sstRows.Single(v => v.IsDefault);
            var info = await db.CompanyInfos.SingleAsync(x => x.Id == 1, ct);
            if (info.VatPercentageId is null)
            {
                info.VatPercentageId = defaultSst.Id;
                await db.SaveChangesAsync(ct);
            }
        }

        // ── LHDN Payment Mode Codes (01–08, source: sdk.myinvois.hasil.gov.my/codes/payment-methods) ─
        var officialPaymentCodes = new[]
        {
            ("01", "Cash"),
            ("02", "Cheque"),
            ("03", "Bank Transfer"),
            ("04", "Credit Card"),
            ("05", "Debit Card"),
            ("06", "e-Wallet / Digital Wallet"),
            ("07", "Digital Bank"),
            ("08", "Others"),
        };
        var existingPaymentCodes = await db.PaymentCodes.ToDictionaryAsync(x => x.Code ?? "", ct);
        foreach (var (code, name) in officialPaymentCodes)
        {
            if (existingPaymentCodes.TryGetValue(code, out var existing))
            {
                existing.Name = name;
                existing.Code = code;
                existing.ModifiedAt = now;
            }
            else
            {
                db.PaymentCodes.Add(new PaymentCode { Name = name, Code = code, IsActive = true, CreatedAt = now });
            }
        }
        await db.SaveChangesAsync(ct);

        // ── Payment Statuses ─────────────────────────────────────────────────
        if (!await db.PaymentStatuses.AnyAsync(ct))
        {
            db.PaymentStatuses.AddRange(
                new[] { "Paid", "UnPaid", "Partially Paid", "Deposit", "Pay within 7 Days", "Pay within 14 Days", "Pay within 30 Days", "Custom Date" }
                    .Select(name => new PaymentStatus { Name = name, Description = name, IsActive = true, CreatedAt = now }));
            await db.SaveChangesAsync(ct);
        }

        // ── Customer Types ───────────────────────────────────────────────────
        if (!await db.CustomerTypes.AnyAsync(ct))
        {
            db.CustomerTypes.AddRange(
                new[] { "Normal", "Premium", "Trader", "Other" }
                    .Select(name => new CustomerType { Name = name, Description = name, IsActive = true, CreatedAt = now }));
            await db.SaveChangesAsync(ct);
        }

        // ── Categories ───────────────────────────────────────────────────────
        if (!await db.Categories.AnyAsync(ct))
        {
            db.Categories.AddRange(new[]
            {
                ("Fruits",               "Fruits Item"),
                ("Dairy Products",       "Dairy Products"),
                ("Beverages",            "Soft drinks, coffees, teas, etc"),
                ("Freezer",              "Freezer"),
                ("Meat & Fish",          "Meat & Fish"),
                ("Vegetables",           "Vegetables"),
                ("Beauty and Cosmetic",  "Beauty and Cosmetic"),
                ("IT",                   "IT"),
                ("Electronics",          "Electronics"),
                ("Steels",               "Coated Steel Sheet"),
                ("Common",               "For common all items"),
            }.Select(c => new Category { Name = c.Item1, Description = c.Item2, IsActive = true, CreatedAt = now }));
            await db.SaveChangesAsync(ct);
        }

        // ── Units of Measure (all official LHDN / UN/CEFACT codes) ──────────
        // Source: sdk.myinvois.hasil.gov.my/codes/unit-types/ — 2161 codes
        // Strategy: upsert by UnCefactCode so existing FK references are preserved.
        {
            var assembly = typeof(SystemSettingsSeeder).Assembly;
            const string resource = "LitXusCount.Infrastructure.Persistence.Seeding.Data.lhdn-unit-types.csv";
            using var stream = assembly.GetManifestResourceStream(resource)
                ?? throw new InvalidOperationException($"Embedded resource '{resource}' not found.");
            using var reader = new System.IO.StreamReader(stream);

            var official = new List<(string Code, string Desc)>();
            while (await reader.ReadLineAsync(ct) is { } line)
            {
                var sep = line.IndexOf('|');
                if (sep < 1) continue;
                official.Add((line[..sep].Trim(), line[(sep + 1)..].Trim()));
            }

            // Name format: "CODE - Description" guarantees uniqueness across all 2161 codes.
            static string MakeName(string code, string desc) =>
                $"{code} - {(desc.Length > 0 ? char.ToUpper(desc[0]) + desc[1..] : desc)}";

            // Update names of existing records to official format.
            var existing = await db.UnitsOfMeasure
                .Where(x => x.UnCefactCode != null)
                .ToListAsync(ct);
            var codeToDesc = official.ToDictionary(u => u.Code, u => u.Desc);
            bool nameUpdated = false;
            foreach (var uom in existing)
            {
                if (uom.UnCefactCode != null && codeToDesc.TryGetValue(uom.UnCefactCode, out var desc))
                {
                    var officialName = MakeName(uom.UnCefactCode, desc);
                    if (uom.Name != officialName) { uom.Name = officialName; nameUpdated = true; }
                }
            }

            // Insert codes not yet in DB.
            var existingCodes = existing.Select(x => x.UnCefactCode).ToHashSet();
            var toAdd = official
                .Where(u => !existingCodes.Contains(u.Code))
                .Select(u => new UnitOfMeasure
                {
                    Name = MakeName(u.Code, u.Desc),
                    UnCefactCode = u.Code,
                    Description = u.Desc,
                    IsActive = true,
                    CreatedAt = now,
                })
                .ToList();

            if (toAdd.Count > 0) db.UnitsOfMeasure.AddRange(toAdd);
            if (nameUpdated || toAdd.Count > 0) await db.SaveChangesAsync(ct);
        }

        // ── LHDN Country Codes (source: sdk.myinvois.hasil.gov.my/codes/countries/) ─
        if (!await db.LhdnCountries.AnyAsync(ct))
        {
            var assembly = typeof(SystemSettingsSeeder).Assembly;
            const string resource = "LitXusCount.Infrastructure.Persistence.Seeding.Data.lhdn-country-codes.csv";
            using var stream = assembly.GetManifestResourceStream(resource)
                ?? throw new InvalidOperationException($"Embedded resource '{resource}' not found.");
            using var reader = new System.IO.StreamReader(stream);

            var countries = new List<LhdnCountry>();
            while (await reader.ReadLineAsync(ct) is { } line)
            {
                var sep = line.IndexOf('|');
                if (sep < 1) continue;
                countries.Add(new LhdnCountry
                {
                    Code = line[..sep].Trim(),
                    Name = line[(sep + 1)..].Trim(),
                    IsActive = true,
                    CreatedAt = now,
                });
            }
            db.LhdnCountries.AddRange(countries);
            await db.SaveChangesAsync(ct);
        }

        // ── LHDN Currency Codes (source: sdk.myinvois.hasil.gov.my/codes/currencies/) ─
        if (!await db.LhdnCurrencyCodes.AnyAsync(ct))
        {
            var assembly = typeof(SystemSettingsSeeder).Assembly;
            const string resource = "LitXusCount.Infrastructure.Persistence.Seeding.Data.lhdn-currency-codes.csv";
            using var stream = assembly.GetManifestResourceStream(resource)
                ?? throw new InvalidOperationException($"Embedded resource '{resource}' not found.");
            using var reader = new System.IO.StreamReader(stream);

            var currencyCodes = new List<LhdnCurrencyCode>();
            while (await reader.ReadLineAsync(ct) is { } line)
            {
                var sep = line.IndexOf('|');
                if (sep < 1) continue;
                currencyCodes.Add(new LhdnCurrencyCode
                {
                    Code      = line[..sep].Trim(),
                    Name      = line[(sep + 1)..].Trim(),
                    IsActive  = true,
                    CreatedAt = now,
                });
            }
            db.LhdnCurrencyCodes.AddRange(currencyCodes);
            await db.SaveChangesAsync(ct);
        }

        // ── LHDN MSIC Codes (source: sdk.myinvois.hasil.gov.my/codes/msic-codes/) ─
        if (!await db.LhdnMsicCodes.AnyAsync(ct))
        {
            var assembly = typeof(SystemSettingsSeeder).Assembly;
            const string resource = "LitXusCount.Infrastructure.Persistence.Seeding.Data.lhdn-msic-codes.csv";
            using var stream = assembly.GetManifestResourceStream(resource)
                ?? throw new InvalidOperationException($"Embedded resource '{resource}' not found.");
            using var reader = new System.IO.StreamReader(stream);

            var msicCodes = new List<LhdnMsicCode>();
            while (await reader.ReadLineAsync(ct) is { } line)
            {
                var parts = line.Split('|');
                if (parts.Length < 3) continue;
                msicCodes.Add(new LhdnMsicCode
                {
                    Code        = parts[0].Trim(),
                    Category    = parts[1].Trim(),
                    Description = parts[2].Trim(),
                    IsActive    = true,
                    CreatedAt   = now,
                });
            }
            db.LhdnMsicCodes.AddRange(msicCodes);
            await db.SaveChangesAsync(ct);
        }

        // ── LHDN Tax Types (source: sdk.myinvois.hasil.gov.my/codes/tax-types/) ──
        if (!await db.LhdnTaxTypes.AnyAsync(ct))
        {
            db.LhdnTaxTypes.AddRange(
                new LhdnTaxType { Code = "01", Description = "Sales Tax",                        IsActive = true, CreatedAt = now },
                new LhdnTaxType { Code = "02", Description = "Service Tax",                      IsActive = true, CreatedAt = now },
                new LhdnTaxType { Code = "03", Description = "Tourism Tax",                      IsActive = true, CreatedAt = now },
                new LhdnTaxType { Code = "04", Description = "High-Value Goods Tax",             IsActive = true, CreatedAt = now },
                new LhdnTaxType { Code = "05", Description = "Sales Tax on Low Value Goods",     IsActive = true, CreatedAt = now },
                new LhdnTaxType { Code = "06", Description = "Not Applicable",                   IsActive = true, CreatedAt = now },
                new LhdnTaxType { Code = "E",  Description = "Tax exemption (where applicable)", IsActive = true, CreatedAt = now }
            );
            await db.SaveChangesAsync(ct);
        }

        // ── LHDN State Codes (source: sdk.myinvois.hasil.gov.my/codes/state-codes/) ─
        if (!await db.LhdnStateCodes.AnyAsync(ct))
        {
            var stateCodes = new (string Code, string Name)[]
            {
                ("01", "Johor"),
                ("02", "Kedah"),
                ("03", "Kelantan"),
                ("04", "Melaka"),
                ("05", "Negeri Sembilan"),
                ("06", "Pahang"),
                ("07", "Pulau Pinang"),
                ("08", "Perak"),
                ("09", "Perlis"),
                ("10", "Selangor"),
                ("11", "Terengganu"),
                ("12", "Sabah"),
                ("13", "Sarawak"),
                ("14", "Wilayah Persekutuan Kuala Lumpur"),
                ("15", "Wilayah Persekutuan Labuan"),
                ("16", "Wilayah Persekutuan Putrajaya"),
            };
            db.LhdnStateCodes.AddRange(stateCodes.Select(s => new LhdnStateCode
            {
                Code = s.Code,
                Name = s.Name,
                IsActive = true,
                CreatedAt = now,
            }));
            await db.SaveChangesAsync(ct);
        }

        // ── GL Accounts ──────────────────────────────────────────────────────
        if (!await db.GlAccounts.AnyAsync(ct))
        {
            db.GlAccounts.AddRange(
                new GlAccount { Code = "11000", Name = "Accounts Receivable",   AccountType = GlAccountType.Asset,     IsActive = true, CreatedAt = now },
                new GlAccount { Code = "21000", Name = "Accounts Payable",      AccountType = GlAccountType.Liability, IsActive = true, CreatedAt = now },
                new GlAccount { Code = "22000", Name = "Tax Payable (Output)",  AccountType = GlAccountType.Liability, IsActive = true, CreatedAt = now },
                new GlAccount { Code = "41000", Name = "Sales Revenue",         AccountType = GlAccountType.Revenue,   IsActive = true, CreatedAt = now },
                new GlAccount { Code = "51000", Name = "Cost of Goods Sold",    AccountType = GlAccountType.Cogs,      IsActive = true, CreatedAt = now },
                new GlAccount { Code = "52000", Name = "Purchase Cost Account", AccountType = GlAccountType.Expense,   IsActive = true, CreatedAt = now });
            await db.SaveChangesAsync(ct);
        }
    }

    /// <summary>
    /// Seeds demo/dev data: sample company profile, email configs, customers, suppliers, products.
    /// Only call this in Development or when SEED_DEMO_DATA=true.
    /// Requires SeedReferenceDataAsync to have run first.
    /// </summary>
    public static async Task SeedDemoDataAsync(ApplicationDbContext db, IEmailConfigEncryptor encryptor, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;

        // ── Email Configs ────────────────────────────────────────────────────
        if (!await db.EmailConfigs.AnyAsync(ct))
        {
            var primaryEmail = new EmailConfig
            {
                Email = "dev01@gmail.com", PasswordEncrypted = encryptor.Encrypt("ChangeMe123!"),
                Hostname = "smtp.gmail.com", Port = 587, SslEnabled = true,
                SenderFullName = "LitXusCount", IsDefault = true, IsActive = true, CreatedAt = now,
            };
            db.EmailConfigs.AddRange(
                primaryEmail,
                new EmailConfig { Email = "dev02@gmail.com", PasswordEncrypted = encryptor.Encrypt("ChangeMe123!"), Hostname = "smtp.gmail.com", Port = 587, SslEnabled = false, SenderFullName = "LitXusCount", IsDefault = false, IsActive = true, CreatedAt = now },
                new EmailConfig { Email = "dev03@gmail.com", PasswordEncrypted = encryptor.Encrypt("ChangeMe123!"), Hostname = "smtp.gmail.com", Port = 587, SslEnabled = false, SenderFullName = "LitXusCount", IsDefault = false, IsActive = true, CreatedAt = now },
                new EmailConfig { Email = "dev04@gmail.com", PasswordEncrypted = encryptor.Encrypt("ChangeMe123!"), Hostname = "smtp.gmail.com", Port = 587, SslEnabled = false, SenderFullName = "LitXusCount", IsDefault = false, IsActive = true, CreatedAt = now });
            await db.SaveChangesAsync(ct);

            var info = await db.CompanyInfos.SingleAsync(x => x.Id == 1, ct);
            if (info.EmailConfigId is null)
            {
                info.EmailConfigId = primaryEmail.Id;
                await db.SaveChangesAsync(ct);
            }
        }

        // ── Company Info (demo profile) ──────────────────────────────────────
        var companyInfo = await db.CompanyInfos.SingleAsync(x => x.Id == 1, ct);
        if (companyInfo.Name == "My Company")
        {
            var myr = await db.Currencies.FirstOrDefaultAsync(c => c.Code == "MYR", ct);
            var defaultVat = await db.VatPercentages.FirstOrDefaultAsync(v => v.IsDefault, ct);
            var primaryEmailCfg = await db.EmailConfigs.FirstOrDefaultAsync(e => e.IsDefault, ct);

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
            companyInfo.VatRegistrationNumber = "W10-1234-12345678"; // SST Registration Number
            companyInfo.InvoiceNumberPrefix = "INV";
            companyInfo.QuoteNumberPrefix = "QTO";
            companyInfo.TermsAndConditions = "Terms and Conditions apply to all sales made through this system. " +
                "By placing an order, the customer agrees to these terms. Prices are subject to change without notice.";
            companyInfo.IsVatEnabled = true;
            companyInfo.VatTitle = "SST(%)";
            companyInfo.IsItemDiscountPercentage = true;
            if (myr != null) companyInfo.CurrencyId = myr.Id;
            if (defaultVat != null) companyInfo.VatPercentageId = defaultVat.Id;
            if (primaryEmailCfg != null) companyInfo.EmailConfigId = primaryEmailCfg.Id;
            companyInfo.ModifiedAt = now;
            await db.SaveChangesAsync(ct);
        }

        // ── Sample Customers ─────────────────────────────────────────────────
        if (!await db.Customers.AnyAsync(ct))
        {
            var arAcc = await db.GlAccounts.FirstOrDefaultAsync(a => a.Code == "11000", ct);
            if (arAcc != null)
            {
                db.Customers.AddRange(
                    new Customer { Code = "CUST001", Name = "Apex Retailers Ltd",   GlAccountId = arAcc.Id, PaymentTermsDays = 30, CreditLimit = 50000,  IsLocked = false, Address1 = "12 Main St",        City = "Ipoh",          Country = "Malaysia", IsActive = true, CreatedAt = now },
                    new Customer { Code = "CUST002", Name = "Global Distributors",  GlAccountId = arAcc.Id, PaymentTermsDays = 60, CreditLimit = 100000, IsLocked = false, Address1 = "45 Industrial Ave", City = "Kuala Lumpur",  Country = "Malaysia", IsActive = true, CreatedAt = now });
                await db.SaveChangesAsync(ct);
            }
        }

        // ── Sample Suppliers ─────────────────────────────────────────────────
        if (!await db.Suppliers.AnyAsync(ct))
        {
            var apAcc = await db.GlAccounts.FirstOrDefaultAsync(a => a.Code == "21000", ct);
            var myr    = await db.Currencies.FirstOrDefaultAsync(c => c.Code == "MYR", ct);
            var usd    = await db.Currencies.FirstOrDefaultAsync(c => c.Code == "USD", ct);
            if (apAcc != null && myr != null && usd != null)
            {
                db.Suppliers.AddRange(
                    new Supplier { Code = "SUPP001", Name = "Prime Tech Wholesalers", GlAccountId = apAcc.Id, PaymentTermsDays = 30, DefaultCurrencyId = myr.Id, Address1 = "88 Trade Center", City = "Penang",   Country = "Malaysia", IsActive = true, CreatedAt = now },
                    new Supplier { Code = "SUPP002", Name = "Euro Supply Corp",       GlAccountId = apAcc.Id, PaymentTermsDays = 45, DefaultCurrencyId = usd.Id, Address1 = "5 Avenue Du Port", City = "Brussels", Country = "Belgium",  IsActive = true, CreatedAt = now });
                await db.SaveChangesAsync(ct);
            }
        }

        // ── Sample Products ──────────────────────────────────────────────────
        if (!await db.Products.AnyAsync(ct))
        {
            var cogsAcc     = await db.GlAccounts.FirstOrDefaultAsync(a => a.Code == "51000", ct);
            var revenueAcc  = await db.GlAccounts.FirstOrDefaultAsync(a => a.Code == "41000", ct);
            var purchCostAcc = await db.GlAccounts.FirstOrDefaultAsync(a => a.Code == "52000", ct);
            var apAcc       = await db.GlAccounts.FirstOrDefaultAsync(a => a.Code == "21000", ct);
            var fruitCat    = await db.Categories.FirstOrDefaultAsync(c => c.Name == "Fruits", ct);
            var commonCat   = await db.Categories.FirstOrDefaultAsync(c => c.Name == "Common", ct);
            var kgUom       = await db.UnitsOfMeasure.FirstOrDefaultAsync(u => u.UnCefactCode == "KGM", ct);
            var pieceUom    = await db.UnitsOfMeasure.FirstOrDefaultAsync(u => u.UnCefactCode == "C62", ct);
            var primeSupp   = await db.Suppliers.FirstOrDefaultAsync(s => s.Code == "SUPP001", ct);

            if (cogsAcc != null && revenueAcc != null && purchCostAcc != null && apAcc != null
                && fruitCat != null && commonCat != null && kgUom != null && pieceUom != null && primeSupp != null)
            {
                db.Products.AddRange(
                    new Product
                    {
                        Code = "PROD-APPLE-01",  Code2 = "501234567890", Description = "Fresh Red Apples",
                        CategoryId = fruitCat.Id, SalesCogsAccountId = cogsAcc.Id, SalesRevenueAccountId = revenueAcc.Id,
                        PurchaseCostAccountId = purchCostAcc.Id, PurchaseAccountId = apAcc.Id,
                        DefaultSupplierId = primeSupp.Id, MainUnitOfMeasureId = kgUom.Id, ConversionFactor = 1,
                        UnitCostPrice = 2.50m, UnitSellingPrice = 5.00m, IsActive = true, CreatedAt = now
                    },
                    new Product
                    {
                        Code = "PROD-WIDGET-02", Code2 = "501234567891", Description = "Common Steel Widget",
                        CategoryId = commonCat.Id, SalesCogsAccountId = cogsAcc.Id, SalesRevenueAccountId = revenueAcc.Id,
                        PurchaseCostAccountId = purchCostAcc.Id, PurchaseAccountId = apAcc.Id,
                        DefaultSupplierId = primeSupp.Id, MainUnitOfMeasureId = pieceUom.Id, ConversionFactor = 1,
                        UnitCostPrice = 10.00m, UnitSellingPrice = 22.50m, IsActive = true, CreatedAt = now
                    });
                await db.SaveChangesAsync(ct);
            }
        }

        // ── Sample Acc Accounts ──────────────────────────────────────────────
        if (!await db.AccAccounts.AnyAsync(ct))
        {
            db.AccAccounts.AddRange(
                new AccAccount { Code = "CASH", AccountName = "Cash", AccountNumber = "001", Description = "Default cash account", Credit = 0, Debit = 0, Balance = 0, IsActive = true, CreatedAt = now },
                new AccAccount { Code = "BANK", AccountName = "Bank", AccountNumber = "002", Description = "Default bank account", Credit = 0, Debit = 0, Balance = 0, IsActive = true, CreatedAt = now });
            await db.SaveChangesAsync(ct);
        }
    }

    /// <summary>Convenience wrapper — seeds reference data then demo data.</summary>
    public static async Task SeedAsync(ApplicationDbContext db, IEmailConfigEncryptor encryptor, CancellationToken ct = default)
    {
        await SeedReferenceDataAsync(db, ct);
        await SeedDemoDataAsync(db, encryptor, ct);
    }
}
