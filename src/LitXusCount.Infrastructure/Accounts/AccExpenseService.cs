using LitXusCount.Application.Accounts;
using LitXusCount.Application.Common;
using LitXusCount.Domain.Entities;
using LitXusCount.Domain.Enums;
using LitXusCount.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LitXusCount.Infrastructure.Accounts;

internal sealed class AccExpenseService(ApplicationDbContext db) : IAccExpenseService
{
    internal ApplicationDbContext Db => db;

    public async Task<PagedResult<AccExpenseDto>> ListAsync(PagedQuery query, long? accountId = null, CancellationToken ct = default)
    {
        var filtered = db.AccExpenses.Include(x => x.AccAccount).Where(x => x.IsActive);

        if (accountId.HasValue)
            filtered = filtered.Where(x => x.AccAccountId == accountId.Value);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var pattern = $"%{query.Search.Trim()}%";
            filtered = filtered.Where(x =>
                EF.Functions.Like(x.Name, pattern) ||
                EF.Functions.Like(x.AccAccount.AccountName, pattern));
        }

        var totalCount = await filtered.CountAsync(ct);
        var items = await filtered
            .OrderByDescending(x => x.ExpenseDate)
            .Skip((query.EffectivePage - 1) * query.EffectivePageSize)
            .Take(query.EffectivePageSize)
            .Select(x => ToDto(x))
            .ToListAsync(ct);

        return new PagedResult<AccExpenseDto>(items, totalCount, query.EffectivePage, query.EffectivePageSize);
    }

    public async Task<AccExpenseDto?> GetAsync(long id, CancellationToken ct = default)
    {
        var entity = await db.AccExpenses.Include(x => x.AccAccount)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
        return entity is null ? null : ToDto(entity);
    }

    public async Task<ServiceResult<AccExpenseDto>> CreateAsync(AccExpenseCreateDto request, CancellationToken ct = default)
    {
        if (request.Amount <= 0)
            return ServiceResult<AccExpenseDto>.Failure("Amount must be greater than zero.");

        var account = await db.AccAccounts.FirstOrDefaultAsync(x => x.Id == request.AccAccountId && x.IsActive, ct);
        if (account is null)
            return ServiceResult<AccExpenseDto>.Failure("Account not found or inactive.");

        var expense = new AccExpense
        {
            AccAccountId = request.AccAccountId,
            Name = request.Name.Trim(),
            ExpenseDate = request.ExpenseDate,
            Amount = request.Amount,
            Note = request.Note?.Trim(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
        };
        db.AccExpenses.Add(expense);

        account.Debit += request.Amount;
        account.Balance -= request.Amount;
        account.ModifiedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);

        db.AccTransactions.Add(new AccTransaction
        {
            AccAccountId = request.AccAccountId,
            Type = AccTransactionType.Expense,
            Debit = request.Amount,
            Amount = request.Amount,
            Reference = $"EXP-{expense.Id}",
            CreatedAt = DateTime.UtcNow,
        });
        await db.SaveChangesAsync(ct);

        expense.AccAccount = account;
        return ServiceResult<AccExpenseDto>.Success(ToDto(expense));
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
    {
        var expense = await db.AccExpenses.Include(x => x.AccAccount)
            .FirstOrDefaultAsync(x => x.Id == id && x.IsActive, ct);
        if (expense is null) return false;

        var account = expense.AccAccount;

        expense.IsActive = false;
        expense.ModifiedAt = DateTime.UtcNow;

        account.Debit -= expense.Amount;
        account.Balance += expense.Amount;
        account.ModifiedAt = DateTime.UtcNow;

        db.AccTransactions.Add(new AccTransaction
        {
            AccAccountId = expense.AccAccountId,
            Type = AccTransactionType.ExpenseReversal,
            Credit = expense.Amount,
            Amount = expense.Amount,
            Reference = $"EXP-{expense.Id}-REV",
            CreatedAt = DateTime.UtcNow,
        });

        await db.SaveChangesAsync(ct);
        return true;
    }

    private static AccExpenseDto ToDto(AccExpense x) =>
        new(x.Id, x.AccAccountId, x.AccAccount.AccountName, x.Name, x.ExpenseDate, x.Amount, x.Note, x.IsActive);
}
