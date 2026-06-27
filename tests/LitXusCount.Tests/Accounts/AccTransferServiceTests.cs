using LitXusCount.Application.Accounts;
using LitXusCount.Domain.Entities;
using LitXusCount.Domain.Enums;
using LitXusCount.Infrastructure.Accounts;
using LitXusCount.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LitXusCount.Tests.Accounts;

public class AccTransferServiceTests
{
    private static async Task<(AccTransferService svc, AccAccount sender, AccAccount receiver)> SetupAsync()
    {
        var ctx = TestDbContextFactory.Create();
        var sender = new AccAccount
        {
            Code = "CASH", AccountName = "Cash",
            Credit = 1000m, Debit = 0, Balance = 1000m,
            IsActive = true, CreatedAt = DateTime.UtcNow
        };
        var receiver = new AccAccount
        {
            Code = "BANK", AccountName = "Bank",
            Credit = 500m, Debit = 0, Balance = 500m,
            IsActive = true, CreatedAt = DateTime.UtcNow
        };
        ctx.AccAccounts.AddRange(sender, receiver);
        await ctx.SaveChangesAsync();
        return (new AccTransferService(ctx), sender, receiver);
    }

    // ── Balance math ──────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_DebitsSenderAndCreditsReceiver()
    {
        var (svc, sender, receiver) = await SetupAsync();
        var result = await svc.CreateAsync(new AccTransferCreateDto(sender.Id, receiver.Id, DateTime.UtcNow, 300m, null));

        Assert.True(result.Succeeded);

        await svc.Db.Entry(sender).ReloadAsync();
        await svc.Db.Entry(receiver).ReloadAsync();

        Assert.Equal(300m, sender.Debit);
        Assert.Equal(700m, sender.Balance);  // 1000 - 300
        Assert.Equal(800m, receiver.Credit); // 500 + 300
        Assert.Equal(800m, receiver.Balance);
    }

    [Fact]
    public async Task CreateAsync_WritesTwoTransactionRows()
    {
        var (svc, sender, receiver) = await SetupAsync();
        await svc.CreateAsync(new AccTransferCreateDto(sender.Id, receiver.Id, DateTime.UtcNow, 100m, null));

        var txns = await svc.Db.AccTransactions.ToListAsync();
        Assert.Equal(2, txns.Count);
        Assert.Contains(txns, t => t.Type == AccTransactionType.TransferOut && t.AccAccountId == sender.Id);
        Assert.Contains(txns, t => t.Type == AccTransactionType.TransferIn && t.AccAccountId == receiver.Id);
    }

    // ── Validation ────────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_SameAccountReturnsFailure()
    {
        var (svc, sender, _) = await SetupAsync();
        var result = await svc.CreateAsync(new AccTransferCreateDto(sender.Id, sender.Id, DateTime.UtcNow, 100m, null));

        Assert.False(result.Succeeded);
        Assert.Contains("same account", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CreateAsync_ZeroAmountReturnsFailure()
    {
        var (svc, sender, receiver) = await SetupAsync();
        var result = await svc.CreateAsync(new AccTransferCreateDto(sender.Id, receiver.Id, DateTime.UtcNow, 0m, null));

        Assert.False(result.Succeeded);
    }

    // ── Delete: reversal ──────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_ReversesBothAccountBalances()
    {
        var (svc, sender, receiver) = await SetupAsync();
        var created = await svc.CreateAsync(new AccTransferCreateDto(sender.Id, receiver.Id, DateTime.UtcNow, 400m, null));

        await svc.DeleteAsync(created.Value!.Id);

        await svc.Db.Entry(sender).ReloadAsync();
        await svc.Db.Entry(receiver).ReloadAsync();

        Assert.Equal(0m, sender.Debit);
        Assert.Equal(1000m, sender.Balance);
        Assert.Equal(500m, receiver.Credit);
        Assert.Equal(500m, receiver.Balance);
    }

    [Fact]
    public async Task DeleteAsync_AppendsFourTransactionRows()
    {
        var (svc, sender, receiver) = await SetupAsync();
        var created = await svc.CreateAsync(new AccTransferCreateDto(sender.Id, receiver.Id, DateTime.UtcNow, 100m, null));
        await svc.DeleteAsync(created.Value!.Id);

        var txns = await svc.Db.AccTransactions.ToListAsync();
        Assert.Equal(4, txns.Count); // 2 original + 2 reversals
        Assert.Contains(txns, t => t.Type == AccTransactionType.TransferOut);
        Assert.Contains(txns, t => t.Type == AccTransactionType.TransferIn);
        Assert.Contains(txns, t => t.Type == AccTransactionType.TransferReversal && t.AccAccountId == sender.Id);
        Assert.Contains(txns, t => t.Type == AccTransactionType.TransferReversal && t.AccAccountId == receiver.Id);
    }
}
