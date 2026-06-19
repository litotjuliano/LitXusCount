using LitXusCount.Application.Admin.Users;
using LitXusCount.Application.Authorization;
using LitXusCount.Application.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LitXusCount.API.Controllers.Admin;

[ApiController]
[Authorize]
[Route("api/admin/users")]
public class UsersController(IUserManagementService service) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = "Permission." + Permissions.Users.View)]
    public async Task<IActionResult> List([FromQuery] PagedQuery query, [FromQuery] bool includeInactive, CancellationToken ct) =>
        Ok(await service.ListAsync(query, includeInactive, ct));

    [HttpGet("{id}")]
    [Authorize(Policy = "Permission." + Permissions.Users.View)]
    public async Task<IActionResult> Get(string id, CancellationToken ct)
    {
        var user = await service.GetAsync(id, ct);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpPost]
    [Authorize(Policy = "Permission." + Permissions.Users.Create)]
    public async Task<IActionResult> Create(UserCreateDto request, CancellationToken ct)
    {
        var result = await service.CreateAsync(request, ct);
        return result.Succeeded ? Ok(result.Value) : BadRequest(new { message = result.Error });
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "Permission." + Permissions.Users.Edit)]
    public async Task<IActionResult> Edit(string id, UserEditDto request, CancellationToken ct)
    {
        var result = await service.EditAsync(id, request, ct);
        return result.Succeeded ? Ok(result.Value) : BadRequest(new { message = result.Error });
    }

    [HttpPost("{id}/deactivate")]
    [Authorize(Policy = "Permission." + Permissions.Users.Delete)]
    public async Task<IActionResult> Deactivate(string id, CancellationToken ct)
    {
        var result = await service.DeactivateAsync(id, ct);
        return result.Succeeded ? NoContent() : BadRequest(new { message = result.Error });
    }

    [HttpPost("{id}/reactivate")]
    [Authorize(Policy = "Permission." + Permissions.Users.Edit)]
    public async Task<IActionResult> Reactivate(string id, CancellationToken ct)
    {
        var result = await service.ReactivateAsync(id, ct);
        return result.Succeeded ? NoContent() : BadRequest(new { message = result.Error });
    }

    [HttpPost("{id}/reset-password")]
    [Authorize(Policy = "Permission." + Permissions.Users.Edit)]
    public async Task<IActionResult> ResetPassword(string id, AdminResetPasswordRequest request, CancellationToken ct)
    {
        var result = await service.AdminResetPasswordAsync(id, request.NewPassword, ct);
        return result.Succeeded ? NoContent() : BadRequest(new { message = result.Error });
    }
}

public record AdminResetPasswordRequest(string NewPassword);
