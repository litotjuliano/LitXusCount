using LitXusCount.Application.Common;
using LitXusCount.Application.Settings.Customers;
using LitXusCount.Domain.Entities;
using LitXusCount.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LitXusCount.Infrastructure.Settings.Customers;

internal sealed class CustomerService(ApplicationDbContext db) : ICustomerService
{
    private static readonly string[] SortableColumns = ["name", "code", "city", "email"];

    public async Task<PagedResult<CustomerDto>> ListAsync(PagedQuery query, CancellationToken ct = default)
    {
        var filtered = db.Customers.Include(x => x.GlAccount).Where(x => x.IsActive);

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
            .Select(x => new CustomerDto(
                x.Id,
                x.Code,
                x.Name,
                x.Name2,
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
                x.Phone2,
                x.Fax,
                x.Email,
                x.ContactPerson,
                x.ConsigneeName,
                x.ConsigneeAddress1,
                x.ConsigneeAddress2,
                x.ConsigneeAddress3,
                x.ConsigneeAddressCode,
                x.ConsigneeCity,
                x.ConsigneeState,
                x.ConsigneeCountry,
                x.ConsigneePhone,
                x.PaymentTermsDays,
                x.CreditLimit,
                x.IsLocked,
                x.IsActive,
                x.TIN,
                x.RegistrationType,
                x.RegistrationNumber,
                x.SSTRegistrationNumber))
            .ToListAsync(ct);

        return new PagedResult<CustomerDto>(items, totalCount, query.EffectivePage, query.EffectivePageSize);
    }

    public async Task<IReadOnlyList<CustomerDto>> ListAllActiveAsync(CancellationToken ct = default) =>
        await db.Customers
            .Include(x => x.GlAccount)
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .Take(200)
            .Select(x => new CustomerDto(
                x.Id,
                x.Code,
                x.Name,
                x.Name2,
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
                x.Phone2,
                x.Fax,
                x.Email,
                x.ContactPerson,
                x.ConsigneeName,
                x.ConsigneeAddress1,
                x.ConsigneeAddress2,
                x.ConsigneeAddress3,
                x.ConsigneeAddressCode,
                x.ConsigneeCity,
                x.ConsigneeState,
                x.ConsigneeCountry,
                x.ConsigneePhone,
                x.PaymentTermsDays,
                x.CreditLimit,
                x.IsLocked,
                x.IsActive,
                x.TIN,
                x.RegistrationType,
                x.RegistrationNumber,
                x.SSTRegistrationNumber))
            .ToListAsync(ct);

    public async Task<CustomerDto?> GetAsync(long id, CancellationToken ct = default)
    {
        var entity = await db.Customers
            .Include(x => x.GlAccount)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
        return entity is null ? null : ToDto(entity);
    }

    public async Task<ServiceResult<CustomerDto>> CreateAsync(CustomerUpsertDto request, CancellationToken ct = default)
    {
        if (await CodeInUseAsync(request.Code, excludeId: null, ct))
        {
            return ServiceResult<CustomerDto>.Failure($"Customer code '{request.Code}' is already in use.");
        }

        var glAccount = await db.GlAccounts.FirstOrDefaultAsync(x => x.Id == request.GlAccountId && x.IsActive, ct);
        if (glAccount is null)
        {
            return ServiceResult<CustomerDto>.Failure("Specified GlAccount was not found or is inactive.");
        }

        var entity = new Customer
        {
            Code = request.Code,
            Name = request.Name,
            Name2 = request.Name2,
            GlAccountId = request.GlAccountId,
            Address1 = request.Address1,
            Address2 = request.Address2,
            Address3 = request.Address3,
            AddressCode = request.AddressCode,
            City = request.City,
            State = request.State,
            Country = request.Country,
            Phone = request.Phone,
            Phone2 = request.Phone2,
            Fax = request.Fax,
            Email = request.Email,
            ContactPerson = request.ContactPerson,
            ConsigneeName = request.ConsigneeName,
            ConsigneeAddress1 = request.ConsigneeAddress1,
            ConsigneeAddress2 = request.ConsigneeAddress2,
            ConsigneeAddress3 = request.ConsigneeAddress3,
            ConsigneeAddressCode = request.ConsigneeAddressCode,
            ConsigneeCity = request.ConsigneeCity,
            ConsigneeState = request.ConsigneeState,
            ConsigneeCountry = request.ConsigneeCountry,
            ConsigneePhone = request.ConsigneePhone,
            PaymentTermsDays = request.PaymentTermsDays,
            CreditLimit = request.CreditLimit,
            IsLocked = request.IsLocked,
            TIN = request.Tin,
            RegistrationType = request.RegistrationType,
            RegistrationNumber = request.RegistrationNumber,
            SSTRegistrationNumber = request.SSTRegistrationNumber,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
        };

        db.Customers.Add(entity);
        await db.SaveChangesAsync(ct);

        entity.GlAccount = glAccount; // populate for DTO mapping
        return ServiceResult<CustomerDto>.Success(ToDto(entity));
    }

