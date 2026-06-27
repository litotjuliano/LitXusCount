using LitXusCount.Application.Common;
using LitXusCount.Application.Sales;
using LitXusCount.Domain.Entities;
using LitXusCount.Domain.Enums;
using LitXusCount.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LitXusCount.Infrastructure.Sales;

internal sealed class SalesInvoiceLineService(ApplicationDbContext db, IStockService stockService) : ISalesInvoiceLineService
{
    internal ApplicationDbContext Db => db;

    public async Task<ServiceResult<SalesInvoiceLineDto>> AddLineAsync(SalesInvoiceLineCreateDto request, CancellationToken ct = default)
    {
        var invoice = await db.SalesInvoices.FirstOrDefaultAsync(x => x.Id == request.SalesInvoiceId && x.IsActive, ct);
        if (invoice is null)
            return ServiceResult<SalesInvoiceLineDto>.Failure("Invoice not found.");

        var product = await db.Products
            .Include(x => x.MainUnitOfMeasure)
            .FirstOrDefaultAsync(x => x.Id == request.ProductId && x.IsActive, ct);
        if (product is null)
            return ServiceResult<SalesInvoiceLineDto>.Failure("Product not found.");

        if (request.Quantity <= 0)
            return ServiceResult<SalesInvoiceLineDto>.Failure("Quantity must be greater than zero.");

        var subLineTotal = request.Quantity * request.UnitPrice;
        var discountAmount = Math.Round(subLineTotal * (request.ItemDiscount / 100m), 4);
        var vatBase = subLineTotal - discountAmount;
        var vatAmount = Math.Round(vatBase * (request.ItemVAT / 100m), 4);
        var totalAmount = vatBase + vatAmount;

        var line = new SalesInvoiceLine
        {
            SalesInvoiceId = request.SalesInvoiceId,
            ProductId = request.ProductId,
            ItemName = product.Description ?? product.Code,
            Quantity = request.Quantity,
            UnitPrice = request.UnitPrice,
            ItemVAT = request.ItemVAT,
            ItemVATAmount = vatAmount,
            ItemDiscount = request.ItemDiscount,
            ItemDiscountAmount = discountAmount,
            TotalAmount = totalAmount,
            ClassificationCode = request.ClassificationCode ?? product.DefaultLhdnClassificationCode,
            TaxTypeCode = request.TaxTypeCode ?? product.DefaultLhdnTaxTypeCode,
            UnitCode = product.MainUnitOfMeasure?.UnCefactCode,
            IsReturn = false,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
        };
        db.SalesInvoiceLines.Add(line);

        await stockService.UpdateStockAsync(product.Id, request.Quantity, isAddition: false, ct);

        await db.SaveChangesAsync(ct);

        await RecalcTotalsAsync(invoice.Id, ct);
        await db.SaveChangesAsync(ct);

        line.Product = product;
        return ServiceResult<SalesInvoiceLineDto>.Success(ToDto(line));
    }

    public async Task<bool> RemoveLineAsync(long lineId, CancellationToken ct = default)
    {
        var line = await db.SalesInvoiceLines.Include(x => x.Product)
            .FirstOrDefaultAsync(x => x.Id == lineId && x.IsActive, ct);
        if (line is null) return false;

        line.IsActive = false;
        line.ModifiedAt = DateTime.UtcNow;

        await stockService.UpdateStockAsync(line.ProductId, line.Quantity, isAddition: !line.IsReturn, ct);

        await db.SaveChangesAsync(ct);

        await RecalcTotalsAsync(line.SalesInvoiceId, ct);
        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<IReadOnlyList<SalesInvoiceLineDto>> GetByInvoiceAsync(long invoiceId, CancellationToken ct = default)
    {
        return await db.SalesInvoiceLines.Include(x => x.Product)
            .Where(x => x.SalesInvoiceId == invoiceId && x.IsActive)
            .OrderBy(x => x.Id)
            .Select(x => ToDto(x))
            .ToListAsync(ct);
    }

    private async Task RecalcTotalsAsync(long invoiceId, CancellationToken ct)
    {
        var invoice = await db.SalesInvoices.FindAsync(new object?[] { invoiceId }, ct);
        if (invoice is null) return;

        var lines = await db.SalesInvoiceLines
            .Where(x => x.SalesInvoiceId == invoiceId && x.IsActive && !x.IsReturn)
            .ToListAsync(ct);

        var subTotal        = lines.Sum(l => l.Quantity * l.UnitPrice);
        var discountAmount  = lines.Sum(l => l.ItemDiscountAmount);
        var vatAmount       = lines.Sum(l => l.ItemVATAmount);
        var grandTotal      = subTotal - discountAmount + vatAmount;
        var dueAmount       = grandTotal - invoice.PaidAmount;

        invoice.SubTotal        = subTotal;
        invoice.DiscountAmount  = discountAmount;
        invoice.VATAmount       = vatAmount;
        invoice.GrandTotal      = grandTotal;
        invoice.DueAmount       = dueAmount < 0 ? 0 : dueAmount;
        invoice.PaymentStatus   = invoice.PaidAmount >= grandTotal && grandTotal > 0
            ? SalesPaymentStatus.Paid
            : invoice.PaidAmount > 0
                ? SalesPaymentStatus.PartiallyPaid
                : SalesPaymentStatus.Unpaid;
        invoice.ModifiedAt = DateTime.UtcNow;
    }

    private static SalesInvoiceLineDto ToDto(SalesInvoiceLine x) =>
        new(x.Id, x.SalesInvoiceId, x.ProductId, x.ItemName,
            x.Quantity, x.UnitPrice, x.ItemVAT, x.ItemVATAmount,
            x.ItemDiscount, x.ItemDiscountAmount, x.TotalAmount,
            x.IsReturn, x.IsActive,
            x.ClassificationCode, x.TaxTypeCode, x.UnitCode);
}
