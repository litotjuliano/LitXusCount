using LitXusCount.Application.Accounts;
using LitXusCount.Application.Common;
using LitXusCount.Domain.Entities;
using LitXusCount.Infrastructure.Accounts;
using LitXusCount.Tests.Helpers;
using Xunit;

namespace LitXusCount.Tests.Accounts;

public class AccAccountServiceTests
{
    private static AccAccountService BuildService() =>
        new(TestDbContextFactory.Create());

    // ── Create ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_SetsBalanceToZero()
    {
        var svc = BuildService();
        var result = await svc.CreateAsync(new AccAccountUpsertDto("CASH", "Cash", null, null));

        Assert.True(result.Succeeded);
        Assert.Equal(0m, result.Value!.Balance);
        Assert.Equal(0m, result.Value.Credit);
        Assert.Equal(0m, result.Value.Debit);
    }

    [Fact]
    public async Task CreateAsync_DuplicateCodeReturnsFailure()
    {
        var svc = BuildService();
        await svc.CreateAsync(new AccAccountUpsertDto("CASH", "Cash", null, null));
        var result = await svc.CreateAsync(new AccAccountUpsertDto("CASH", "Cash 2", null, null));

        Assert.False(result.Succeeded);
        Assert.Contains("already in use", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CreateAsync_SoftDeletedCodeCanBeReused()
    {
        var svc = BuildService();
        var created = await svc.CreateAsync(new AccAccountUpsertDto("CASH", "Cash", null, null));
        await svc.DeleteAsync(created.Value!.Id);

        var reused = await svc.CreateAsync(new AccAccountUpsertDto("CASH", "Cash New", null, null));
        Assert.True(reused.Succeeded);
    }

    // ── Edit ─────────────────────────────────────────────────────────────────

    [Fact]
    public async Task EditAsync_DoesNotChangeBalanceColumns()
    {
        var ctx = TestDbContextFactory.Create();
        // Seed an account with a non-zero balance to confirm edit cannot change it
        var account = new AccAccount
        {
            Code = "BANK", AccountName = "Bank", Credit = 500m, Debit = 200m, Balance = 300m,
            IsActive = true, CreatedAt = DateTime.UtcNow
        };
        ctx.AccAccounts.Add(account);
        await ctx.SaveChangesAsync();

        var svc = new AccAccountService(ctx);
        var result = await svc.EditAsync(account.Id, new AccAccountUpsertDto("BANK", "Bank Updated", "001", "desc"));

        Assert.True(result.Succeeded);
        Assert.Equal(500m, result.Value!.Credit);
        Assert.Equal(200m, result.Value.Debit);
        Assert.Equal(300m, result.Value.Balance);
        Assert.Equal("Bank Updated", result.Value.AccountName);
    }

    // ── Delete ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_SoftDeletesAccount()
    {
        var svc = BuildService();
        var created = await svc.CreateAsync(new AccAccountUpsertDto("X", "X Account", null, null));

        var deleted = await svc.DeleteAsync(created.Value!.Id);
        Assert.True(deleted);

        var list = await svc.ListAsync(new PagedQuery());
        Assert.DoesNotContain(list.Items, x => x.Id == created.Value.Id);
    }

    // ── AllActive ────────────────────────────────────────────────────────────

    [Fact]
    public async Task ListAllActiveAsync_ExcludesInactiveAccounts()
    {
        var svc = BuildService();
        var a = await svc.CreateAsync(new AccAccountUpsertDto("A1", "Active", null, null));
        var b = await svc.CreateAsync(new AccAccountUpsertDto("A2", "To Delete", null, null));
        await svc.DeleteAsync(b.Value!.Id);

        var allActive = await svc.ListAllActiveAsync();
        Assert.Single(allActive);
        Assert.Equal(a.Value!.Id, allActive[0].Id);
    }

    // ── Permissions discovery ────────────────────────────────────────────────

    [Fact]
    public void PermissionsAll_ContainsAccountsPermissions()
    {
        var all = LitXusCount.Application.Authorization.Permissions.All;
        Assert.Contains("Accounts.Account.View", all);
        Assert.Contains("Accounts.Account.Create", all);
        Assert.Contains("Accounts.Deposit.View", all);
        Assert.Contains("Accounts.Expense.Create", all);
        Assert.Contains("Accounts.Transfer.Delete", all);
    }
}
