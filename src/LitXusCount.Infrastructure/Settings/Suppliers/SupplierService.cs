using LitXusCount.Application.Common;
using LitXusCount.Application.Settings.Suppliers;
using LitXusCount.Domain.Entities;
using LitXusCount.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LitXusCount.Infrastructure.Settings.Suppliers;

internal sealed class SupplierService(ApplicationDbContext db) : ISupplierService
{
    private static readonly string[] SortableColumns = ["name", "code", "city", "email"];

    public async Task<PagedResult<SupplierDto>> ListAsync(PagedQuery query, CancellationToken ct = default)
    {
        var filtered = db.Suppliers
            .Include(x => x.GlAccount)
            .Include(x => x.DefaultCurrency)
            .Where(x => x.IsActive);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var pattern = $"%{query.Search.Trim()}%";
            filtered = filtered.Where(x =>
                EF.Functions.Like(x.Name, pattern) ||
                EF.Functions.Like(x.Code, pattern) ||
                EF.Functions.Like(x.City ?? "", pattern) ||
                EF.Functions.Like(x.Email ?? "", pattern));
        }

        var totalCount = await filtered.CountAsync(ct);

        var sortBy = SortableColumns.Contains(query.SortBy?.ToLowerInvariant()) ? query.SortBy!.ToLowerInvariant() : "name";
        filtered = (sortBy, query.SortDescending) switch
        {
            ("code", true) => filtered.OrderByDescending(x => x.Code),
            ("code", false) => filtered.OrderBy(x => x.Code),
            ("city", true) => filtered.OrderByDescending(x => x.City),
            ("city", false) => filtered.OrderBy(x => x.City),
            ("email", true) => filtered.OrderByDescending(x => x.Email),
            ("email", false) => filtered.OrderBy(x => x.Email),
            (_, true) => filtered.OrderByDescending(x => x.Name),
            (_, false) => filtered.OrderBy(x => x.Name),
        };

        var items = await filtered
            .Skip((query.EffectivePage - 1) * query.EffectivePageSize)
            .Take(query.EffectivePageSize)
            .Select(x => new SupplierDto(
                x.Id,
                x.Code,
                x.Name,
                x.GlAccountId,
                x.GlAccount.Name,
                x.GlAccount.Code,
                x.Address1,
                x.Address2,
                x.Address3,
                x.AddressCode,
                x.City,
                x.State,
                x.Country,
                x.Phone,
                x.Fax,
                x.Email,
                x.ContactPerson,
                x.PaymentTermsDays,
                x.DefaultCurrencyId,
                x.DefaultCurrency != null ? x.DefaultCurrency.Code : null,
                x.IsActive))
            .ToListAsync(ct);

