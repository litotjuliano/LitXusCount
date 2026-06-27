namespace LitXusCount.Domain.Entities;

public class Customer : AuditableEntity
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Name2 { get; set; }
    
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
    public string? Phone2 { get; set; }
    public string? Fax { get; set; }
    public string? Email { get; set; }
    public string? ContactPerson { get; set; }
    
    public string? ConsigneeName { get; set; }
    public string? ConsigneeAddress1 { get; set; }
    public string? ConsigneeAddress2 { get; set; }
    public string? ConsigneeAddress3 { get; set; }
    public string? ConsigneeAddressCode { get; set; }
    public string? ConsigneeCity { get; set; }
    public string? ConsigneeState { get; set; }
    public string? ConsigneeCountry { get; set; }
    public string? ConsigneePhone { get; set; }
    
    public int PaymentTermsDays { get; set; }
    public decimal CreditLimit { get; set; }
    public bool IsLocked { get; set; }

    // LHDN buyer identity (required for B2B e-invoicing; B2C >= RM10,000)
    public string? TIN { get; set; }
    public string? RegistrationType { get; set; }
    public string? RegistrationNumber { get; set; }
    public string? SSTRegistrationNumber { get; set; }
}
