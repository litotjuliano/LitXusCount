namespace LitXusCount.Domain.Entities;

public class Supplier : AuditableEntity
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    
    public long GlAccountId { get; set; }
    public GlAccount GlAccount { get; set; } = null!;
    
    public string? Address1 { get; set; }
    public string? Address2 { get; set; }
    public string? Address3 { get; set; }
    public string? AddressCode { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    
    public string? Phone { get; set; }
    public string? Fax { get; set; }
    public string? Email { get; set; }
    public string? ContactPerson { get; set; }
    
    public int PaymentTermsDays { get; set; }

    public long? DefaultCurrencyId { get; set; }
    public Currency? DefaultCurrency { get; set; }

    public string? TIN { get; set; }
    public string? RegistrationType { get; set; }
    public string? RegistrationNumber { get; set; }
    public string? SSTRegistrationNumber { get; set; }
}