        return new PagedResult<SupplierDto>(items, totalCount, query.EffectivePage, query.EffectivePageSize);
    }

    public async Task<IReadOnlyList<SupplierDto>> ListAllActiveAsync(CancellationToken ct = default) =>
        await db.Suppliers
            .Include(x => x.GlAccount)
            .Include(x => x.DefaultCurrency)
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .Take(200)
            .Select(x => new SupplierDto(
                x.Id,
                x.Code,
                x.Name,
                x.GlAccountId,
                x.GlAccount.Name,
                x.GlAccount.Code,
                x.Address1,
                x.Address2,
                x.Address3,
                x.AddressCode,
                x.City,
                x.State,
                x.Country,
                x.Phone,
                x.Fax,
                x.Email,
                x.ContactPerson,
                x.PaymentTermsDays,
                x.DefaultCurrencyId,
                x.DefaultCurrency != null ? x.DefaultCurrency.Code : null,
                x.IsActive))
            .ToListAsync(ct);

    public async Task<SupplierDto?> GetAsync(long id, CancellationToken ct = default)
    {
        var entity = await db.Suppliers
            .Include(x => x.GlAccount)
            .Include(x => x.DefaultCurrency)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
        return entity is null ? null : ToDto(entity);
    }

    public async Task<ServiceResult<SupplierDto>> CreateAsync(SupplierUpsertDto request, CancellationToken ct = default)
    {
        if (await CodeInUseAsync(request.Code, excludeId: null, ct))
        {
            return ServiceResult<SupplierDto>.Failure($"Supplier code '{request.Code}' is already in use.");
        }

        var glAccount = await db.GlAccounts.FirstOrDefaultAsync(x => x.Id == request.GlAccountId && x.IsActive, ct);
        if (glAccount is null)
        {
            return ServiceResult<SupplierDto>.Failure("Specified GlAccount was not found or is inactive.");
        }

        Currency? currency = null;
        if (request.DefaultCurrencyId.HasValue)
        {
            currency = await db.Currencies.FirstOrDefaultAsync(x => x.Id == request.DefaultCurrencyId && x.IsActive, ct);
            if (currency is null)
            {
                return ServiceResult<SupplierDto>.Failure("Specified default currency was not found or is inactive.");
            }
        }

        var entity = new Supplier
        {
            Code = request.Code,
            Name = request.Name,
            GlAccountId = request.GlAccountId,
            Address1 = request.Address1,
            Address2 = request.Address2,
            Address3 = request.Address3,
            AddressCode = request.AddressCode,
            City = request.City,
            State = request.State,
            Country = request.Country,
            Phone = request.Phone,
            Fax = request.Fax,
            Email = request.Email,
            ContactPerson = request.ContactPerson,
            PaymentTermsDays = request.PaymentTermsDays,
            DefaultCurrencyId = request.DefaultCurrencyId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
        };

        db.Suppliers.Add(entity);
        await db.SaveChangesAsync(ct);

        entity.GlAccount = glAccount; // populate for DTO mapping
        entity.DefaultCurrency = currency;
        return ServiceResult<SupplierDto>.Success(ToDto(entity));
    }

    public async Task<ServiceResult<SupplierDto>> EditAsync(long id, SupplierUpsertDto request, CancellationToken ct = default)
    {
        var entity = await db.Suppliers
            .Include(x => x.GlAccount)
            .Include(x => x.DefaultCurrency)
            .FirstOrDefaultAsync(x => x.Id == id && x.IsActive, ct);
        if (entity is null)
        {
            return ServiceResult<SupplierDto>.Failure("Not found.");
        }

        if (await CodeInUseAsync(request.Code, excludeId: id, ct))
        {
            return ServiceResult<SupplierDto>.Failure($"Supplier code '{request.Code}' is already in use.");
        }

        var glAccount = await db.GlAccounts.FirstOrDefaultAsync(x => x.Id == request.GlAccountId && x.IsActive, ct);
        if (glAccount is null)
        {
            return ServiceResult<SupplierDto>.Failure("Specified GlAccount was not found or is inactive.");
        }

        Currency? currency = null;
        if (request.DefaultCurrencyId.HasValue)
        {
            currency = await db.Currencies.FirstOrDefaultAsync(x => x.Id == request.DefaultCurrencyId && x.IsActive, ct);
            if (currency is null)
            {
                return ServiceResult<SupplierDto>.Failure("Specified default currency was not found or is inactive.");
            }
        }

        entity.Code = request.Code;
        entity.Name = request.Name;
        entity.GlAccountId = request.GlAccountId;
        entity.GlAccount = glAccount;
        entity.Address1 = request.Address1;
        entity.Address2 = request.Address2;
        entity.Address3 = request.Address3;
        entity.AddressCode = request.AddressCode;
        entity.City = request.City;
        entity.State = request.State;
        entity.Country = request.Country;
        entity.Phone = request.Phone;
        entity.Fax = request.Fax;
        entity.Email = request.Email;
        entity.ContactPerson = request.ContactPerson;
        entity.PaymentTermsDays = request.PaymentTermsDays;
        entity.DefaultCurrencyId = request.DefaultCurrencyId;
        entity.DefaultCurrency = currency;
        entity.ModifiedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);

        return ServiceResult<SupplierDto>.Success(ToDto(entity));
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
    {
        var entity = await db.Suppliers.FirstOrDefaultAsync(x => x.Id == id && x.IsActive, ct);
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
        return db.Suppliers.AnyAsync(x => x.IsActive && x.Code.ToLower() == normalized && (excludeId == null || x.Id != excludeId), ct);
    }

    private static SupplierDto ToDto(Supplier x) =>
        new(
            x.Id,
            x.Code,
            x.Name,
            x.GlAccountId,
            x.GlAccount.Name,
            x.GlAccount.Code,
            x.Address1,
            x.Address2,
            x.Address3,
            x.AddressCode,
            x.City,
            x.State,
            x.Country,
            x.Phone,
            x.Fax,
            x.Email,
            x.ContactPerson,
            x.PaymentTermsDays,
            x.DefaultCurrencyId,
            x.DefaultCurrency != null ? x.DefaultCurrency.Code : null,
            x.IsActive);
}
