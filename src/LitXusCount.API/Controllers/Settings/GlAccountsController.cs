using LitXusCount.Application.Authorization;
using LitXusCount.Application.Common;
using LitXusCount.Application.Settings.GlAccounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LitXusCount.API.Controllers.Settings;

[ApiController]
[Authorize]
[Route("api/settings/gl-accounts")]
public class GlAccountsController(IGlAccountService service) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = "Permission." + Permissions.Settings.GlAccount.View)]
    public async Task<IActionResult> List([FromQuery] PagedQuery query, CancellationToken ct) =>
        Ok(await service.ListAsync(query, ct));

    [HttpGet("all-active")]
    [Authorize(Policy = "Permission." + Permissions.Settings.GlAccount.View)]
    public async Task<IActionResult> ListAllActive(CancellationToken ct) => Ok(await service.ListAllActiveAsync(ct));

    [HttpGet("{id:long}")]
    [Authorize(Policy = "Permission." + Permissions.Settings.GlAccount.View)]
    public async Task<IActionResult> Get(long id, CancellationToken ct)
    {
        var item = await service.GetAsync(id, ct);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    [Authorize(Policy = "Permission." + Permissions.Settings.GlAccount.Create)]
    public async Task<IActionResult> Create(GlAccountUpsertDto request, CancellationToken ct)
    {
        var result = await service.CreateAsync(request, ct);
        return result.Succeeded ? Ok(result.Value) : BadRequest(new { message = result.Error });
    }

    [HttpPut("{id:long}")]
    [Authorize(Policy = "Permission." + Permissions.Settings.GlAccount.Edit)]
    public async Task<IActionResult> Edit(long id, GlAccountUpsertDto request, CancellationToken ct)
    {
        var result = await service.EditAsync(id, request, ct);
        return result.Succeeded ? Ok(result.Value) : BadRequest(new { message = result.Error });
    }

    [HttpDelete("{id:long}")]
    [Authorize(Policy = "Permission." + Permissions.Settings.GlAccount.Delete)]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
    {
        var deleted = await service.DeleteAsync(id, ct);
        return deleted ? NoContent() : NotFound();
    }
}
