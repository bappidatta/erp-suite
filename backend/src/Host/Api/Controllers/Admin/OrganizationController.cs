using ErpSuite.Modules.Admin.Application.Organization;
using ErpSuite.Modules.Admin.Application.Organization.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Admin;

[ApiController]
[Route("api/admin/organization")]
[Authorize(Policy = "AdminOnly")]
public sealed class OrganizationController : ControllerBase
{
    private readonly IOrganizationSettingsService _organizationService;

    public OrganizationController(IOrganizationSettingsService organizationService) =>
        _organizationService = organizationService;

    [HttpGet]
    public async Task<IActionResult> GetSettings(CancellationToken cancellationToken)
    {
        var settings = await _organizationService.GetAsync(cancellationToken);
        if (settings is null)
            return Ok(new OrganizationSettingsResponse(0, "My Organization", null, null, null, null, null, null, null, "USD", null, null, null, null));
        return Ok(settings);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateSettings([FromBody] UpdateOrganizationSettingsRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = User.FindFirst("user_id")?.Value ?? "system";
        var result = await _organizationService.UpdateAsync(request, currentUserId, cancellationToken);
        if (result.IsFailure) return BadRequest(new { message = result.Error });
        return Ok(result.Value);
    }

    [HttpPost("logo")]
    [RequestSizeLimit(5 * 1024 * 1024)]
    public async Task<IActionResult> UploadLogo(IFormFile file, CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { message = "No file provided." });

        var currentUserId = User.FindFirst("user_id")?.Value ?? "system";

        await using var stream = file.OpenReadStream();
        var result = await _organizationService.UploadLogoAsync(stream, file.FileName, currentUserId, cancellationToken);

        if (result.IsFailure) return BadRequest(new { message = result.Error });
        return Ok(new { logoPath = result.Value });
    }
}
