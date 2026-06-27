using LitXusCount.Application.Accounts;
using LitXusCount.Domain.Entities;
using LitXusCount.Domain.Enums;
using LitXusCount.Infrastructure.Accounts;
using LitXusCount.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LitXusCount.Tests.Accounts;

public class AccExpenseServiceTests
{
    private static async Task<(AccExpenseService svc, AccAccount account)> SetupAsync(decimal startBalance = 1000m)
    {
        var ctx = TestDbContextFactory.Create();
        var account = new AccAccount
        {
            Code = "CASH", AccountName = "Cash",
            Credit = startBalance, Debit = 0, Balance = startBalance,
            IsActive = true, CreatedAt = DateTime.UtcNow
        };
        ctx.AccAccounts.Add(account);
        await ctx.SaveChangesAsync();
        return (new AccExpenseService(ctx), account);
    }

    [Fact]
    public async Task CreateAsync_IncreaseDebitAndDecreasesBalance()
    {
        var (svc, account) = await SetupAsync(1000m);
        var result = await svc.CreateAsync(new AccExpenseCreateDto(account.Id, "Office Supplies", DateTime.UtcNow, 200m, null));

        Assert.True(result.Succeeded);

        await svc.Db.Entry(account).ReloadAsync();
        Assert.Equal(200m, account.Debit);
        Assert.Equal(800m, account.Balance);
        Assert.Equal(1000m, account.Credit); // unchanged
    }

    [Fact]
    public async Task CreateAsync_AppendsExpenseTransaction()
    {
        var (svc, account) = await SetupAsync();
        await svc.CreateAsync(new AccExpenseCreateDto(account.Id, "Fuel", DateTime.UtcNow, 150m, "petrol"));

        var tx = await svc.Db.AccTransactions.FirstOrDefaultAsync(x => x.AccAccountId == account.Id);
        Assert.NotNull(tx);
        Assert.Equal(AccTransactionType.Expense, tx.Type);
        Assert.Equal(150m, tx.Debit);
        Assert.Equal(150m, tx.Amount);
    }

    [Fact]
    public async Task CreateAsync_ZeroAmountFails() =>
        Assert.False((await (await SetupAsync()).svc.CreateAsync(
            new AccExpenseCreateDto((await SetupAsync()).account.Id, "X", DateTime.UtcNow, 0m, null))).Succeeded);

    [Fact]
    public async Task DeleteAsync_ReversesDebitAndRestoresBalance()
    {
        var (svc, account) = await SetupAsync(1000m);
        var created = await svc.CreateAsync(new AccExpenseCreateDto(account.Id, "X", DateTime.UtcNow, 300m, null));

        await svc.DeleteAsync(created.Value!.Id);

        await svc.Db.Entry(account).ReloadAsync();
        Assert.Equal(0m, account.Debit);
        Assert.Equal(1000m, account.Balance);
    }

    [Fact]
    public async Task DeleteAsync_AppendsExpenseReversalTransaction()
    {
        var (svc, account) = await SetupAsync(500m);
        var created = await svc.CreateAsync(new AccExpenseCreateDto(account.Id, "Y", DateTime.UtcNow, 100m, null));
        await svc.DeleteAsync(created.Value!.Id);

        var txns = await svc.Db.AccTransactions.Where(x => x.AccAccountId == account.Id).ToListAsync();
        Assert.Equal(2, txns.Count);
        Assert.Contains(txns, t => t.Type == AccTransactionType.Expense);
        Assert.Contains(txns, t => t.Type == AccTransactionType.ExpenseReversal);
    }
}
