using LitXusCount.Application.Common;
using LitXusCount.Application.Settings.Currencies;
using LitXusCount.Domain.Entities;
using LitXusCount.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LitXusCount.Infrastructure.Settings.Currencies;

internal sealed class CurrencyService(ApplicationDbContext db) : ICurrencyService
{
    private static readonly string[] SortableColumns = ["name", "code", "country"];

    public async Task<PagedResult<CurrencyDto>> ListAsync(PagedQuery query, CancellationToken ct = default)
    {
        var filtered = db.Currencies.Where(x => x.IsActive);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var pattern = $"%{query.Search.Trim()}%";
            filtered = filtered.Where(x =>
                EF.Functions.Like(x.Name, pattern) ||
                EF.Functions.Like(x.Code, pattern) ||
                EF.Functions.Like(x.Country ?? "", pattern));
        }

        var totalCount = await filtered.CountAsync(ct);

        var sortBy = SortableColumns.Contains(query.SortBy?.ToLowerInvariant()) ? query.SortBy!.ToLowerInvariant() : "name";
        filtered = (sortBy, query.SortDescending) switch
        {
            ("code", true) => filtered.OrderByDescending(x => x.Code),
            ("code", false) => filtered.OrderBy(x => x.Code),
            ("country", true) => filtered.OrderByDescending(x => x.Country),
            ("country", false) => filtered.OrderBy(x => x.Country),
            (_, true) => filtered.OrderByDescending(x => x.Name),
            (_, false) => filtered.OrderBy(x => x.Name),
        };

        var items = await filtered
            .Skip((query.EffectivePage - 1) * query.EffectivePageSize)
            .Take(query.EffectivePageSize)
            .Select(x => new CurrencyDto(x.Id, x.Name, x.Code, x.Symbol, x.Country, x.Description, x.IsDefault, x.IsActive))
            .ToListAsync(ct);

        return new PagedResult<CurrencyDto>(items, totalCount, query.EffectivePage, query.EffectivePageSize);
    }

    public async Task<IReadOnlyList<CurrencyDto>> ListAllActiveAsync(CancellationToken ct = default) =>
        await db.Currencies
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .Take(100)
            .Select(x => new CurrencyDto(x.Id, x.Name, x.Code, x.Symbol, x.Country, x.Description, x.IsDefault, x.IsActive))
            .ToListAsync(ct);

    public async Task<CurrencyDto?> GetAsync(long id, CancellationToken ct = default)
    {
        var entity = await db.Currencies.FirstOrDefaultAsync(x => x.Id == id, ct);
        return entity is null ? null : ToDto(entity);
    }

    public async Task<ServiceResult<CurrencyDto>> CreateAsync(CurrencyUpsertDto request, CancellationToken ct = default)
    {
        if (await CodeInUseAsync(request.Code, excludeId: null, ct))
        {
            return ServiceResult<CurrencyDto>.Failure($"Currency code '{request.Code}' is already in use.");
        }

        var entity = new Currency
        {
            Name = request.Name,
            Code = request.Code,
            Symbol = request.Symbol,
            Country = request.Country,
            Description = request.Description,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
        };

        db.Currencies.Add(entity);
        await db.SaveChangesAsync(ct);

        return ServiceResult<CurrencyDto>.Success(ToDto(entity));
    }

    public async Task<ServiceResult<CurrencyDto>> EditAsync(long id, CurrencyUpsertDto request, CancellationToken ct = default)
    {
        var entity = await db.Currencies.FirstOrDefaultAsync(x => x.Id == id && x.IsActive, ct);
        if (entity is null)
        {
            return ServiceResult<CurrencyDto>.Failure("Not found.");
        }

        if (await CodeInUseAsync(request.Code, excludeId: id, ct))
        {
            return ServiceResult<CurrencyDto>.Failure($"Currency code '{request.Code}' is already in use.");
        }

        entity.Name = request.Name;
        entity.Code = request.Code;
        entity.Symbol = request.Symbol;
        entity.Country = request.Country;
        entity.Description = request.Description;
        entity.ModifiedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);

        return ServiceResult<CurrencyDto>.Success(ToDto(entity));
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
    {
        var entity = await db.Currencies.FirstOrDefaultAsync(x => x.Id == id && x.IsActive, ct);
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
        return db.Currencies.AnyAsync(x => x.IsActive && x.Code.ToLower() == normalized && (excludeId == null || x.Id != excludeId), ct);
    }

    private static CurrencyDto ToDto(Currency entity) =>
        new(entity.Id, entity.Name, entity.Code, entity.Symbol, entity.Country, entity.Description, entity.IsDefault, entity.IsActive);
}
