using LitXusCount.Domain.Entities;
using LitXusCount.Domain.Enums;
using LitXusCount.Infrastructure.Persistence;
using LitXusCount.Infrastructure.Sales;
using LitXusCount.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LitXusCount.Tests.Sales;

public class SalesInvoiceServiceTests
{
    private static async Task<(ApplicationDbContext db, Customer customer)> SetupAsync()
    {
        var db = TestDbContextFactory.Create();
        var customer = new Customer
        {
            Code = "C001", Name = "Test Customer", GlAccountId = 0,
            IsActive = true, CreatedAt = DateTime.UtcNow
        };
        db.Customers.Add(customer);
        await db.SaveChangesAsync();
        return (db, customer);
    }

    [Fact]
    public async Task CreateDraftAsync_CreatesDraftWithDPrefix()
    {
        var (db, customer) = await SetupAsync();
        var svc = new SalesInvoiceService(db);

        var result = await svc.CreateDraftAsync(new(customer.Id, null));

        Assert.True(result.Succeeded);
        var inv = await db.SalesInvoices.FirstAsync();
        Assert.Equal(SalesInvoiceCategory.Draft, inv.Category);
        Assert.StartsWith("D-", inv.InvoiceNo);
    }

    [Fact]
    public async Task CreateDraftAsync_SecondDraft_IncrementsNumber()
    {
        var (db, customer) = await SetupAsync();
        var svc = new SalesInvoiceService(db);

        await svc.CreateDraftAsync(new(customer.Id, null));
        var result2 = await svc.CreateDraftAsync(new(customer.Id, null));

        Assert.True(result2.Succeeded);
        var invoices = await db.SalesInvoices.OrderBy(x => x.Id).ToListAsync();
        Assert.Equal(2, invoices.Count);
        Assert.NotEqual(invoices[0].InvoiceNo, invoices[1].InvoiceNo);
    }

    [Fact]
    public async Task PromoteAsync_AssignsINVPrefix_ForRegular()
    {
        var (db, customer) = await SetupAsync();
        var svc = new SalesInvoiceService(db);

        var draft = await svc.CreateDraftAsync(new(customer.Id, null));
        var promoted = await svc.PromoteAsync(draft.Value!.Id, SalesInvoiceCategory.Regular);

        Assert.True(promoted.Succeeded);
        Assert.Equal(SalesInvoiceCategory.Regular, promoted.Value!.Category);
        Assert.StartsWith("INV-", promoted.Value.InvoiceNo);
    }

    [Fact]
    public async Task PromoteAsync_AssignsQTPrefix_ForQuote()
    {
        var (db, customer) = await SetupAsync();
        var svc = new SalesInvoiceService(db);

        var draft = await svc.CreateDraftAsync(new(customer.Id, null));
        var promoted = await svc.PromoteAsync(draft.Value!.Id, SalesInvoiceCategory.Quote);

        Assert.True(promoted.Succeeded);
        Assert.StartsWith("QT-", promoted.Value!.InvoiceNo);
    }

    [Fact]
    public async Task DeleteAsync_SoftDeletesInvoice()
    {
        var (db, customer) = await SetupAsync();
        var svc = new SalesInvoiceService(db);

        var draft = await svc.CreateDraftAsync(new(customer.Id, null));
        var deleted = await svc.DeleteAsync(draft.Value!.Id);

        Assert.True(deleted);
        var inv = await db.SalesInvoices.FirstAsync();
        Assert.False(inv.IsActive);
    }

    [Fact]
    public async Task PermissionsAll_ContainsSalesPermissions()
    {
        var all = LitXusCount.Application.Authorization.Permissions.All;
        Assert.Contains("Sales.Invoice.View", all);
        Assert.Contains("Sales.Invoice.Create", all);
        Assert.Contains("Sales.Invoice.Manage", all);
    }
}
