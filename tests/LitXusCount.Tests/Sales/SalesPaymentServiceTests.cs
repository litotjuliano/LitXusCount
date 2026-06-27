using LitXusCount.Domain.Entities;
using LitXusCount.Domain.Enums;
using LitXusCount.Infrastructure.Accounts;
using LitXusCount.Infrastructure.Sales;
using LitXusCount.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LitXusCount.Tests.Sales;

public class SalesPaymentServiceTests
{
    private static async Task<(Infrastructure.Persistence.ApplicationDbContext db, SalesInvoice invoice, AccAccount account)> SetupAsync(decimal grandTotal = 1000)
    {
        var db = TestDbContextFactory.Create();
        var customer = new Customer
        {
            Code = "C001", Name = "Test Customer", GlAccountId = 0,
            IsActive = true, CreatedAt = DateTime.UtcNow
        };
        db.Customers.Add(customer);
        var account = new AccAccount
        {
            Code = "CASH", AccountName = "Cash",
            Credit = 0, Debit = 0, Balance = 0,
            IsActive = true, CreatedAt = DateTime.UtcNow
        };
        db.AccAccounts.Add(account);
        await db.SaveChangesAsync();

        var invoice = new SalesInvoice
        {
            CustomerId = customer.Id,
            Category = SalesInvoiceCategory.Regular,
            InvoiceNo = "INV-1",
            GrandTotal = grandTotal,
            DueAmount = grandTotal,
            PaidAmount = 0,
            PaymentStatus = SalesPaymentStatus.Unpaid,
            IsActive = true, CreatedAt = DateTime.UtcNow
        };
        db.SalesInvoices.Add(invoice);
        await db.SaveChangesAsync();

        return (db, invoice, account);
    }

    [Fact]
    public async Task RecordPaymentAsync_UpdatesAccAccountBalance()
    {
        var (db, invoice, account) = await SetupAsync();
        var accSvc = new AccAccountService(db);
        var svc = new SalesPaymentService(db, accSvc);

        await svc.RecordPaymentAsync(new(invoice.Id, account.Id, "Cash", 500, null));

        var updatedAccount = await db.AccAccounts.FindAsync(account.Id);
        Assert.Equal(500, updatedAccount!.Balance);
        Assert.Equal(500, updatedAccount.Credit);
    }

    [Fact]
    public async Task RecordPaymentAsync_SetsPaidStatus_WhenDueIsZero()
    {
        var (db, invoice, account) = await SetupAsync(grandTotal: 500);
        var accSvc = new AccAccountService(db);
        var svc = new SalesPaymentService(db, accSvc);

        await svc.RecordPaymentAsync(new(invoice.Id, account.Id, "Cash", 500, null));

        var updated = await db.SalesInvoices.FindAsync(invoice.Id);
        Assert.Equal(SalesPaymentStatus.Paid, updated!.PaymentStatus);
        Assert.Equal(0, updated.DueAmount);
        Assert.Equal(500, updated.PaidAmount);
    }

    [Fact]
    public async Task RecordPaymentAsync_SetsPartialStatus_WhenDueRemains()
    {
        var (db, invoice, account) = await SetupAsync(grandTotal: 1000);
        var accSvc = new AccAccountService(db);
        var svc = new SalesPaymentService(db, accSvc);

        await svc.RecordPaymentAsync(new(invoice.Id, account.Id, "Cash", 400, null));

        var updated = await db.SalesInvoices.FindAsync(invoice.Id);
        Assert.Equal(SalesPaymentStatus.PartiallyPaid, updated!.PaymentStatus);
        Assert.Equal(600, updated.DueAmount);
        Assert.Equal(400, updated.PaidAmount);
    }

    [Fact]
    public async Task DeletePaymentAsync_ReversesAccAccountBalance()
    {
        var (db, invoice, account) = await SetupAsync();
        var accSvc = new AccAccountService(db);
        var svc = new SalesPaymentService(db, accSvc);

        var payment = await svc.RecordPaymentAsync(new(invoice.Id, account.Id, "Cash", 300, null));
        await svc.DeletePaymentAsync(payment.Value!.Id);

        var updatedAccount = await db.AccAccounts.FindAsync(account.Id);
        Assert.Equal(0, updatedAccount!.Balance);
        Assert.Equal(0, updatedAccount.Credit);
    }

    [Fact]
    public async Task DeletePaymentAsync_RestoresUnpaidStatus()
    {
        var (db, invoice, account) = await SetupAsync(grandTotal: 500);
        var accSvc = new AccAccountService(db);
        var svc = new SalesPaymentService(db, accSvc);

        var payment = await svc.RecordPaymentAsync(new(invoice.Id, account.Id, "Cash", 500, null));
        await svc.DeletePaymentAsync(payment.Value!.Id);

        var updated = await db.SalesInvoices.FindAsync(invoice.Id);
        Assert.Equal(SalesPaymentStatus.Unpaid, updated!.PaymentStatus);
        Assert.Equal(500, updated.DueAmount);
        Assert.Equal(0, updated.PaidAmount);
    }

    [Fact]
    public async Task RecordPaymentAsync_AppendsSalesPaymentTransaction()
    {
        var (db, invoice, account) = await SetupAsync();
        var accSvc = new AccAccountService(db);
        var svc = new SalesPaymentService(db, accSvc);

        await svc.RecordPaymentAsync(new(invoice.Id, account.Id, "Cash", 200, "REF-001"));

        var txns = await db.AccTransactions.Where(x => x.AccAccountId == account.Id).ToListAsync();
        Assert.Single(txns);
        Assert.Equal(AccTransactionType.SalesPayment, txns[0].Type);
        Assert.Equal(200, txns[0].Credit);
    }
}
