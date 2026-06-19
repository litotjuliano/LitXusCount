using LitXusCount.Application.Common;
using LitXusCount.Application.Settings.VatPercentages;
using LitXusCount.Domain.Entities;
using LitXusCount.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LitXusCount.Infrastructure.Settings.VatPercentages;

internal sealed class VatPercentageService(ApplicationDbContext db) : IVatPercentageService
{
    private static readonly string[] SortableColumns = ["name", "percentage"];

    public async Task<PagedResult<VatPercentageDto>> ListAsync(PagedQuery query, CancellationToken ct = default)
    {
        var filtered = db.VatPercentages.Where(x => x.IsActive);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var pattern = $"%{query.Search.Trim()}%";
            filtered = filtered.Where(x => EF.Functions.Like(x.Name, pattern));
        }

        var totalCount = await filtered.CountAsync(ct);

        var sortBy = SortableColumns.Contains(query.SortBy?.ToLowerInvariant()) ? query.SortBy!.ToLowerInvariant() : "name";
        filtered = (sortBy, query.SortDescending) switch
        {
            ("percentage", true) => filtered.OrderByDescending(x => x.Percentage),
            ("percentage", false) => filtered.OrderBy(x => x.Percentage),
            (_, true) => filtered.OrderByDescending(x => x.Name),
            (_, false) => filtered.OrderBy(x => x.Name),
        };

        var items = await filtered
            .Skip((query.EffectivePage - 1) * query.EffectivePageSize)
            .Take(query.EffectivePageSize)
            .Select(x => new VatPercentageDto(x.Id, x.Name, x.Percentage, x.IsDefault, x.IsActive))
            .ToListAsync(ct);

        return new PagedResult<VatPercentageDto>(items, totalCount, query.EffectivePage, query.EffectivePageSize);
    }

    public async Task<IReadOnlyList<VatPercentageDto>> ListAllActiveAsync(CancellationToken ct = default) =>
        await db.VatPercentages
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .Take(100)
            .Select(x => new VatPercentageDto(x.Id, x.Name, x.Percentage, x.IsDefault, x.IsActive))
            .ToListAsync(ct);

    public async Task<VatPercentageDto?> GetAsync(long id, CancellationToken ct = default)
    {
        var entity = await db.VatPercentages.FirstOrDefaultAsync(x => x.Id == id, ct);
        return entity is null ? null : ToDto(entity);
    }

    public async Task<ServiceResult<VatPercentageDto>> CreateAsync(VatPercentageUpsertDto request, CancellationToken ct = default)
    {
        var entity = new VatPercentage
        {
            Name = request.Name,
            Percentage = request.Percentage,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
        };

        db.VatPercentages.Add(entity);
        await db.SaveChangesAsync(ct);

        return ServiceResult<VatPercentageDto>.Success(ToDto(entity));
    }

    public async Task<ServiceResult<VatPercentageDto>> EditAsync(long id, VatPercentageUpsertDto request, CancellationToken ct = default)
    {
        var entity = await db.VatPercentages.FirstOrDefaultAsync(x => x.Id == id && x.IsActive, ct);
        if (entity is null)
        {
            return ServiceResult<VatPercentageDto>.Failure("Not found.");
        }

        entity.Name = request.Name;
        entity.Percentage = request.Percentage;
        entity.ModifiedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);

        return ServiceResult<VatPercentageDto>.Success(ToDto(entity));
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
    {
        var entity = await db.VatPercentages.FirstOrDefaultAsync(x => x.Id == id && x.IsActive, ct);
        if (entity is null)
        {
            return false;
        }

        entity.IsActive = false;
        entity.ModifiedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        return true;
    }

    private static VatPercentageDto ToDto(VatPercentage entity) =>
        new(entity.Id, entity.Name, entity.Percentage, entity.IsDefault, entity.IsActive);
}
