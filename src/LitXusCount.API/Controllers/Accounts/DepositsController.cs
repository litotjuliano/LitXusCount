using LitXusCount.Application.Accounts;
using LitXusCount.Application.Authorization;
using LitXusCount.Application.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LitXusCount.API.Controllers.Accounts;

[ApiController]
[Authorize]
[Route("api/accounts/deposits")]
public class DepositsController(IAccDepositService service) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = "Permission." + Permissions.Accounts.Deposit.View)]
    public async Task<IActionResult> List([FromQuery] PagedQuery query, [FromQuery] long? accountId, CancellationToken ct) =>
        Ok(await service.ListAsync(query, accountId, ct));

    [HttpGet("{id:long}")]
    [Authorize(Policy = "Permission." + Permissions.Accounts.Deposit.View)]
    public async Task<IActionResult> Get(long id, CancellationToken ct)
    {
        var item = await service.GetAsync(id, ct);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    [Authorize(Policy = "Permission." + Permissions.Accounts.Deposit.Create)]
    public async Task<IActionResult> Create(AccDepositCreateDto request, CancellationToken ct)
    {
        var result = await service.CreateAsync(request, ct);
        return result.Succeeded ? Ok(result.Value) : BadRequest(new { message = result.Error });
    }

    [HttpDelete("{id:long}")]
    [Authorize(Policy = "Permission." + Permissions.Accounts.Deposit.Delete)]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
    {
        var deleted = await service.DeleteAsync(id, ct);
        return deleted ? NoContent() : NotFound();
    }
}
