namespace LitXusCount.Domain.Entities;

/// <summary>
/// Marker shape for lookup entities that are just a unique Name + optional Description
/// (PaymentType, PaymentStatus, CustomerType, Category, UnitOfMeasure) — lets Infrastructure
/// share one generic CRUD implementation across them without exposing a generic repository
/// or IQueryable to the Application layer.
/// </summary>
public interface ISimpleLookupEntity
{
    long Id { get; set; }
    string Name { get; set; }
    string? Description { get; set; }
    bool IsActive { get; set; }
    DateTime CreatedAt { get; set; }
    DateTime? ModifiedAt { get; set; }
    string? CreatedBy { get; set; }
    string? ModifiedBy { get; set; }
}
