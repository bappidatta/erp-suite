using ErpSuite.Modules.Procurement.Application.Vendors;
using ErpSuite.Modules.Procurement.Application.Vendors.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Procurement;

[ApiController]
[Route("api/procurement/vendors")]
[Authorize(Policy = "AuthenticatedUser")]
public sealed class VendorsController : ControllerBase
{
    private readonly IVendorService _vendorService;

    public VendorsController(IVendorService vendorService) => _vendorService = vendorService;

    [HttpGet]
    public async Task<IActionResult> GetVendors([FromQuery] GetVendorsQuery query, CancellationToken cancellationToken)
    {
        var result = await _vendorService.GetVendorsAsync(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetVendor(long id, CancellationToken cancellationToken)
    {
        var vendor = await _vendorService.GetVendorByIdAsync(id, cancellationToken);
        return vendor is null ? NotFound(new { message = "Vendor not found." }) : Ok(vendor);
    }

    [HttpPost]
    public async Task<IActionResult> CreateVendor([FromBody] CreateVendorRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = User.FindFirst("user_id")?.Value ?? "system";
        var result = await _vendorService.CreateVendorAsync(request, currentUserId, cancellationToken);
        if (result.IsFailure) return BadRequest(new { message = result.Error });
        return CreatedAtAction(nameof(GetVendor), new { id = result.Value.Id }, result.Value);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> UpdateVendor(long id, [FromBody] UpdateVendorRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = User.FindFirst("user_id")?.Value ?? "system";
        var result = await _vendorService.UpdateVendorAsync(id, request, currentUserId, cancellationToken);
        if (result.IsFailure) return BadRequest(new { message = result.Error });
        return Ok(result.Value);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteVendor(long id, CancellationToken cancellationToken)
    {
        var currentUserId = User.FindFirst("user_id")?.Value ?? "system";
        var result = await _vendorService.DeleteVendorAsync(id, currentUserId, cancellationToken);
        if (result.IsFailure) return BadRequest(new { message = result.Error });
        return NoContent();
    }

    [HttpPost("{id:long}/activate")]
    public async Task<IActionResult> ActivateVendor(long id, CancellationToken cancellationToken)
    {
        var currentUserId = User.FindFirst("user_id")?.Value ?? "system";
        var result = await _vendorService.ActivateVendorAsync(id, currentUserId, cancellationToken);
        if (result.IsFailure) return BadRequest(new { message = result.Error });
        return Ok(new { message = "Vendor activated successfully." });
    }

    [HttpPost("{id:long}/deactivate")]
    public async Task<IActionResult> DeactivateVendor(long id, CancellationToken cancellationToken)
    {
        var currentUserId = User.FindFirst("user_id")?.Value ?? "system";
        var result = await _vendorService.DeactivateVendorAsync(id, currentUserId, cancellationToken);
        if (result.IsFailure) return BadRequest(new { message = result.Error });
        return Ok(new { message = "Vendor deactivated successfully." });
    }
}
