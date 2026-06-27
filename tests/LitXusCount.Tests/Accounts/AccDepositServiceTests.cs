using LitXusCount.Application.Accounts;
using LitXusCount.Domain.Entities;
using LitXusCount.Domain.Enums;
using LitXusCount.Infrastructure.Accounts;
using LitXusCount.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LitXusCount.Tests.Accounts;

public class AccDepositServiceTests
{
    private static async Task<(AccDepositService svc, long accountId)> SetupAsync()
    {
        var ctx = TestDbContextFactory.Create();
        var account = new AccAccount
        {
            Code = "CASH", AccountName = "Cash", Credit = 0, Debit = 0, Balance = 0,
            IsActive = true, CreatedAt = DateTime.UtcNow
        };
        ctx.AccAccounts.Add(account);
        await ctx.SaveChangesAsync();
        return (new AccDepositService(ctx), account.Id);
    }

    // ── Create: balance math ──────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_IncreasesCreditAndBalance()
    {
        var (svc, accountId) = await SetupAsync();
        var result = await svc.CreateAsync(new AccDepositCreateDto(accountId, DateTime.UtcNow, 500m, null));

        Assert.True(result.Succeeded);

        // Reload account from same context
        var ctx = TestDbContextFactory.Create();
        // We need to verify via the service's embedded context
        // Re-fetch from service's own GetAsync
        var dto = await svc.GetAsync(result.Value!.Id);
        Assert.NotNull(dto);

        // Verify via AccAccount
        var accountDto = await new AccAccountService(svc.Db).GetAsync(accountId);
        Assert.Equal(500m, accountDto!.Credit);
        Assert.Equal(0m, accountDto.Debit);
        Assert.Equal(500m, accountDto.Balance);
    }

    [Fact]
    public async Task CreateAsync_AppendsDepositTransaction()
    {
        var ctx = TestDbContextFactory.Create();
        var account = new AccAccount { Code = "C", AccountName = "Cash", IsActive = true, CreatedAt = DateTime.UtcNow };
        ctx.AccAccounts.Add(account);
        await ctx.SaveChangesAsync();

        var svc = new AccDepositService(ctx);
        await svc.CreateAsync(new AccDepositCreateDto(account.Id, DateTime.UtcNow, 300m, null));

        var tx = await ctx.AccTransactions.FirstOrDefaultAsync(x => x.AccAccountId == account.Id);
        Assert.NotNull(tx);
        Assert.Equal(AccTransactionType.Deposit, tx.Type);
        Assert.Equal(300m, tx.Credit);
        Assert.Equal(300m, tx.Amount);
    }

    [Fact]
    public async Task CreateAsync_ZeroAmountReturnsFailure()
    {
        var (svc, accountId) = await SetupAsync();
        var result = await svc.CreateAsync(new AccDepositCreateDto(accountId, DateTime.UtcNow, 0m, null));

        Assert.False(result.Succeeded);
    }

    [Fact]
    public async Task CreateAsync_NegativeAmountReturnsFailure()
    {
        var (svc, accountId) = await SetupAsync();
        var result = await svc.CreateAsync(new AccDepositCreateDto(accountId, DateTime.UtcNow, -100m, null));

        Assert.False(result.Succeeded);
    }

    [Fact]
    public async Task CreateAsync_InactiveAccountReturnsFailure()
    {
        var ctx = TestDbContextFactory.Create();
        var account = new AccAccount { Code = "DEL", AccountName = "Deleted", IsActive = false, CreatedAt = DateTime.UtcNow };
        ctx.AccAccounts.Add(account);
        await ctx.SaveChangesAsync();

        var svc = new AccDepositService(ctx);
        var result = await svc.CreateAsync(new AccDepositCreateDto(account.Id, DateTime.UtcNow, 100m, null));
        Assert.False(result.Succeeded);
    }

    // ── Delete: reversal ──────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_ReversesCreditAndBalance()
    {
        var ctx = TestDbContextFactory.Create();
        var account = new AccAccount { Code = "C", AccountName = "Cash", IsActive = true, CreatedAt = DateTime.UtcNow };
        ctx.AccAccounts.Add(account);
        await ctx.SaveChangesAsync();

        var svc = new AccDepositService(ctx);
        var created = await svc.CreateAsync(new AccDepositCreateDto(account.Id, DateTime.UtcNow, 500m, null));

        var deleted = await svc.DeleteAsync(created.Value!.Id);
        Assert.True(deleted);

        await ctx.Entry(account).ReloadAsync();
        Assert.Equal(0m, account.Credit);
        Assert.Equal(0m, account.Balance);
    }

    [Fact]
    public async Task DeleteAsync_OriginalTransactionPreserved_ReversalAppended()
    {
        var ctx = TestDbContextFactory.Create();
        var account = new AccAccount { Code = "C", AccountName = "Cash", IsActive = true, CreatedAt = DateTime.UtcNow };
        ctx.AccAccounts.Add(account);
        await ctx.SaveChangesAsync();

        var svc = new AccDepositService(ctx);
        var created = await svc.CreateAsync(new AccDepositCreateDto(account.Id, DateTime.UtcNow, 500m, null));
        await svc.DeleteAsync(created.Value!.Id);

        var txns = await ctx.AccTransactions.Where(x => x.AccAccountId == account.Id).ToListAsync();
        Assert.Equal(2, txns.Count);
        Assert.Contains(txns, t => t.Type == AccTransactionType.Deposit);
        Assert.Contains(txns, t => t.Type == AccTransactionType.DepositReversal);
    }
}
