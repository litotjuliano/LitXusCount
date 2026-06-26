using LitXusCount.Application.Common;
using LitXusCount.Application.Settings.Products;
using LitXusCount.Domain.Entities;
using LitXusCount.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LitXusCount.Infrastructure.Settings.Products;

internal sealed class ProductService(ApplicationDbContext db) : IProductService
{
    private static readonly string[] SortableColumns = ["name", "code", "ucp", "usp"];

    public async Task<PagedResult<ProductDto>> ListAsync(PagedQuery query, CancellationToken ct = default)
    {
        var filtered = db.Products
            .Include(x => x.Category)
            .Include(x => x.SalesCogsAccount)
            .Include(x => x.SalesRevenueAccount)
            .Include(x => x.PurchaseCostAccount)
            .Include(x => x.PurchaseAccount)
            .Include(x => x.DefaultSupplier)
            .Include(x => x.MainUnitOfMeasure)
            .Include(x => x.AltUnitOfMeasure)
            .Where(x => x.IsActive);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var pattern = $"%{query.Search.Trim()}%";
            filtered = filtered.Where(x =>
                EF.Functions.Like(x.Code, pattern) ||
                EF.Functions.Like(x.Description ?? "", pattern) ||
                (x.Code2 != null && EF.Functions.Like(x.Code2, pattern)));
        }

        var totalCount = await filtered.CountAsync(ct);

        var sortBy = SortableColumns.Contains(query.SortBy?.ToLowerInvariant()) ? query.SortBy!.ToLowerInvariant() : "code";
        filtered = (sortBy, query.SortDescending) switch
        {
            ("name", true) => filtered.OrderByDescending(x => x.Description),
            ("name", false) => filtered.OrderBy(x => x.Description),
            ("ucp", true) => filtered.OrderByDescending(x => x.UnitCostPrice),
            ("ucp", false) => filtered.OrderBy(x => x.UnitCostPrice),
            ("usp", true) => filtered.OrderByDescending(x => x.UnitSellingPrice),
            ("usp", false) => filtered.OrderBy(x => x.UnitSellingPrice),
            (_, true) => filtered.OrderByDescending(x => x.Code),
            (_, false) => filtered.OrderBy(x => x.Code),
        };

        var items = await filtered
            .Skip((query.EffectivePage - 1) * query.EffectivePageSize)
            .Take(query.EffectivePageSize)
            .Select(x => new ProductDto(
                x.Id,
                x.Code,
                x.Code2,
                x.ParentProductCode,
                x.CategoryId,
                x.Category != null ? x.Category.Name : null,
                x.Description,
                x.SalesCogsAccountId,
                x.SalesCogsAccount != null ? x.SalesCogsAccount.Name : null,
                x.SalesRevenueAccountId,
                x.SalesRevenueAccount != null ? x.SalesRevenueAccount.Name : null,
                x.PurchaseCostAccountId,
                x.PurchaseCostAccount != null ? x.PurchaseCostAccount.Name : null,
                x.PurchaseAccountId,
                x.PurchaseAccount != null ? x.PurchaseAccount.Name : null,
                x.SalesTaxCode,
                x.PurchaseTaxCode,
                x.DefaultSupplierId,
                x.DefaultSupplier != null ? x.DefaultSupplier.Name : null,
                x.MainUnitOfMeasureId,
                x.MainUnitOfMeasure != null ? x.MainUnitOfMeasure.Name : null,
                x.AltUnitOfMeasureId,
                x.AltUnitOfMeasure != null ? x.AltUnitOfMeasure.Name : null,
                x.ConversionFactor,
                x.ShelfLifeDays,
                x.UnitCostPrice,
                x.UnitSellingPrice,
                x.UnitSellingPrice2,
                x.MinSalesQty2,
                x.UnitSellingPrice3,
                x.MinSalesQty3,
                x.PromoCode,
                x.PromoFromDate,
                x.PromoToDate,
                x.MinQty,
                x.MaxQty,
                x.ReorderQty,
                x.LeadTimeDays,
                x.PackagingQty,
                x.Remark,
                x.ImageRef,
                x.IsActive))
            .ToListAsync(ct);

