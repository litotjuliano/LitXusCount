using LitXusCount.Application.Common;
using LitXusCount.Application.Sales;
using LitXusCount.Domain.Entities;
using LitXusCount.Domain.Enums;
using LitXusCount.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LitXusCount.Infrastructure.Sales;

internal sealed class SalesInvoiceService(ApplicationDbContext db) : ISalesInvoiceService
{
    internal ApplicationDbContext Db => db;

    public async Task<ServiceResult<SalesInvoiceDto>> CreateDraftAsync(SalesInvoiceCreateDto request, CancellationToken ct = default)
    {
        var customer = await db.Customers.FirstOrDefaultAsync(x => x.Id == request.CustomerId && x.IsActive, ct);
        if (customer is null)
            return ServiceResult<SalesInvoiceDto>.Failure("Customer not found or inactive.");

        var nextNum = await NextNumberAsync(SalesInvoiceCategory.Draft, ct);
        var invoice = new SalesInvoice
        {
            CustomerId = request.CustomerId,
            Category = SalesInvoiceCategory.Draft,
            InvoiceNo = $"D-{nextNum}",
            PaymentStatus = SalesPaymentStatus.Unpaid,
            CurrencyId = request.CurrencyId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
        };
        db.SalesInvoices.Add(invoice);
        await db.SaveChangesAsync(ct);

        invoice.Customer = customer;
        return ServiceResult<SalesInvoiceDto>.Success(ToDto(invoice));
    }

    public async Task<ServiceResult<SalesInvoiceDto>> UpdateHeaderAsync(long invoiceId, SalesInvoiceHeaderUpdateDto request, CancellationToken ct = default)
    {
        var invoice = await db.SalesInvoices.Include(x => x.Customer)
            .FirstOrDefaultAsync(x => x.Id == invoiceId && x.IsActive, ct);
        if (invoice is null)
            return ServiceResult<SalesInvoiceDto>.Failure("Invoice not found.");

        if (request.CustomerId != invoice.CustomerId)
        {
            var customer = await db.Customers.FirstOrDefaultAsync(x => x.Id == request.CustomerId && x.IsActive, ct);
            if (customer is null)
                return ServiceResult<SalesInvoiceDto>.Failure("Customer not found or inactive.");
            invoice.CustomerId = request.CustomerId;
            invoice.Customer = customer;
        }

        invoice.PurchaseOrderNumber = request.PurchaseOrderNumber?.Trim();
        invoice.Notes = request.Notes?.Trim();
        invoice.CurrencyId = request.CurrencyId;
        invoice.ModifiedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);

