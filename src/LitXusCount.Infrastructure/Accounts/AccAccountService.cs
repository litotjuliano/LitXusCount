using LitXusCount.Application.Accounts;
using LitXusCount.Application.Common;
using LitXusCount.Domain.Entities;
using LitXusCount.Domain.Enums;
using LitXusCount.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LitXusCount.Infrastructure.Accounts;

internal sealed class AccAccountService(ApplicationDbContext db) : IAccAccountService
{
    // Expose db for test helpers
    internal ApplicationDbContext Db => db;

    private static readonly string[] SortableColumns = ["code", "accountname", "balance"];

    public async Task<PagedResult<AccAccountDto>> ListAsync(PagedQuery query, CancellationToken ct = default)
    {
        var filtered = db.AccAccounts.Where(x => x.IsActive);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var pattern = $"%{query.Search.Trim()}%";
            filtered = filtered.Where(x =>
                EF.Functions.Like(x.AccountName, pattern) ||
                EF.Functions.Like(x.Code, pattern));
        }

        var totalCount = await filtered.CountAsync(ct);

        var sortBy = SortableColumns.Contains(query.SortBy?.ToLowerInvariant()) ? query.SortBy!.ToLowerInvariant() : "accountname";
        filtered = (sortBy, query.SortDescending) switch
        {
            ("code", true) => filtered.OrderByDescending(x => x.Code),
            ("code", false) => filtered.OrderBy(x => x.Code),
            ("balance", true) => filtered.OrderByDescending(x => x.Balance),
            ("balance", false) => filtered.OrderBy(x => x.Balance),
            (_, true) => filtered.OrderByDescending(x => x.AccountName),
            _ => filtered.OrderBy(x => x.AccountName),
        };

        var items = await filtered
            .Skip((query.EffectivePage - 1) * query.EffectivePageSize)
            .Take(query.EffectivePageSize)
            .Select(x => ToDto(x))
            .ToListAsync(ct);

        return new PagedResult<AccAccountDto>(items, totalCount, query.EffectivePage, query.EffectivePageSize);
    }

    public async Task<IReadOnlyList<AccAccountDto>> ListAllActiveAsync(CancellationToken ct = default) =>
        await db.AccAccounts
            .Where(x => x.IsActive)
            .OrderBy(x => x.AccountName)
            .Take(200)
            .Select(x => ToDto(x))
            .ToListAsync(ct);

    public async Task<AccAccountDto?> GetAsync(long id, CancellationToken ct = default)
    {
        var entity = await db.AccAccounts.FirstOrDefaultAsync(x => x.Id == id, ct);
        return entity is null ? null : ToDto(entity);
    }

    public async Task<ServiceResult<AccAccountDto>> CreateAsync(AccAccountUpsertDto request, CancellationToken ct = default)
    {
        if (await CodeInUseAsync(request.Code, null, ct))
            return ServiceResult<AccAccountDto>.Failure($"Account code '{request.Code}' is already in use.");

        var entity = new AccAccount
        {
            Code = request.Code.Trim(),
            AccountName = request.AccountName.Trim(),
            AccountNumber = request.AccountNumber?.Trim(),
            Description = request.Description?.Trim(),
            Credit = 0, Debit = 0, Balance = 0,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
        };

        db.AccAccounts.Add(entity);
        await db.SaveChangesAsync(ct);
        return ServiceResult<AccAccountDto>.Success(ToDto(entity));
    }

    public async Task<ServiceResult<AccAccountDto>> EditAsync(long id, AccAccountUpsertDto request, CancellationToken ct = default)
    {
        var entity = await db.AccAccounts.FirstOrDefaultAsync(x => x.Id == id && x.IsActive, ct);
        if (entity is null) return ServiceResult<AccAccountDto>.Failure("Account not found.");

        if (await CodeInUseAsync(request.Code, id, ct))
            return ServiceResult<AccAccountDto>.Failure($"Account code '{request.Code}' is already in use.");

        entity.Code = request.Code.Trim();
        entity.AccountName = request.AccountName.Trim();
        entity.AccountNumber = request.AccountNumber?.Trim();
        entity.Description = request.Description?.Trim();
        entity.ModifiedAt = DateTime.UtcNow;
        // Credit, Debit, Balance intentionally NOT updated here

        await db.SaveChangesAsync(ct);
        return ServiceResult<AccAccountDto>.Success(ToDto(entity));
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
    {
        var entity = await db.AccAccounts.FirstOrDefaultAsync(x => x.Id == id && x.IsActive, ct);
        if (entity is null) return false;

        entity.IsActive = false;
        entity.ModifiedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task UpdateBalanceAsync(long accountId, decimal amount, AccTransactionType type, string? reference, CancellationToken ct = default)
    {
        var account = await db.AccAccounts.FirstOrDefaultAsync(x => x.Id == accountId && x.IsActive, ct)
            ?? throw new InvalidOperationException($"AccAccount {accountId} not found or inactive.");

        decimal credit = 0, debit = 0;

        switch (type)
        {
            case AccTransactionType.SalesPayment:
                account.Credit += amount;
                account.Balance += amount;
                credit = amount;
                break;
            case AccTransactionType.SalesPaymentReversal:
                account.Credit -= amount;
                account.Balance -= amount;
                credit = -amount;
                break;
            case AccTransactionType.PurchasePayment:
                account.Debit += amount;
                account.Balance -= amount;
                debit = amount;
                break;
            case AccTransactionType.PurchasePaymentReversal:
                account.Debit -= amount;
                account.Balance += amount;
                debit = -amount;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, "Unsupported type for UpdateBalanceAsync.");
        }

        account.ModifiedAt = DateTime.UtcNow;

        db.AccTransactions.Add(new AccTransaction
        {
            AccAccountId = accountId,
            Type = type,
            Credit = credit,
            Debit = debit,
            Amount = amount,
            Reference = reference,
            CreatedAt = DateTime.UtcNow,
        });
        // Caller calls SaveChangesAsync
    }

    private Task<bool> CodeInUseAsync(string code, long? excludeId, CancellationToken ct)
    {
        var normalized = code.Trim().ToLower();
        return db.AccAccounts.AnyAsync(x =>
            x.IsActive &&
            x.Code.ToLower() == normalized &&
            (excludeId == null || x.Id != excludeId), ct);
    }

    private static AccAccountDto ToDto(AccAccount x) =>
        new(x.Id, x.Code, x.AccountName, x.AccountNumber, x.Description, x.Credit, x.Debit, x.Balance, x.IsActive);
}
