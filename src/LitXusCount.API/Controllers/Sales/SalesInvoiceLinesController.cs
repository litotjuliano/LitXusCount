using LitXusCount.Application.Authorization;
using LitXusCount.Application.Sales;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LitXusCount.API.Controllers.Sales;

[ApiController]
[Route("api/sales/invoice-lines")]
[Authorize]
public class SalesInvoiceLinesController(ISalesInvoiceLineService lineService) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = Permissions.Sales.Invoice.View)]
    public async Task<IActionResult> GetByInvoice([FromQuery] long invoiceId, CancellationToken ct)
    {
        var result = await lineService.GetByInvoiceAsync(invoiceId, ct);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = Permissions.Sales.Invoice.Edit)]
    public async Task<IActionResult> AddLine([FromBody] SalesInvoiceLineCreateDto dto, CancellationToken ct)
    {
        var result = await lineService.AddLineAsync(dto, ct);
        return result.Succeeded ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpDelete("{id:long}")]
    [Authorize(Policy = Permissions.Sales.Invoice.Edit)]
    public async Task<IActionResult> RemoveLine(long id, CancellationToken ct)
    {
        var deleted = await lineService.RemoveLineAsync(id, ct);
        return deleted ? NoContent() : NotFound();
    }
}
