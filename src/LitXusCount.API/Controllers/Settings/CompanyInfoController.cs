using LitXusCount.Application.Authorization;
using LitXusCount.Application.Settings.Company;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LitXusCount.API.Controllers.Settings;

[ApiController]
[Authorize]
[Route("api/settings/company-info")]
public class CompanyInfoController(ICompanyInfoService service) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = "Permission." + Permissions.Settings.CompanyInfo.View)]
    public async Task<IActionResult> Get(CancellationToken ct) => Ok(await service.GetAsync(ct));

    [HttpPut]
    [Authorize(Policy = "Permission." + Permissions.Settings.CompanyInfo.Edit)]
    public async Task<IActionResult> Edit(CompanyInfoUpdateDto request, CancellationToken ct) =>
        Ok(await service.EditAsync(request, ct));
}
