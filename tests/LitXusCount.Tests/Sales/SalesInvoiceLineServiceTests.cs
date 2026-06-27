using LitXusCount.Domain.Entities;
using LitXusCount.Domain.Enums;
using LitXusCount.Infrastructure.Sales;
using LitXusCount.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LitXusCount.Tests.Sales;

public class SalesInvoiceLineServiceTests
{
    private static async Task<(Infrastructure.Persistence.ApplicationDbContext db, SalesInvoice invoice, Product product)> SetupAsync()
    {
        var db = TestDbContextFactory.Create();
        var customer = new Customer
        {
            Code = "C001", Name = "Test Customer", GlAccountId = 0,
            IsActive = true, CreatedAt = DateTime.UtcNow
        };
        db.Customers.Add(customer);
        var product = new Product
        {
            Code = "P001", Description = "Widget", StockQuantity = 100,
            UnitSellingPrice = 10, ConversionFactor = 1,
            IsActive = true, CreatedAt = DateTime.UtcNow
        };
        db.Products.Add(product);
        await db.SaveChangesAsync();

        var invoice = new SalesInvoice
        {
            CustomerId = customer.Id, Category = SalesInvoiceCategory.Draft,
            InvoiceNo = "D-1", PaymentStatus = SalesPaymentStatus.Unpaid,
            IsActive = true, CreatedAt = DateTime.UtcNow
        };
        db.SalesInvoices.Add(invoice);
        await db.SaveChangesAsync();

        return (db, invoice, product);
    }

    [Fact]
    public async Task AddLineAsync_DeductsStock()
    {
        var (db, invoice, product) = await SetupAsync();
        var stockSvc = new StockService(db);
        var svc = new SalesInvoiceLineService(db, stockSvc);

        await svc.AddLineAsync(new(invoice.Id, product.Id, 5, 10, 0, 0));

        var updated = await db.Products.FindAsync(product.Id);
        Assert.Equal(95, updated!.StockQuantity);
    }

    [Fact]
    public async Task AddLineAsync_StoresItemNameSnapshot()
    {
        var (db, invoice, product) = await SetupAsync();
        product.Description = "Special Widget";
        await db.SaveChangesAsync();

        var stockSvc = new StockService(db);
        var svc = new SalesInvoiceLineService(db, stockSvc);

        var result = await svc.AddLineAsync(new(invoice.Id, product.Id, 2, 10, 0, 0));

        Assert.True(result.Succeeded);
        Assert.Equal("Special Widget", result.Value!.ItemName);
    }

    [Fact]
    public async Task AddLineAsync_RecalculatesInvoiceTotals()
    {
        var (db, invoice, product) = await SetupAsync();
        var stockSvc = new StockService(db);
        var svc = new SalesInvoiceLineService(db, stockSvc);

        await svc.AddLineAsync(new(invoice.Id, product.Id, 3, 20, 0, 0));

        var updated = await db.SalesInvoices.FindAsync(invoice.Id);
        Assert.Equal(60, updated!.SubTotal);
        Assert.Equal(60, updated.GrandTotal);
        Assert.Equal(60, updated.DueAmount);
    }

    [Fact]
    public async Task AddLineAsync_WithVATAndDiscount_RecalculatesTotals()
    {
        var (db, invoice, product) = await SetupAsync();
        var stockSvc = new StockService(db);
        var svc = new SalesInvoiceLineService(db, stockSvc);

        // Qty=10, Price=100, VAT=10%, Discount=5%
        // SubTotal=1000, DiscountAmount=50, vatBase=950, VATAmount=95, GrandTotal=1045
        await svc.AddLineAsync(new(invoice.Id, product.Id, 10, 100, 10, 5));

        var updated = await db.SalesInvoices.FindAsync(invoice.Id);
        Assert.Equal(1000, updated!.SubTotal);
        Assert.Equal(50, updated.DiscountAmount);
        Assert.Equal(95, updated.VATAmount);
        Assert.Equal(1045, updated.GrandTotal);
    }

    [Fact]
    public async Task RemoveLineAsync_RestoresStock()
    {
        var (db, invoice, product) = await SetupAsync();
        var stockSvc = new StockService(db);
        var svc = new SalesInvoiceLineService(db, stockSvc);

        var addResult = await svc.AddLineAsync(new(invoice.Id, product.Id, 5, 10, 0, 0));
        await svc.RemoveLineAsync(addResult.Value!.Id);

        var updated = await db.Products.FindAsync(product.Id);
        Assert.Equal(100, updated!.StockQuantity);
    }

    [Fact]
    public async Task RemoveLineAsync_RecalculatesInvoiceTotalsToZero()
    {
        var (db, invoice, product) = await SetupAsync();
        var stockSvc = new StockService(db);
        var svc = new SalesInvoiceLineService(db, stockSvc);

        var addResult = await svc.AddLineAsync(new(invoice.Id, product.Id, 3, 20, 0, 0));
        await svc.RemoveLineAsync(addResult.Value!.Id);

        var updated = await db.SalesInvoices.FindAsync(invoice.Id);
        Assert.Equal(0, updated!.SubTotal);
        Assert.Equal(0, updated.GrandTotal);
    }
}
