namespace LitXusCount.Application.Settings.Company;

public interface ICompanyInfoService
{
    Task<CompanyInfoDto> GetAsync(CancellationToken ct = default);
    Task<CompanyInfoDto> EditAsync(CompanyInfoUpdateDto request, CancellationToken ct = default);
}
