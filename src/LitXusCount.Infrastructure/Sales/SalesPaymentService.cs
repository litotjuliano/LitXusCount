using LitXusCount.Application.Accounts;
using LitXusCount.Application.Common;
using LitXusCount.Application.Sales;
using LitXusCount.Domain.Entities;
using LitXusCount.Domain.Enums;
using LitXusCount.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LitXusCount.Infrastructure.Sales;

internal sealed class SalesPaymentService(ApplicationDbContext db, IAccAccountService accAccountService) : ISalesPaymentService
{
    internal ApplicationDbContext Db => db;

    public async Task<ServiceResult<SalesPaymentRecordDto>> RecordPaymentAsync(SalesPaymentRecordCreateDto request, CancellationToken ct = default)
    {
        if (request.Amount <= 0)
            return ServiceResult<SalesPaymentRecordDto>.Failure("Amount must be greater than zero.");

        var invoice = await db.SalesInvoices.Include(x => x.Customer)
            .FirstOrDefaultAsync(x => x.Id == request.SalesInvoiceId && x.IsActive, ct);
        if (invoice is null)
            return ServiceResult<SalesPaymentRecordDto>.Failure("Invoice not found.");

        var account = await db.AccAccounts.FirstOrDefaultAsync(x => x.Id == request.AccAccountId && x.IsActive, ct);
        if (account is null)
            return ServiceResult<SalesPaymentRecordDto>.Failure("Account not found or inactive.");

        var payment = new SalesPaymentRecord
        {
            SalesInvoiceId = request.SalesInvoiceId,
            AccAccountId = request.AccAccountId,
            ModeOfPayment = request.ModeOfPayment,
            Amount = request.Amount,
            ReferenceNo = request.ReferenceNo?.Trim(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
        };
        db.SalesPaymentRecords.Add(payment);

        await accAccountService.UpdateBalanceAsync(
            request.AccAccountId, request.Amount,
            AccTransactionType.SalesPayment,
            $"INV-{request.SalesInvoiceId}", ct);

        invoice.PaidAmount += request.Amount;
        invoice.DueAmount = invoice.GrandTotal - invoice.PaidAmount;
        if (invoice.DueAmount < 0) invoice.DueAmount = 0;
        invoice.PaymentStatus = invoice.DueAmount == 0
            ? SalesPaymentStatus.Paid
            : SalesPaymentStatus.PartiallyPaid;
        invoice.ModifiedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);

        payment.AccAccount = account;
        return ServiceResult<SalesPaymentRecordDto>.Success(ToDto(payment));
    }

    public async Task<bool> DeletePaymentAsync(long paymentId, CancellationToken ct = default)
    {
        var payment = await db.SalesPaymentRecords.Include(x => x.AccAccount)
            .FirstOrDefaultAsync(x => x.Id == paymentId && x.IsActive, ct);
        if (payment is null) return false;

        var invoice = await db.SalesInvoices.FirstOrDefaultAsync(x => x.Id == payment.SalesInvoiceId && x.IsActive, ct);
        if (invoice is null) return false;

        payment.IsActive = false;
        payment.ModifiedAt = DateTime.UtcNow;

        await accAccountService.UpdateBalanceAsync(
            payment.AccAccountId, payment.Amount,
            AccTransactionType.SalesPaymentReversal,
            $"INV-{payment.SalesInvoiceId}-REV", ct);

        invoice.PaidAmount -= payment.Amount;
        if (invoice.PaidAmount < 0) invoice.PaidAmount = 0;
        invoice.DueAmount = invoice.GrandTotal - invoice.PaidAmount;
        invoice.PaymentStatus = invoice.PaidAmount <= 0
            ? SalesPaymentStatus.Unpaid
            : invoice.DueAmount <= 0
                ? SalesPaymentStatus.Paid
                : SalesPaymentStatus.PartiallyPaid;
        invoice.ModifiedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<IReadOnlyList<SalesPaymentRecordDto>> GetByInvoiceAsync(long invoiceId, CancellationToken ct = default)
    {
        return await db.SalesPaymentRecords.Include(x => x.AccAccount)
            .Where(x => x.SalesInvoiceId == invoiceId && x.IsActive)
            .OrderBy(x => x.Id)
            .Select(x => ToDto(x))
            .ToListAsync(ct);
    }

    private static SalesPaymentRecordDto ToDto(SalesPaymentRecord x) =>
        new(x.Id, x.SalesInvoiceId, x.AccAccountId,
            x.AccAccount?.AccountName ?? string.Empty,
            x.ModeOfPayment, x.Amount, x.ReferenceNo, x.IsActive);
}
