using LitXusCount.Application.Accounts;
using LitXusCount.Application.Common;
using LitXusCount.Domain.Entities;
using LitXusCount.Domain.Enums;
using LitXusCount.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LitXusCount.Infrastructure.Accounts;

internal sealed class AccTransferService(ApplicationDbContext db) : IAccTransferService
{
    internal ApplicationDbContext Db => db;

    public async Task<PagedResult<AccTransferDto>> ListAsync(PagedQuery query, CancellationToken ct = default)
    {
        var filtered = db.AccTransfers
            .Include(x => x.SenderAccount)
            .Include(x => x.ReceiverAccount)
            .Where(x => x.IsActive);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var pattern = $"%{query.Search.Trim()}%";
            filtered = filtered.Where(x =>
                EF.Functions.Like(x.SenderAccount.AccountName, pattern) ||
                EF.Functions.Like(x.ReceiverAccount.AccountName, pattern));
        }

        var totalCount = await filtered.CountAsync(ct);
        var items = await filtered
            .OrderByDescending(x => x.TransferDate)
            .Skip((query.EffectivePage - 1) * query.EffectivePageSize)
            .Take(query.EffectivePageSize)
            .Select(x => ToDto(x))
            .ToListAsync(ct);

        return new PagedResult<AccTransferDto>(items, totalCount, query.EffectivePage, query.EffectivePageSize);
    }

    public async Task<AccTransferDto?> GetAsync(long id, CancellationToken ct = default)
    {
        var entity = await db.AccTransfers
            .Include(x => x.SenderAccount)
            .Include(x => x.ReceiverAccount)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
        return entity is null ? null : ToDto(entity);
    }

    public async Task<ServiceResult<AccTransferDto>> CreateAsync(AccTransferCreateDto request, CancellationToken ct = default)
    {
        if (request.SenderAccountId == request.ReceiverAccountId)
            return ServiceResult<AccTransferDto>.Failure("Cannot transfer to the same account.");

        if (request.Amount <= 0)
            return ServiceResult<AccTransferDto>.Failure("Amount must be greater than zero.");

        var sender = await db.AccAccounts.FirstOrDefaultAsync(x => x.Id == request.SenderAccountId && x.IsActive, ct);
        if (sender is null)
            return ServiceResult<AccTransferDto>.Failure("Sender account not found or inactive.");

        var receiver = await db.AccAccounts.FirstOrDefaultAsync(x => x.Id == request.ReceiverAccountId && x.IsActive, ct);
        if (receiver is null)
            return ServiceResult<AccTransferDto>.Failure("Receiver account not found or inactive.");

        var transfer = new AccTransfer
        {
            SenderAccountId = request.SenderAccountId,
            ReceiverAccountId = request.ReceiverAccountId,
            TransferDate = request.TransferDate,
            Amount = request.Amount,
            Note = request.Note?.Trim(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
        };
        db.AccTransfers.Add(transfer);

        sender.Debit += request.Amount;
        sender.Balance -= request.Amount;
        sender.ModifiedAt = DateTime.UtcNow;

        receiver.Credit += request.Amount;
        receiver.Balance += request.Amount;
        receiver.ModifiedAt = DateTime.UtcNow;

        db.AccTransactions.Add(new AccTransaction
        {
            AccAccountId = request.SenderAccountId,
            Type = AccTransactionType.TransferOut,
            Debit = request.Amount,
            Amount = request.Amount,
            CreatedAt = DateTime.UtcNow,
        });

        db.AccTransactions.Add(new AccTransaction
        {
            AccAccountId = request.ReceiverAccountId,
            Type = AccTransactionType.TransferIn,
            Credit = request.Amount,
            Amount = request.Amount,
            CreatedAt = DateTime.UtcNow,
        });

        await db.SaveChangesAsync(ct);

        transfer.SenderAccount = sender;
        transfer.ReceiverAccount = receiver;
        return ServiceResult<AccTransferDto>.Success(ToDto(transfer));
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
    {
        var transfer = await db.AccTransfers
            .Include(x => x.SenderAccount)
            .Include(x => x.ReceiverAccount)
            .FirstOrDefaultAsync(x => x.Id == id && x.IsActive, ct);
        if (transfer is null) return false;

        transfer.IsActive = false;
        transfer.ModifiedAt = DateTime.UtcNow;

        var sender = transfer.SenderAccount;
        var receiver = transfer.ReceiverAccount;

        sender.Debit -= transfer.Amount;
        sender.Balance += transfer.Amount;
        sender.ModifiedAt = DateTime.UtcNow;

        receiver.Credit -= transfer.Amount;
        receiver.Balance -= transfer.Amount;
        receiver.ModifiedAt = DateTime.UtcNow;

        db.AccTransactions.Add(new AccTransaction
        {
            AccAccountId = transfer.SenderAccountId,
            Type = AccTransactionType.TransferReversal,
            Credit = transfer.Amount,
            Amount = transfer.Amount,
            Reference = $"TRF-{transfer.Id}-REV",
            CreatedAt = DateTime.UtcNow,
        });

        db.AccTransactions.Add(new AccTransaction
        {
            AccAccountId = transfer.ReceiverAccountId,
            Type = AccTransactionType.TransferReversal,
            Debit = transfer.Amount,
            Amount = transfer.Amount,
            Reference = $"TRF-{transfer.Id}-REV",
            CreatedAt = DateTime.UtcNow,
        });

        await db.SaveChangesAsync(ct);
        return true;
    }

    private static AccTransferDto ToDto(AccTransfer x) =>
        new(x.Id, x.SenderAccountId, x.SenderAccount.AccountName,
            x.ReceiverAccountId, x.ReceiverAccount.AccountName,
            x.TransferDate, x.Amount, x.Note, x.IsActive);
}
