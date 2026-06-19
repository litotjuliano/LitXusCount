using LitXusCount.Application.Common;
using LitXusCount.Application.Settings.Lookups;
using LitXusCount.Domain.Entities;
using LitXusCount.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LitXusCount.Infrastructure.Settings.Lookups;

internal abstract class LookupServiceBase<TEntity>
    where TEntity : class, ISimpleLookupEntity, new()
{
    private readonly ApplicationDbContext _db;
    private readonly DbSet<TEntity> _set;

    protected LookupServiceBase(ApplicationDbContext db)
    {
        _db = db;
        _set = db.Set<TEntity>();
    }

    private static readonly string[] SortableColumns = ["name", "description"];

    public async Task<PagedResult<LookupItemDto>> ListAsync(PagedQuery query, CancellationToken ct = default)
    {
        var filtered = _set.Where(x => x.IsActive);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var pattern = $"%{query.Search.Trim()}%";
            filtered = filtered.Where(x => EF.Functions.Like(x.Name, pattern) || EF.Functions.Like(x.Description ?? "", pattern));
        }

        var totalCount = await filtered.CountAsync(ct);

        var sortBy = SortableColumns.Contains(query.SortBy?.ToLowerInvariant()) ? query.SortBy!.ToLowerInvariant() : "name";
        filtered = (sortBy, query.SortDescending) switch
        {
            ("description", true) => filtered.OrderByDescending(x => x.Description),
            ("description", false) => filtered.OrderBy(x => x.Description),
            (_, true) => filtered.OrderByDescending(x => x.Name),
            (_, false) => filtered.OrderBy(x => x.Name),
        };

        var items = await filtered
            .Skip((query.EffectivePage - 1) * query.EffectivePageSize)
            .Take(query.EffectivePageSize)
            .Select(x => new LookupItemDto(x.Id, x.Name, x.Description, x.IsActive))
            .ToListAsync(ct);

        return new PagedResult<LookupItemDto>(items, totalCount, query.EffectivePage, query.EffectivePageSize);
    }

    public async Task<IReadOnlyList<LookupItemDto>> ListAllActiveAsync(CancellationToken ct = default)
    {
        return await _set
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .Take(100)
            .Select(x => new LookupItemDto(x.Id, x.Name, x.Description, x.IsActive))
            .ToListAsync(ct);
    }

    public async Task<LookupItemDto?> GetAsync(long id, CancellationToken ct = default)
    {
        var entity = await _set.FirstOrDefaultAsync(x => x.Id == id, ct);
        return entity is null ? null : ToDto(entity);
    }

    public async Task<ServiceResult<LookupItemDto>> CreateAsync(LookupItemUpsertDto request, CancellationToken ct = default)
    {
        if (await NameInUseAsync(request.Name, excludeId: null, ct))
        {
            return ServiceResult<LookupItemDto>.Failure($"'{request.Name}' is already in use.");
        }

        var entity = new TEntity
        {
            Name = request.Name,
            Description = request.Description,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
        };

        _set.Add(entity);
        await _db.SaveChangesAsync(ct);

        return ServiceResult<LookupItemDto>.Success(ToDto(entity));
    }

    public async Task<ServiceResult<LookupItemDto>> EditAsync(long id, LookupItemUpsertDto request, CancellationToken ct = default)
    {
        var entity = await _set.FirstOrDefaultAsync(x => x.Id == id && x.IsActive, ct);
        if (entity is null)
        {
            return ServiceResult<LookupItemDto>.Failure("Not found.");
        }

        if (await NameInUseAsync(request.Name, excludeId: id, ct))
        {
            return ServiceResult<LookupItemDto>.Failure($"'{request.Name}' is already in use.");
        }

        entity.Name = request.Name;
        entity.Description = request.Description;
        entity.ModifiedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);

        return ServiceResult<LookupItemDto>.Success(ToDto(entity));
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
    {
        var entity = await _set.FirstOrDefaultAsync(x => x.Id == id && x.IsActive, ct);
        if (entity is null)
        {
            return false;
        }

        entity.IsActive = false;
        entity.ModifiedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return true;
    }

    private Task<bool> NameInUseAsync(string name, long? excludeId, CancellationToken ct)
    {
        var normalized = name.Trim().ToLower();
        return _set.AnyAsync(x => x.IsActive && x.Name.ToLower() == normalized && (excludeId == null || x.Id != excludeId), ct);
    }

    private static LookupItemDto ToDto(TEntity entity) => new(entity.Id, entity.Name, entity.Description, entity.IsActive);
}
