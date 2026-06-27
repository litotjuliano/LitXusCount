using LitXusCount.Application.Authorization;
using LitXusCount.Application.Common;
using LitXusCount.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LitXusCount.API.Controllers.Settings;

[ApiController]
[Authorize]
[Route("api/settings/lhdn-msic-codes")]
public class LhdnMsicCodesController(ApplicationDbContext db) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = "Permission." + Permissions.Settings.LhdnMsicCode.View)]
    public async Task<IActionResult> List([FromQuery] PagedQuery query, CancellationToken ct)
    {
        var q = db.LhdnMsicCodes.Where(x => x.IsActive);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim().ToLower();
            q = q.Where(x => x.Code.ToLower().Contains(term)
                           || x.Description.ToLower().Contains(term)
                           || (x.Category != null && x.Category.ToLower().Contains(term)));
        }

        q = query.SortBy switch
        {
            "description" when query.SortDescending => q.OrderByDescending(x => x.Description),
            "description"                           => q.OrderBy(x => x.Description),
            "category" when query.SortDescending    => q.OrderByDescending(x => x.Category).ThenByDescending(x => x.Code),
            "category"                              => q.OrderBy(x => x.Category).ThenBy(x => x.Code),
            "code" when query.SortDescending        => q.OrderByDescending(x => x.Code),
            _                                       => q.OrderBy(x => x.Code),
        };

        var total = await q.CountAsync(ct);
        var items = await q
            .Skip((query.EffectivePage - 1) * query.EffectivePageSize)
            .Take(query.EffectivePageSize)
            .Select(x => new { x.Id, x.Code, x.Category, x.Description, x.IsActive })
            .ToListAsync(ct);

        return Ok(new PagedResult<object>(items, total, query.EffectivePage, query.EffectivePageSize));
    }

    [HttpGet("all-active")]
    [Authorize(Policy = "Permission." + Permissions.Settings.LhdnMsicCode.View)]
    public async Task<IActionResult> ListAllActive(CancellationToken ct)
    {
        var items = await db.LhdnMsicCodes
            .Where(x => x.IsActive)
            .OrderBy(x => x.Code)
            .Select(x => new { x.Id, x.Code, x.Category, x.Description })
            .ToListAsync(ct);
        return Ok(items);
    }
}
