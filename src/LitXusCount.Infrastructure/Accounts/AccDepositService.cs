using LitXusCount.Application.Accounts;
using LitXusCount.Application.Common;
using LitXusCount.Domain.Entities;
using LitXusCount.Domain.Enums;
using LitXusCount.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LitXusCount.Infrastructure.Accounts;

internal sealed class AccDepositService(ApplicationDbContext db) : IAccDepositService
{
    internal ApplicationDbContext Db => db;

    public async Task<PagedResult<AccDepositDto>> ListAsync(PagedQuery query, long? accountId = null, CancellationToken ct = default)
    {
        var filtered = db.AccDeposits.Include(x => x.AccAccount).Where(x => x.IsActive);

        if (accountId.HasValue)
            filtered = filtered.Where(x => x.AccAccountId == accountId.Value);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var pattern = $"%{query.Search.Trim()}%";
            filtered = filtered.Where(x => EF.Functions.Like(x.AccAccount.AccountName, pattern));
        }

        var totalCount = await filtered.CountAsync(ct);
        var items = await filtered
            .OrderByDescending(x => x.DepositDate)
            .Skip((query.EffectivePage - 1) * query.EffectivePageSize)
            .Take(query.EffectivePageSize)
            .Select(x => ToDto(x))
            .ToListAsync(ct);

        return new PagedResult<AccDepositDto>(items, totalCount, query.EffectivePage, query.EffectivePageSize);
    }

    public async Task<AccDepositDto?> GetAsync(long id, CancellationToken ct = default)
    {
        var entity = await db.AccDeposits.Include(x => x.AccAccount)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
        return entity is null ? null : ToDto(entity);
    }

    public async Task<ServiceResult<AccDepositDto>> CreateAsync(AccDepositCreateDto request, CancellationToken ct = default)
    {
        if (request.Amount <= 0)
            return ServiceResult<AccDepositDto>.Failure("Amount must be greater than zero.");

        var account = await db.AccAccounts.FirstOrDefaultAsync(x => x.Id == request.AccAccountId && x.IsActive, ct);
        if (account is null)
            return ServiceResult<AccDepositDto>.Failure("Account not found or inactive.");

        var deposit = new AccDeposit
        {
            AccAccountId = request.AccAccountId,
            DepositDate = request.DepositDate,
            Amount = request.Amount,
            Note = request.Note?.Trim(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
        };
        db.AccDeposits.Add(deposit);

        account.Credit += request.Amount;
        account.Balance += request.Amount;
        account.ModifiedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);

        db.AccTransactions.Add(new AccTransaction
        {
            AccAccountId = request.AccAccountId,
            Type = AccTransactionType.Deposit,
            Credit = request.Amount,
            Amount = request.Amount,
            Reference = $"DEP-{deposit.Id}",
            CreatedAt = DateTime.UtcNow,
        });
        await db.SaveChangesAsync(ct);

        deposit.AccAccount = account;
        return ServiceResult<AccDepositDto>.Success(ToDto(deposit));
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
    {
        var deposit = await db.AccDeposits.Include(x => x.AccAccount)
            .FirstOrDefaultAsync(x => x.Id == id && x.IsActive, ct);
        if (deposit is null) return false;

        var account = deposit.AccAccount;

        deposit.IsActive = false;
        deposit.ModifiedAt = DateTime.UtcNow;

        account.Credit -= deposit.Amount;
        account.Balance -= deposit.Amount;
        account.ModifiedAt = DateTime.UtcNow;

        db.AccTransactions.Add(new AccTransaction
        {
            AccAccountId = deposit.AccAccountId,
            Type = AccTransactionType.DepositReversal,
            Debit = deposit.Amount,
            Amount = deposit.Amount,
            Reference = $"DEP-{deposit.Id}-REV",
            CreatedAt = DateTime.UtcNow,
        });

        await db.SaveChangesAsync(ct);
        return true;
    }

    private static AccDepositDto ToDto(AccDeposit x) =>
        new(x.Id, x.AccAccountId, x.AccAccount.AccountName, x.DepositDate, x.Amount, x.Note, x.IsActive);
}