        return ServiceResult<SalesInvoiceDto>.Success(ToDto(invoice));
    }

    public async Task<ServiceResult<SalesInvoiceDto>> PromoteAsync(long invoiceId, SalesInvoiceCategory targetCategory, CancellationToken ct = default)
    {
        if (targetCategory == SalesInvoiceCategory.Draft)
            return ServiceResult<SalesInvoiceDto>.Failure("Cannot promote to Draft.");

        var invoice = await db.SalesInvoices.Include(x => x.Customer)
            .FirstOrDefaultAsync(x => x.Id == invoiceId && x.IsActive, ct);
        if (invoice is null)
            return ServiceResult<SalesInvoiceDto>.Failure("Invoice not found.");

        var nextNum = await NextNumberAsync(targetCategory, ct);
        var prefix = targetCategory switch
        {
            SalesInvoiceCategory.Regular => "INV",
            SalesInvoiceCategory.Quote   => "QT",
            SalesInvoiceCategory.Manual  => "M",
            _                            => "INV",
        };

        invoice.Category = targetCategory;
        invoice.InvoiceNo = $"{prefix}-{nextNum}";
        invoice.ModifiedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);

        return ServiceResult<SalesInvoiceDto>.Success(ToDto(invoice));
    }

    public async Task<PagedResult<SalesInvoiceDto>> GetPagedAsync(PagedQuery query, SalesInvoiceCategory? categoryFilter = null, CancellationToken ct = default)
    {
        var filtered = db.SalesInvoices.Include(x => x.Customer).Where(x => x.IsActive);

        if (categoryFilter.HasValue)
            filtered = filtered.Where(x => x.Category == categoryFilter.Value);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var pattern = $"%{query.Search.Trim()}%";
            filtered = filtered.Where(x =>
                EF.Functions.Like(x.InvoiceNo, pattern) ||
                EF.Functions.Like(x.Customer.Name, pattern));
        }

        var totalCount = await filtered.CountAsync(ct);
        var items = await filtered
            .OrderByDescending(x => x.CreatedAt)
            .Skip((query.EffectivePage - 1) * query.EffectivePageSize)
            .Take(query.EffectivePageSize)
            .Select(x => ToDto(x))
            .ToListAsync(ct);

        return new PagedResult<SalesInvoiceDto>(items, totalCount, query.EffectivePage, query.EffectivePageSize);
    }

    public async Task<SalesInvoiceDto?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var entity = await db.SalesInvoices.Include(x => x.Customer)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
        return entity is null ? null : ToDto(entity);
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
    {
        var invoice = await db.SalesInvoices
            .Include(x => x.Lines.Where(l => l.IsActive))
            .Include(x => x.Payments.Where(p => p.IsActive))
            .FirstOrDefaultAsync(x => x.Id == id && x.IsActive, ct);
        if (invoice is null) return false;

        // Reverse stock for all active lines
        foreach (var line in invoice.Lines)
        {
            var product = await db.Products.FindAsync(new object?[] { line.ProductId }, ct);
            if (product is not null)
            {
                product.StockQuantity += line.IsReturn ? -line.Quantity : line.Quantity;
                product.ModifiedAt = DateTime.UtcNow;
            }
            line.IsActive = false;
            line.ModifiedAt = DateTime.UtcNow;
        }

        // Reverse all account payments
        foreach (var payment in invoice.Payments)
        {
            var account = await db.AccAccounts.FindAsync(new object?[] { payment.AccAccountId }, ct);
            if (account is not null)
            {
                account.Credit -= payment.Amount;
                account.Balance -= payment.Amount;
                account.ModifiedAt = DateTime.UtcNow;
                db.AccTransactions.Add(new Domain.Entities.AccTransaction
                {
                    AccAccountId = payment.AccAccountId,
                    Type = AccTransactionType.SalesPaymentReversal,
                    Debit = payment.Amount,
                    Amount = payment.Amount,
                    Reference = $"INV-{invoice.Id}-REV",
                    CreatedAt = DateTime.UtcNow,
                });
            }
            payment.IsActive = false;
            payment.ModifiedAt = DateTime.UtcNow;
        }

        invoice.IsActive = false;
        invoice.ModifiedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        return true;
    }

    private async Task<int> NextNumberAsync(SalesInvoiceCategory category, CancellationToken ct)
    {
        var prefix = category switch
        {
            SalesInvoiceCategory.Draft   => "D-",
            SalesInvoiceCategory.Regular => "INV-",
            SalesInvoiceCategory.Quote   => "QT-",
            SalesInvoiceCategory.Manual  => "M-",
            _                            => "INV-",
        };

        var max = await db.SalesInvoices
            .Where(x => x.InvoiceNo.StartsWith(prefix))
            .Select(x => x.InvoiceNo)
            .ToListAsync(ct);

        var maxNum = max
            .Select(n => int.TryParse(n[prefix.Length..], out var v) ? v : 0)
            .DefaultIfEmpty(0)
            .Max();

        return maxNum + 1;
    }

    private static SalesInvoiceDto ToDto(SalesInvoice x) =>
        new(x.Id, x.CustomerId, x.Customer?.Name ?? string.Empty, x.Category, x.InvoiceNo,
            x.SubTotal, x.DiscountAmount, x.VATAmount, x.GrandTotal,
            x.PaidAmount, x.DueAmount, x.PaymentStatus,
            x.PurchaseOrderNumber, x.Notes, x.CurrencyId, x.IsActive, x.CreatedAt);
}