    public async Task<ServiceResult<CustomerDto>> EditAsync(long id, CustomerUpsertDto request, CancellationToken ct = default)
    {
        var entity = await db.Customers.Include(x => x.GlAccount).FirstOrDefaultAsync(x => x.Id == id && x.IsActive, ct);
        if (entity is null)
        {
            return ServiceResult<CustomerDto>.Failure("Not found.");
        }

        if (await CodeInUseAsync(request.Code, excludeId: id, ct))
        {
            return ServiceResult<CustomerDto>.Failure($"Customer code '{request.Code}' is already in use.");
        }

        var glAccount = await db.GlAccounts.FirstOrDefaultAsync(x => x.Id == request.GlAccountId && x.IsActive, ct);
        if (glAccount is null)
        {
            return ServiceResult<CustomerDto>.Failure("Specified GlAccount was not found or is inactive.");
        }

        entity.Code = request.Code;
        entity.Name = request.Name;
        entity.Name2 = request.Name2;
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
        entity.Phone2 = request.Phone2;
        entity.Fax = request.Fax;
        entity.Email = request.Email;
        entity.ContactPerson = request.ContactPerson;
        entity.ConsigneeName = request.ConsigneeName;
        entity.ConsigneeAddress1 = request.ConsigneeAddress1;
        entity.ConsigneeAddress2 = request.ConsigneeAddress2;
        entity.ConsigneeAddress3 = request.ConsigneeAddress3;
        entity.ConsigneeAddressCode = request.ConsigneeAddressCode;
        entity.ConsigneeCity = request.ConsigneeCity;
        entity.ConsigneeState = request.ConsigneeState;
        entity.ConsigneeCountry = request.ConsigneeCountry;
        entity.ConsigneePhone = request.ConsigneePhone;
        entity.PaymentTermsDays = request.PaymentTermsDays;
        entity.CreditLimit = request.CreditLimit;
        entity.IsLocked = request.IsLocked;
        entity.TIN = request.Tin;
        entity.RegistrationType = request.RegistrationType;
        entity.RegistrationNumber = request.RegistrationNumber;
        entity.SSTRegistrationNumber = request.SSTRegistrationNumber;
        entity.ModifiedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);

        return ServiceResult<CustomerDto>.Success(ToDto(entity));
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
    {
        var entity = await db.Customers.FirstOrDefaultAsync(x => x.Id == id && x.IsActive, ct);
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
        return db.Customers.AnyAsync(x => x.IsActive && x.Code.ToLower() == normalized && (excludeId == null || x.Id != excludeId), ct);
    }

    private static CustomerDto ToDto(Customer x) =>
        new(
            x.Id,
            x.Code,
            x.Name,
            x.Name2,
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
            x.Phone2,
            x.Fax,
            x.Email,
            x.ContactPerson,
            x.ConsigneeName,
            x.ConsigneeAddress1,
            x.ConsigneeAddress2,
            x.ConsigneeAddress3,
            x.ConsigneeAddressCode,
            x.ConsigneeCity,
            x.ConsigneeState,
            x.ConsigneeCountry,
            x.ConsigneePhone,
            x.PaymentTermsDays,
            x.CreditLimit,
            x.IsLocked,
            x.IsActive,
            x.TIN,
            x.RegistrationType,
            x.RegistrationNumber,
            x.SSTRegistrationNumber);
}
