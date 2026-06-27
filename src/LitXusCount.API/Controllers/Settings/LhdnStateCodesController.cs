using LitXusCount.Application.Authorization;
using LitXusCount.Application.Common;
using LitXusCount.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LitXusCount.API.Controllers.Settings;

[ApiController]
[Authorize]
[Route("api/settings/lhdn-state-codes")]
public class LhdnStateCodesController(ApplicationDbContext db) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = "Permission." + Permissions.Settings.LhdnStateCode.View)]
    public async Task<IActionResult> List([FromQuery] PagedQuery query, CancellationToken ct)
    {
        var q = db.LhdnStateCodes.Where(x => x.IsActive);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim().ToLower();
            q = q.Where(x => x.Code.ToLower().Contains(term) || x.Name.ToLower().Contains(term));
        }

        q = query.SortBy switch
        {
            "name" when query.SortDescending  => q.OrderByDescending(x => x.Name),
            "name"                            => q.OrderBy(x => x.Name),
            "code" when query.SortDescending  => q.OrderByDescending(x => x.Code),
            _                                 => q.OrderBy(x => x.Code),
        };

        var total = await q.CountAsync(ct);
        var items = await q
            .Skip((query.EffectivePage - 1) * query.EffectivePageSize)
            .Take(query.EffectivePageSize)
            .Select(x => new { x.Id, x.Code, x.Name, x.IsActive })
            .ToListAsync(ct);

        return Ok(new PagedResult<object>(items, total, query.EffectivePage, query.EffectivePageSize));
    }

    [HttpGet("all-active")]
    [Authorize(Policy = "Permission." + Permissions.Settings.LhdnStateCode.View)]
    public async Task<IActionResult> ListAllActive(CancellationToken ct)
    {
        var items = await db.LhdnStateCodes
            .Where(x => x.IsActive)
            .OrderBy(x => x.Code)
            .Select(x => new { x.Id, x.Code, x.Name })
            .ToListAsync(ct);
        return Ok(items);
    }
}
