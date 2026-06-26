using LitXusCount.Application.Authorization;
using LitXusCount.Application.Common;
using LitXusCount.Application.Tenants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LitXusCount.API.Controllers;

[ApiController]
[Authorize]
[Route("api/admin/tenants")]
public class TenantsController(ITenantService service) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = "Permission." + Permissions.Tenants.View)]
    public async Task<IActionResult> List([FromQuery] PagedQuery query, CancellationToken ct) =>
        Ok(await service.ListAsync(query, ct));

    [HttpGet("{id:long}")]
    [Authorize(Policy = "Permission." + Permissions.Tenants.View)]
    public async Task<IActionResult> Get(long id, CancellationToken ct)
    {
        var item = await service.GetAsync(id, ct);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    [Authorize(Policy = "Permission." + Permissions.Tenants.Create)]
    public async Task<IActionResult> Create(TenantUpsertDto request, CancellationToken ct)
    {
        var result = await service.CreateAsync(request, ct);
        return result.Succeeded ? Ok(result.Value) : BadRequest(new { message = result.Error });
    }

    [HttpPut("{id:long}")]
    [Authorize(Policy = "Permission." + Permissions.Tenants.Edit)]
    public async Task<IActionResult> Edit(long id, TenantUpsertDto request, CancellationToken ct)
    {
        var result = await service.EditAsync(id, request, ct);
        return result.Succeeded ? Ok(result.Value) : BadRequest(new { message = result.Error });
    }

    [HttpPatch("{id:long}/toggle-active")]
    [Authorize(Policy = "Permission." + Permissions.Tenants.Edit)]
    public async Task<IActionResult> ToggleActive(long id, CancellationToken ct)
    {
        var toggled = await service.ToggleActiveAsync(id, ct);
        return toggled ? NoContent() : NotFound();
    }
}
