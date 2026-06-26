using LitXusCount.Application.Authorization;
using LitXusCount.Application.Settings.Company;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LitXusCount.API.Controllers.Settings;

[ApiController]
[Authorize]
[Route("api/settings/company-info")]
public class CompanyInfoController(ICompanyInfoService service, IWebHostEnvironment env) : ControllerBase
{
    private static readonly string[] AllowedImageTypes = ["image/png", "image/jpeg", "image/gif", "image/webp"];
    private const long MaxLogoBytes = 5 * 1024 * 1024; // 5 MB

    [HttpGet]
    [Authorize(Policy = "Permission." + Permissions.Settings.CompanyInfo.View)]
    public async Task<IActionResult> Get(CancellationToken ct) => Ok(await service.GetAsync(ct));

    [HttpPut]
    [Authorize(Policy = "Permission." + Permissions.Settings.CompanyInfo.Edit)]
    public async Task<IActionResult> Edit(CompanyInfoUpdateDto request, CancellationToken ct) =>
        Ok(await service.EditAsync(request, ct));

    [HttpPost("logo")]
    [Authorize(Policy = "Permission." + Permissions.Settings.CompanyInfo.Edit)]
    public async Task<IActionResult> UploadLogo(IFormFile file, CancellationToken ct)
    {
        if (file is null || file.Length == 0)
            return BadRequest("No file provided.");

        if (!AllowedImageTypes.Contains(file.ContentType.ToLowerInvariant()))
            return BadRequest("Invalid file type. Allowed: PNG, JPG, GIF, WEBP.");

        if (file.Length > MaxLogoBytes)
            return BadRequest("File exceeds 5 MB limit.");

        var uploadsDir = Path.Combine(env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"),
            "uploads", "logos");
        Directory.CreateDirectory(uploadsDir);

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        var fileName = $"{Guid.NewGuid()}{ext}";
        var filePath = Path.Combine(uploadsDir, fileName);

        await using var stream = System.IO.File.Create(filePath);
        await file.CopyToAsync(stream, ct);

        return Ok(new { url = $"/uploads/logos/{fileName}" });
    }
}
