using LitXusCount.Application.Authorization;
using LitXusCount.Application.Sales;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LitXusCount.API.Controllers.Sales;

[ApiController]
[Route("api/sales/payments")]
[Authorize]
public class SalesPaymentsController(ISalesPaymentService paymentService) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = Permissions.Sales.Invoice.View)]
    public async Task<IActionResult> GetByInvoice([FromQuery] long invoiceId, CancellationToken ct)
    {
        var result = await paymentService.GetByInvoiceAsync(invoiceId, ct);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = Permissions.Sales.Invoice.Manage)]
    public async Task<IActionResult> RecordPayment([FromBody] SalesPaymentRecordCreateDto dto, CancellationToken ct)
    {
        var result = await paymentService.RecordPaymentAsync(dto, ct);
        return result.Succeeded ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpDelete("{id:long}")]
    [Authorize(Policy = Permissions.Sales.Invoice.Manage)]
    public async Task<IActionResult> DeletePayment(long id, CancellationToken ct)
    {
        var deleted = await paymentService.DeletePaymentAsync(id, ct);
        return deleted ? NoContent() : NotFound();
    }
}
