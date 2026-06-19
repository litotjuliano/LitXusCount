using LitXusCount.Application.Common;
using LitXusCount.Application.Settings.Lookups;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LitXusCount.API.Controllers.Settings;

/// <summary>
/// Shared by 5 lookup entities, each requiring its own resource-specific permission
/// (e.g. Settings.PaymentType.Delete vs Settings.Category.Delete). A static [Authorize(Policy=...)]
/// attribute on these shared method bodies would apply the same policy to every subclass, so
/// authorization here is imperative instead, keyed off each subclass's <see cref="ResourceName"/>.
/// </summary>
[ApiController]
[Authorize]
public abstract class LookupControllerBase<TService>(TService service, IAuthorizationService authorizationService) : ControllerBase
    where TService : ILookupService
{
    protected abstract string ResourceName { get; }

    private Task<AuthorizationResult> AuthorizeActionAsync(string action) =>
        authorizationService.AuthorizeAsync(User, $"Permission.Settings.{ResourceName}.{action}");

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] PagedQuery query, CancellationToken ct)
    {
        if (!(await AuthorizeActionAsync("View")).Succeeded)
        {
            return Forbid();
        }

        return Ok(await service.ListAsync(query, ct));
    }

    [HttpGet("all-active")]
    public async Task<IActionResult> ListAllActive(CancellationToken ct)
    {
        if (!(await AuthorizeActionAsync("View")).Succeeded)
        {
            return Forbid();
        }

        return Ok(await service.ListAllActiveAsync(ct));
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> Get(long id, CancellationToken ct)
    {
        if (!(await AuthorizeActionAsync("View")).Succeeded)
        {
            return Forbid();
        }

        var item = await service.GetAsync(id, ct);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create(LookupItemUpsertDto request, CancellationToken ct)
    {
        if (!(await AuthorizeActionAsync("Create")).Succeeded)
        {
            return Forbid();
        }

        var result = await service.CreateAsync(request, ct);
        return result.Succeeded ? Ok(result.Value) : BadRequest(new { message = result.Error });
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Edit(long id, LookupItemUpsertDto request, CancellationToken ct)
    {
        if (!(await AuthorizeActionAsync("Edit")).Succeeded)
        {
            return Forbid();
        }

        var result = await service.EditAsync(id, request, ct);
        return result.Succeeded ? Ok(result.Value) : BadRequest(new { message = result.Error });
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
    {
        if (!(await AuthorizeActionAsync("Delete")).Succeeded)
        {
            return Forbid();
        }

        var deleted = await service.DeleteAsync(id, ct);
        return deleted ? NoContent() : NotFound();
    }
}
