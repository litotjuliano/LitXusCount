using LitXusCount.Domain.Entities;
using LitXusCount.Domain.Enums;
using LitXusCount.Infrastructure.Accounts;
using LitXusCount.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LitXusCount.Tests.Accounts;

/// <summary>
/// Tests for IAccAccountService.UpdateBalanceAsync — the method called by Phase 4/5
/// Sales and Purchase payment recording. Covers all 4 payment transaction types.
/// </summary>
public class UpdateBalanceAsyncTests
{
    private static async Task<(AccAccountService svc, AccAccount account)> SetupAsync(decimal credit = 0, decimal debit = 0)
    {
        var ctx = TestDbContextFactory.Create();
        var account = new AccAccount
        {
            Code = "ACC", AccountName = "Test Account",
            Credit = credit, Debit = debit, Balance = credit - debit,
            IsActive = true, CreatedAt = DateTime.UtcNow
        };
        ctx.AccAccounts.Add(account);
        await ctx.SaveChangesAsync();
        return (new AccAccountService(ctx), account);
    }

    // ── SalesPayment: money IN (credit account) ───────────────────────────────

    [Fact]
    public async Task SalesPayment_IncreasesCreditAndBalance()
    {
        var (svc, account) = await SetupAsync();
        await svc.UpdateBalanceAsync(account.Id, 500m, AccTransactionType.SalesPayment, "INV-1", default);
        await svc.Db.SaveChangesAsync(); // caller normally does this

        await svc.Db.Entry(account).ReloadAsync();
        Assert.Equal(500m, account.Credit);
        Assert.Equal(0m, account.Debit);
        Assert.Equal(500m, account.Balance);
    }

    [Fact]
    public async Task SalesPayment_AppendsTransactionWithCredit()
    {
        var (svc, account) = await SetupAsync();
        await svc.UpdateBalanceAsync(account.Id, 200m, AccTransactionType.SalesPayment, "INV-2", default);
        await svc.Db.SaveChangesAsync();

        var tx = await svc.Db.AccTransactions.FirstAsync();
        Assert.Equal(AccTransactionType.SalesPayment, tx.Type);
        Assert.Equal(200m, tx.Credit);
        Assert.Equal(200m, tx.Amount);
        Assert.Equal("INV-2", tx.Reference);
    }

    // ── SalesPaymentReversal: undo Sales payment ──────────────────────────────

    [Fact]
    public async Task SalesPaymentReversal_DecreasesCreditAndBalance()
    {
        var (svc, account) = await SetupAsync(credit: 500m);
        await svc.UpdateBalanceAsync(account.Id, 200m, AccTransactionType.SalesPaymentReversal, "INV-1-REV", default);
        await svc.Db.SaveChangesAsync();

        await svc.Db.Entry(account).ReloadAsync();
        Assert.Equal(300m, account.Credit);
        Assert.Equal(300m, account.Balance);
    }

    // ── PurchasePayment: money OUT (debit account) ────────────────────────────

    [Fact]
    public async Task PurchasePayment_IncreasesDebitAndDecreasesBalance()
    {
        var (svc, account) = await SetupAsync(credit: 1000m);
        await svc.UpdateBalanceAsync(account.Id, 300m, AccTransactionType.PurchasePayment, "PO-1", default);
        await svc.Db.SaveChangesAsync();

        await svc.Db.Entry(account).ReloadAsync();
        Assert.Equal(300m, account.Debit);
        Assert.Equal(700m, account.Balance);   // 1000 - 300
        Assert.Equal(1000m, account.Credit);   // unchanged
    }

    [Fact]
    public async Task PurchasePayment_AppendsTransactionWithDebit()
    {
        var (svc, account) = await SetupAsync(credit: 1000m);
        await svc.UpdateBalanceAsync(account.Id, 400m, AccTransactionType.PurchasePayment, "PO-2", default);
        await svc.Db.SaveChangesAsync();

        var tx = await svc.Db.AccTransactions.FirstAsync();
        Assert.Equal(AccTransactionType.PurchasePayment, tx.Type);
        Assert.Equal(400m, tx.Debit);
        Assert.Equal(400m, tx.Amount);
    }

    // ── PurchasePaymentReversal: undo Purchase payment ────────────────────────

    [Fact]
    public async Task PurchasePaymentReversal_DecreasesDebitAndIncreasesBalance()
    {
        var (svc, account) = await SetupAsync(credit: 1000m, debit: 400m);
        // balance = 1000 - 400 = 600
        await svc.UpdateBalanceAsync(account.Id, 400m, AccTransactionType.PurchasePaymentReversal, "PO-1-REV", default);
        await svc.Db.SaveChangesAsync();

        await svc.Db.Entry(account).ReloadAsync();
        Assert.Equal(0m, account.Debit);
        Assert.Equal(1000m, account.Balance);
    }

    // ── Does NOT call SaveChangesAsync internally ─────────────────────────────

    [Fact]
    public async Task UpdateBalanceAsync_DoesNotCommitByItself()
    {
        var ctx = TestDbContextFactory.Create();
        var account = new AccAccount
        {
            Code = "A", AccountName = "A", Credit = 0, Debit = 0, Balance = 0,
            IsActive = true, CreatedAt = DateTime.UtcNow
        };
        ctx.AccAccounts.Add(account);
        await ctx.SaveChangesAsync();

        var svc = new AccAccountService(ctx);

        // Call UpdateBalanceAsync but do NOT call SaveChangesAsync
        await svc.UpdateBalanceAsync(account.Id, 100m, AccTransactionType.SalesPayment, null, default);

        // Reload directly — should still be 0 because save wasn't called
        var fresh = await ctx.AccAccounts.AsNoTracking().FirstAsync();
        // Note: EF change tracker still holds the change, so we check AccTransactions count instead
        // The transaction row should be tracked but not persisted
        var txCount = await ctx.AccTransactions.AsNoTracking().CountAsync();
        Assert.Equal(0, txCount); // nothing committed yet
    }
}
