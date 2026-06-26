using LitXusCount.Application.Common;
using LitXusCount.Domain.Enums;

namespace LitXusCount.Application.Accounts;

public interface IAccAccountService
{
    Task<PagedResult<AccAccountDto>> ListAsync(PagedQuery query, CancellationToken ct = default);
    Task<IReadOnlyList<AccAccountDto>> ListAllActiveAsync(CancellationToken ct = default);
    Task<AccAccountDto?> GetAsync(long id, CancellationToken ct = default);
    Task<ServiceResult<AccAccountDto>> CreateAsync(AccAccountUpsertDto request, CancellationToken ct = default);
    Task<ServiceResult<AccAccountDto>> EditAsync(long id, AccAccountUpsertDto request, CancellationToken ct = default);
    Task<bool> DeleteAsync(long id, CancellationToken ct = default);

    /// <summary>
    /// Updates the account balance and appends an AccTransaction row.
    /// Does NOT call SaveChangesAsync — the caller commits the surrounding transaction.
    /// Used by Phase 4 (Sales) and Phase 5 (Purchasing) payment recording.
    /// </summary>
    Task UpdateBalanceAsync(long accountId, decimal amount, AccTransactionType type, string? reference, CancellationToken ct = default);
}
