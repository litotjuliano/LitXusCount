namespace LitXusCount.Domain.Entities;

/// <summary>Always exactly one row (Id = 1) — no create/delete, edit-only.</summary>
public class CompanyInfo
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;
    public string? LogoUrl { get; set; }

    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? PostCode { get; set; }

    public string? Phone { get; set; }
    public string? Mobile { get; set; }
    public string? Email { get; set; }
    public string? Fax { get; set; }
    public string? Website { get; set; }

    public string? CompanyRegistrationNumber { get; set; }
    public string? VatRegistrationNumber { get; set; }

    public string? InvoiceNumberPrefix { get; set; }
    public string? QuoteNumberPrefix { get; set; }
    public string? TermsAndConditions { get; set; }

    public bool IsVatEnabled { get; set; }
    public string? VatTitle { get; set; }
    public bool IsItemDiscountPercentage { get; set; }

    public long? CurrencyId { get; set; }
    public Currency? Currency { get; set; }

    public long? VatPercentageId { get; set; }
    public VatPercentage? VatPercentage { get; set; }

    public long? EmailConfigId { get; set; }
    public EmailConfig? EmailConfig { get; set; }

    public DateTime? ModifiedAt { get; set; }
    public string? ModifiedBy { get; set; }
}
