using LitXusCount.Application.Common;
using LitXusCount.Application.Settings.GlAccounts;
using LitXusCount.Domain.Entities;
using LitXusCount.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LitXusCount.Infrastructure.Settings.GlAccounts;

internal sealed class GlAccountService(ApplicationDbContext db) : IGlAccountService
{
    private static readonly string[] SortableColumns = ["name", "code", "type"];

    public async Task<PagedResult<GlAccountDto>> ListAsync(PagedQuery query, CancellationToken ct = default)
    {
        var filtered = db.GlAccounts.Where(x => x.IsActive);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var pattern = $"%{query.Search.Trim()}%";
            filtered = filtered.Where(x =>
                EF.Functions.Like(x.Name, pattern) ||
                EF.Functions.Like(x.Code, pattern));
        }

        var totalCount = await filtered.CountAsync(ct);

        var sortBy = SortableColumns.Contains(query.SortBy?.ToLowerInvariant()) ? query.SortBy!.ToLowerInvariant() : "code";
        filtered = (sortBy, query.SortDescending) switch
        {
            ("name", true) => filtered.OrderByDescending(x => x.Name),
            ("name", false) => filtered.OrderBy(x => x.Name),
            ("type", true) => filtered.OrderByDescending(x => x.AccountType),
            ("type", false) => filtered.OrderBy(x => x.AccountType),
            (_, true) => filtered.OrderByDescending(x => x.Code),
            (_, false) => filtered.OrderBy(x => x.Code),
        };

        var items = await filtered
            .Skip((query.EffectivePage - 1) * query.EffectivePageSize)
            .Take(query.EffectivePageSize)
            .Select(x => new GlAccountDto(
                x.Id,
                x.Code,
                x.Name,
                x.AccountType,
                x.ParentId,
                x.Parent != null ? x.Parent.Name : null,
                x.IsControl,
                x.OpeningBalance,
                x.IsActive))
            .ToListAsync(ct);

        return new PagedResult<GlAccountDto>(items, totalCount, query.EffectivePage, query.EffectivePageSize);
    }

    public async Task<IReadOnlyList<GlAccountDto>> ListAllActiveAsync(CancellationToken ct = default) =>
        await db.GlAccounts
            .Where(x => x.IsActive)
            .OrderBy(x => x.Code)
            .Take(200)
            .Select(x => new GlAccountDto(
                x.Id,
                x.Code,
                x.Name,
                x.AccountType,
                x.ParentId,
                x.Parent != null ? x.Parent.Name : null,
                x.IsControl,
                x.OpeningBalance,
                x.IsActive))
            .ToListAsync(ct);

    public async Task<GlAccountDto?> GetAsync(long id, CancellationToken ct = default)
    {
        var entity = await db.GlAccounts
            .Include(x => x.Parent)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
        return entity is null ? null : ToDto(entity);
    }

    public async Task<ServiceResult<GlAccountDto>> CreateAsync(GlAccountUpsertDto request, CancellationToken ct = default)
    {
        if (await CodeInUseAsync(request.Code, excludeId: null, ct))
        {
            return ServiceResult<GlAccountDto>.Failure($"GlAccount code '{request.Code}' is already in use.");
        }

        var entity = new GlAccount
        {
            Code = request.Code,
            Name = request.Name,
            AccountType = request.AccountType,
            ParentId = request.ParentId,
            IsControl = request.IsControl,
            OpeningBalance = request.OpeningBalance,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
        };

        db.GlAccounts.Add(entity);
        await db.SaveChangesAsync(ct);

        return ServiceResult<GlAccountDto>.Success(ToDto(entity));
    }

    public async Task<ServiceResult<GlAccountDto>> EditAsync(long id, GlAccountUpsertDto request, CancellationToken ct = default)
    {
        var entity = await db.GlAccounts.FirstOrDefaultAsync(x => x.Id == id && x.IsActive, ct);
        if (entity is null)
        {
            return ServiceResult<GlAccountDto>.Failure("Not found.");
        }

        if (await CodeInUseAsync(request.Code, excludeId: id, ct))
        {
            return ServiceResult<GlAccountDto>.Failure($"GlAccount code '{request.Code}' is already in use.");
        }

        entity.Code = request.Code;
        entity.Name = request.Name;
        entity.AccountType = request.AccountType;
        entity.ParentId = request.ParentId;
        entity.IsControl = request.IsControl;
        entity.OpeningBalance = request.OpeningBalance;
        entity.ModifiedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);

        return ServiceResult<GlAccountDto>.Success(ToDto(entity));
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
    {
        var entity = await db.GlAccounts.FirstOrDefaultAsync(x => x.Id == id && x.IsActive, ct);
        if (entity is null)
        {
            return false;
        }

        entity.IsActive = false;
        entity.ModifiedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        return true;
    }

    private Task<bool> CodeInUseAsync(string code, long? excludeId, CancellationToken ct)
    {
        var normalized = code.Trim().ToLower();
        return db.GlAccounts.AnyAsync(x => x.IsActive && x.Code.ToLower() == normalized && (excludeId == null || x.Id != excludeId), ct);
    }

    private static GlAccountDto ToDto(GlAccount entity) =>
        new(
            entity.Id,
            entity.Code,
            entity.Name,
            entity.AccountType,
            entity.ParentId,
            entity.Parent != null ? entity.Parent.Name : null,
            entity.IsControl,
            entity.OpeningBalance,
            entity.IsActive);
}