        return new PagedResult<ProductDto>(items, totalCount, query.EffectivePage, query.EffectivePageSize);
    }

    public async Task<IReadOnlyList<ProductDto>> ListAllActiveAsync(CancellationToken ct = default) =>
        await db.Products
            .Include(x => x.Category)
            .Include(x => x.SalesCogsAccount)
            .Include(x => x.SalesRevenueAccount)
            .Include(x => x.PurchaseCostAccount)
            .Include(x => x.PurchaseAccount)
            .Include(x => x.DefaultSupplier)
            .Include(x => x.MainUnitOfMeasure)
            .Include(x => x.AltUnitOfMeasure)
            .Where(x => x.IsActive)
            .OrderBy(x => x.Code)
            .Take(200)
            .Select(x => new ProductDto(
                x.Id,
                x.Code,
                x.Code2,
                x.ParentProductCode,
                x.CategoryId,
                x.Category != null ? x.Category.Name : null,
                x.Description,
                x.SalesCogsAccountId,
                x.SalesCogsAccount != null ? x.SalesCogsAccount.Name : null,
                x.SalesRevenueAccountId,
                x.SalesRevenueAccount != null ? x.SalesRevenueAccount.Name : null,
                x.PurchaseCostAccountId,
                x.PurchaseCostAccount != null ? x.PurchaseCostAccount.Name : null,
                x.PurchaseAccountId,
                x.PurchaseAccount != null ? x.PurchaseAccount.Name : null,
                x.SalesTaxCode,
                x.PurchaseTaxCode,
                x.DefaultSupplierId,
                x.DefaultSupplier != null ? x.DefaultSupplier.Name : null,
                x.MainUnitOfMeasureId,
                x.MainUnitOfMeasure != null ? x.MainUnitOfMeasure.Name : null,
                x.AltUnitOfMeasureId,
                x.AltUnitOfMeasure != null ? x.AltUnitOfMeasure.Name : null,
                x.ConversionFactor,
                x.ShelfLifeDays,
                x.UnitCostPrice,
                x.UnitSellingPrice,
                x.UnitSellingPrice2,
                x.MinSalesQty2,
                x.UnitSellingPrice3,
                x.MinSalesQty3,
                x.PromoCode,
                x.PromoFromDate,
                x.PromoToDate,
                x.MinQty,
                x.MaxQty,
                x.ReorderQty,
                x.LeadTimeDays,
                x.PackagingQty,
                x.Remark,
                x.ImageRef,
                x.IsActive))
            .ToListAsync(ct);

    public async Task<ProductDto?> GetAsync(long id, CancellationToken ct = default)
    {
        var entity = await db.Products
            .Include(x => x.Category)
            .Include(x => x.SalesCogsAccount)
            .Include(x => x.SalesRevenueAccount)
            .Include(x => x.PurchaseCostAccount)
            .Include(x => x.PurchaseAccount)
            .Include(x => x.DefaultSupplier)
            .Include(x => x.MainUnitOfMeasure)
            .Include(x => x.AltUnitOfMeasure)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
        return entity is null ? null : ToDto(entity);
    }

    public async Task<ServiceResult<ProductDto>> CreateAsync(ProductUpsertDto request, CancellationToken ct = default)
    {
        if (await CodeInUseAsync(request.Code, excludeId: null, ct))
        {
            return ServiceResult<ProductDto>.Failure($"Product code '{request.Code}' is already in use.");
        }

        var entity = new Product
        {
            Code = request.Code,
            Code2 = request.Code2,
            ParentProductCode = request.ParentProductCode,
            CategoryId = request.CategoryId,
            Description = request.Description,
            SalesCogsAccountId = request.SalesCogsAccountId,
            SalesRevenueAccountId = request.SalesRevenueAccountId,
            PurchaseCostAccountId = request.PurchaseCostAccountId,
            PurchaseAccountId = request.PurchaseAccountId,
            SalesTaxCode = request.SalesTaxCode,
            PurchaseTaxCode = request.PurchaseTaxCode,
            DefaultSupplierId = request.DefaultSupplierId,
            MainUnitOfMeasureId = request.MainUnitOfMeasureId,
            AltUnitOfMeasureId = request.AltUnitOfMeasureId,
            ConversionFactor = request.ConversionFactor,
            ShelfLifeDays = request.ShelfLifeDays,
            UnitCostPrice = request.UnitCostPrice,
            UnitSellingPrice = request.UnitSellingPrice,
            UnitSellingPrice2 = request.UnitSellingPrice2,
            MinSalesQty2 = request.MinSalesQty2,
            UnitSellingPrice3 = request.UnitSellingPrice3,
            MinSalesQty3 = request.MinSalesQty3,
            PromoCode = request.PromoCode,
            PromoFromDate = request.PromoFromDate,
            PromoToDate = request.PromoToDate,
            MinQty = request.MinQty,
            MaxQty = request.MaxQty,
            ReorderQty = request.ReorderQty,
            LeadTimeDays = request.LeadTimeDays,
            PackagingQty = request.PackagingQty,
            Remark = request.Remark,
            ImageRef = request.ImageRef,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
        };

        db.Products.Add(entity);
        await db.SaveChangesAsync(ct);

        // Fetch again to populate navigation properties for the DTO
        var saved = await db.Products
            .Include(x => x.Category)
            .Include(x => x.SalesCogsAccount)
            .Include(x => x.SalesRevenueAccount)
            .Include(x => x.PurchaseCostAccount)
            .Include(x => x.PurchaseAccount)
            .Include(x => x.DefaultSupplier)
            .Include(x => x.MainUnitOfMeasure)
            .Include(x => x.AltUnitOfMeasure)
            .FirstAsync(x => x.Id == entity.Id, ct);

        return ServiceResult<ProductDto>.Success(ToDto(saved));
    }

    public async Task<ServiceResult<ProductDto>> EditAsync(long id, ProductUpsertDto request, CancellationToken ct = default)
    {
        var entity = await db.Products.FirstOrDefaultAsync(x => x.Id == id && x.IsActive, ct);
        if (entity is null)
        {
            return ServiceResult<ProductDto>.Failure("Not found.");
        }

        if (await CodeInUseAsync(request.Code, excludeId: id, ct))
        {
            return ServiceResult<ProductDto>.Failure($"Product code '{request.Code}' is already in use.");
        }

        entity.Code = request.Code;
        entity.Code2 = request.Code2;
        entity.ParentProductCode = request.ParentProductCode;
        entity.CategoryId = request.CategoryId;
        entity.Description = request.Description;
        entity.SalesCogsAccountId = request.SalesCogsAccountId;
        entity.SalesRevenueAccountId = request.SalesRevenueAccountId;
        entity.PurchaseCostAccountId = request.PurchaseCostAccountId;
        entity.PurchaseAccountId = request.PurchaseAccountId;
        entity.SalesTaxCode = request.SalesTaxCode;
        entity.PurchaseTaxCode = request.PurchaseTaxCode;
        entity.DefaultSupplierId = request.DefaultSupplierId;
        entity.MainUnitOfMeasureId = request.MainUnitOfMeasureId;
        entity.AltUnitOfMeasureId = request.AltUnitOfMeasureId;
        entity.ConversionFactor = request.ConversionFactor;
        entity.ShelfLifeDays = request.ShelfLifeDays;
        entity.UnitCostPrice = request.UnitCostPrice;
        entity.UnitSellingPrice = request.UnitSellingPrice;
        entity.UnitSellingPrice2 = request.UnitSellingPrice2;
        entity.MinSalesQty2 = request.MinSalesQty2;
        entity.UnitSellingPrice3 = request.UnitSellingPrice3;
        entity.MinSalesQty3 = request.MinSalesQty3;
        entity.PromoCode = request.PromoCode;
        entity.PromoFromDate = request.PromoFromDate;
        entity.PromoToDate = request.PromoToDate;
        entity.MinQty = request.MinQty;
        entity.MaxQty = request.MaxQty;
        entity.ReorderQty = request.ReorderQty;
        entity.LeadTimeDays = request.LeadTimeDays;
        entity.PackagingQty = request.PackagingQty;
        entity.Remark = request.Remark;
        entity.ImageRef = request.ImageRef;
        entity.ModifiedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);

        var saved = await db.Products
            .Include(x => x.Category)
            .Include(x => x.SalesCogsAccount)
            .Include(x => x.SalesRevenueAccount)
            .Include(x => x.PurchaseCostAccount)
            .Include(x => x.PurchaseAccount)
            .Include(x => x.DefaultSupplier)
            .Include(x => x.MainUnitOfMeasure)
            .Include(x => x.AltUnitOfMeasure)
            .FirstAsync(x => x.Id == entity.Id, ct);

        return ServiceResult<ProductDto>.Success(ToDto(saved));
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
    {
        var entity = await db.Products.FirstOrDefaultAsync(x => x.Id == id && x.IsActive, ct);
        if (entity is null)
        {
            return false;
        }

        entity.IsActive = false;
        entity.ModifiedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        return true;
    }

    private Task<bool> CodeInUseAsync(string code, long? excludeId, CancellationToken ct)
    {
        var normalized = code.Trim().ToLower();
        return db.Products.AnyAsync(x => x.IsActive && x.Code.ToLower() == normalized && (excludeId == null || x.Id != excludeId), ct);
    }

    private static ProductDto ToDto(Product x) =>
        new(
            x.Id,
            x.Code,
            x.Code2,
            x.ParentProductCode,
            x.CategoryId,
            x.Category != null ? x.Category.Name : null,
            x.Description,
            x.SalesCogsAccountId,
            x.SalesCogsAccount != null ? x.SalesCogsAccount.Name : null,
            x.SalesRevenueAccountId,
            x.SalesRevenueAccount != null ? x.SalesRevenueAccount.Name : null,
            x.PurchaseCostAccountId,
            x.PurchaseCostAccount != null ? x.PurchaseCostAccount.Name : null,
            x.PurchaseAccountId,
            x.PurchaseAccount != null ? x.PurchaseAccount.Name : null,
            x.SalesTaxCode,
            x.PurchaseTaxCode,
            x.DefaultSupplierId,
            x.DefaultSupplier != null ? x.DefaultSupplier.Name : null,
            x.MainUnitOfMeasureId,
            x.MainUnitOfMeasure != null ? x.MainUnitOfMeasure.Name : null,
            x.AltUnitOfMeasureId,
            x.AltUnitOfMeasure != null ? x.AltUnitOfMeasure.Name : null,
            x.ConversionFactor,
            x.ShelfLifeDays,
            x.UnitCostPrice,
            x.UnitSellingPrice,
            x.UnitSellingPrice2,
            x.MinSalesQty2,
            x.UnitSellingPrice3,
            x.MinSalesQty3,
            x.PromoCode,
            x.PromoFromDate,
            x.PromoToDate,
            x.MinQty,
            x.MaxQty,
            x.ReorderQty,
            x.LeadTimeDays,
            x.PackagingQty,
            x.Remark,
            x.ImageRef,
            x.IsActive);
}
