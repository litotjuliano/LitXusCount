using LitXusCount.Application.Admin.Roles;
using LitXusCount.Application.Authorization;
using LitXusCount.Application.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LitXusCount.API.Controllers.Admin;

[ApiController]
[Authorize]
[Route("api/admin/roles")]
public class RolesController(IRoleService service) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = "Permission." + Permissions.Roles.View)]
    public async Task<IActionResult> List([FromQuery] PagedQuery query, CancellationToken ct) =>
        Ok(await service.ListAsync(query, ct));

    [HttpGet("permissions-catalog")]
    [Authorize(Policy = "Permission." + Permissions.Roles.View)]
    public IActionResult PermissionsCatalog() => Ok(service.GetPermissionCatalog());

    [HttpGet("{id}")]
    [Authorize(Policy = "Permission." + Permissions.Roles.View)]
    public async Task<IActionResult> Get(string id, CancellationToken ct)
    {
        var role = await service.GetAsync(id, ct);
        return role is null ? NotFound() : Ok(role);
    }

    [HttpPost]
    [Authorize(Policy = "Permission." + Permissions.Roles.Create)]
    public async Task<IActionResult> Create(RoleUpsertDto request, CancellationToken ct)
    {
        var result = await service.CreateAsync(request, ct);
        return result.Succeeded ? Ok(result.Value) : BadRequest(new { message = result.Error });
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "Permission." + Permissions.Roles.Edit)]
    public async Task<IActionResult> Edit(string id, RoleUpsertDto request, CancellationToken ct)
    {
        var result = await service.EditAsync(id, request, ct);
        return result.Succeeded ? Ok(result.Value) : BadRequest(new { message = result.Error });
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "Permission." + Permissions.Roles.Delete)]
    public async Task<IActionResult> Delete(string id, CancellationToken ct)
    {
        var result = await service.DeleteAsync(id, ct);
        return result.Succeeded ? NoContent() : BadRequest(new { message = result.Error });
    }
}
