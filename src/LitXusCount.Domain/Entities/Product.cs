namespace LitXusCount.Domain.Entities;

public class Product : AuditableEntity
{
    public string Code { get; set; } = null!;
    public string? Code2 { get; set; }
    public string? ParentProductCode { get; set; }
    
    public long? CategoryId { get; set; }
    public Category? Category { get; set; }
    
    public string? Description { get; set; }
    
    // GL Accounts for Posting Engine
    public long? SalesCogsAccountId { get; set; }
    public GlAccount? SalesCogsAccount { get; set; }
    
    public long? SalesRevenueAccountId { get; set; }
    public GlAccount? SalesRevenueAccount { get; set; }
    
    public long? PurchaseCostAccountId { get; set; }
    public GlAccount? PurchaseCostAccount { get; set; }
    
    public long? PurchaseAccountId { get; set; }
    public GlAccount? PurchaseAccount { get; set; }
    
    public string? SalesTaxCode { get; set; }
    public string? PurchaseTaxCode { get; set; }
    
    public long? DefaultSupplierId { get; set; }
    public Supplier? DefaultSupplier { get; set; }
    
    public long? MainUnitOfMeasureId { get; set; }
    public UnitOfMeasure? MainUnitOfMeasure { get; set; }
    
    public long? AltUnitOfMeasureId { get; set; }
    public UnitOfMeasure? AltUnitOfMeasure { get; set; }
    
    public decimal ConversionFactor { get; set; }
    public int ShelfLifeDays { get; set; }
    
    // Pricing
    public decimal UnitCostPrice { get; set; }
    public decimal UnitSellingPrice { get; set; }
    public decimal UnitSellingPrice2 { get; set; }
    public decimal MinSalesQty2 { get; set; }
    public decimal UnitSellingPrice3 { get; set; }
    public decimal MinSalesQty3 { get; set; }
    
    // Promotion
    public string? PromoCode { get; set; }
    public DateTime? PromoFromDate { get; set; }
    public DateTime? PromoToDate { get; set; }
    
    // Stock Control
    public decimal MinQty { get; set; }
    public decimal MaxQty { get; set; }
    public decimal ReorderQty { get; set; }
    public int LeadTimeDays { get; set; }
    
    public decimal StockQuantity { get; set; }

    public string? PackagingQty { get; set; }
    public string? Remark { get; set; }
    public string? ImageRef { get; set; }
}
