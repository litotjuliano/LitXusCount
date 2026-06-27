using LitXusCount.Application.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LitXusCount.API.Controllers.Settings;

[ApiController]
[Authorize]
[Route("api/settings/lhdn-einvoice-types")]
public class LhdnEInvoiceTypesController : ControllerBase
{
    private static readonly IReadOnlyList<object> Types =
    [
        new { Code = "01", Description = "Invoice" },
        new { Code = "02", Description = "Credit Note" },
        new { Code = "03", Description = "Debit Note" },
        new { Code = "04", Description = "Refund Note" },
        new { Code = "11", Description = "Self-billed Invoice" },
        new { Code = "12", Description = "Self-billed Credit Note" },
        new { Code = "13", Description = "Self-billed Debit Note" },
        new { Code = "14", Description = "Self-billed Refund Note" },
    ];

    [HttpGet]
    [Authorize(Policy = "Permission." + Permissions.Settings.LhdnEInvoiceType.View)]
    public IActionResult List() => Ok(Types);
}
