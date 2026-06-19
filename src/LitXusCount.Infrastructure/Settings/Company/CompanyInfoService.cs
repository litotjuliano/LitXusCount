using LitXusCount.Application.Settings.Company;
using LitXusCount.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using DomainCompanyInfo = LitXusCount.Domain.Entities.CompanyInfo;

namespace LitXusCount.Infrastructure.Settings.Company;

internal sealed class CompanyInfoService(ApplicationDbContext db) : ICompanyInfoService
{
    public async Task<CompanyInfoDto> GetAsync(CancellationToken ct = default)
    {
        var entity = await GetSingletonAsync(ct);
        return ToDto(entity);
    }

    public async Task<CompanyInfoDto> EditAsync(CompanyInfoUpdateDto request, CancellationToken ct = default)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(ct);

        var entity = await GetSingletonAsync(ct);

        if (entity.CurrencyId != request.CurrencyId)
        {
            await SetDefaultCurrencyAsync(request.CurrencyId, ct);
        }

        if (entity.VatPercentageId != request.VatPercentageId)
        {
            await SetDefaultVatPercentageAsync(request.VatPercentageId, ct);
        }

        if (entity.EmailConfigId != request.EmailConfigId)
        {
            await SetDefaultEmailConfigAsync(request.EmailConfigId, ct);
        }

        entity.Name = request.Name;
        entity.LogoUrl = request.LogoUrl;
        entity.Address = request.Address;
        entity.City = request.City;
        entity.Country = request.Country;
        entity.PostCode = request.PostCode;
        entity.Phone = request.Phone;
        entity.Mobile = request.Mobile;
        entity.Email = request.Email;
        entity.Fax = request.Fax;
        entity.Website = request.Website;
        entity.CompanyRegistrationNumber = request.CompanyRegistrationNumber;
        entity.VatRegistrationNumber = request.VatRegistrationNumber;
        entity.InvoiceNumberPrefix = request.InvoiceNumberPrefix;
        entity.QuoteNumberPrefix = request.QuoteNumberPrefix;
        entity.TermsAndConditions = request.TermsAndConditions;
        entity.IsVatEnabled = request.IsVatEnabled;
        entity.VatTitle = request.VatTitle;
        entity.IsItemDiscountPercentage = request.IsItemDiscountPercentage;
        entity.CurrencyId = request.CurrencyId;
        entity.VatPercentageId = request.VatPercentageId;
        entity.EmailConfigId = request.EmailConfigId;
        entity.ModifiedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);

        return ToDto(entity);
    }

    private async Task<DomainCompanyInfo> GetSingletonAsync(CancellationToken ct) =>
        await db.CompanyInfos.FirstAsync(x => x.Id == 1, ct);

    private async Task SetDefaultCurrencyAsync(long? newDefaultId, CancellationToken ct)
    {
        await db.Currencies.Where(x => x.IsDefault).ExecuteUpdateAsync(s => s.SetProperty(x => x.IsDefault, false), ct);
        if (newDefaultId is { } id)
        {
            var newDefault = await db.Currencies.FirstAsync(x => x.Id == id, ct);
            newDefault.IsDefault = true;
        }
    }

    private async Task SetDefaultVatPercentageAsync(long? newDefaultId, CancellationToken ct)
    {
        await db.VatPercentages.Where(x => x.IsDefault).ExecuteUpdateAsync(s => s.SetProperty(x => x.IsDefault, false), ct);
        if (newDefaultId is { } id)
        {
            var newDefault = await db.VatPercentages.FirstAsync(x => x.Id == id, ct);
            newDefault.IsDefault = true;
        }
    }

    private async Task SetDefaultEmailConfigAsync(long? newDefaultId, CancellationToken ct)
    {
        await db.EmailConfigs.Where(x => x.IsDefault).ExecuteUpdateAsync(s => s.SetProperty(x => x.IsDefault, false), ct);
        if (newDefaultId is { } id)
        {
            var newDefault = await db.EmailConfigs.FirstAsync(x => x.Id == id, ct);
            newDefault.IsDefault = true;
        }
    }

    private static CompanyInfoDto ToDto(DomainCompanyInfo entity) => new(
        entity.Id,
        entity.Name,
        entity.LogoUrl,
        entity.Address,
        entity.City,
        entity.Country,
        entity.PostCode,
        entity.Phone,
        entity.Mobile,
        entity.Email,
        entity.Fax,
        entity.Website,
        entity.CompanyRegistrationNumber,
        entity.VatRegistrationNumber,
        entity.InvoiceNumberPrefix,
        entity.QuoteNumberPrefix,
        entity.TermsAndConditions,
        entity.IsVatEnabled,
        entity.VatTitle,
        entity.IsItemDiscountPercentage,
        entity.CurrencyId,
        entity.VatPercentageId,
        entity.EmailConfigId);
}
