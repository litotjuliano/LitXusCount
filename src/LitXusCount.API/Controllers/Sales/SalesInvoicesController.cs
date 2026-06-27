using LitXusCount.Application.Authorization;
using LitXusCount.Application.Common;
using LitXusCount.Application.Sales;
using LitXusCount.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LitXusCount.API.Controllers.Sales;

[ApiController]
[Route("api/sales/invoices")]
[Authorize]
public class SalesInvoicesController(ISalesInvoiceService salesInvoiceService) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = Permissions.Sales.Invoice.View)]
    public async Task<IActionResult> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false,
        [FromQuery] SalesInvoiceCategory? category = null,
        CancellationToken ct = default)
    {
        var query = new PagedQuery { Page = page, PageSize = pageSize, Search = search, SortBy = sortBy, SortDescending = sortDescending };
        var result = await salesInvoiceService.GetPagedAsync(query, category, ct);
        return Ok(result);
    }

    [HttpGet("{id:long}")]
    [Authorize(Policy = Permissions.Sales.Invoice.View)]
    public async Task<IActionResult> GetById(long id, CancellationToken ct)
    {
        var result = await salesInvoiceService.GetByIdAsync(id, ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = Permissions.Sales.Invoice.Create)]
    public async Task<IActionResult> CreateDraft([FromBody] SalesInvoiceCreateDto dto, CancellationToken ct)
    {
        var result = await salesInvoiceService.CreateDraftAsync(dto, ct);
        return result.Succeeded ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPut("{id:long}")]
    [Authorize(Policy = Permissions.Sales.Invoice.Edit)]
    public async Task<IActionResult> UpdateHeader(long id, [FromBody] SalesInvoiceHeaderUpdateDto dto, CancellationToken ct)
    {
        var result = await salesInvoiceService.UpdateHeaderAsync(id, dto, ct);
        return result.Succeeded ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPatch("{id:long}/promote")]
    [Authorize(Policy = Permissions.Sales.Invoice.Manage)]
    public async Task<IActionResult> Promote(long id, [FromBody] SalesInvoicePromoteDto dto, CancellationToken ct)
    {
        var result = await salesInvoiceService.PromoteAsync(id, dto.TargetCategory, ct);
        return result.Succeeded ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpDelete("{id:long}")]
    [Authorize(Policy = Permissions.Sales.Invoice.Delete)]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
    {
        var deleted = await salesInvoiceService.DeleteAsync(id, ct);
        return deleted ? NoContent() : NotFound();
    }
}
