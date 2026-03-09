using ErpSuite.Modules.Finance.Application.TaxCodes;
using ErpSuite.Modules.Finance.Application.TaxCodes.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Finance;

[ApiController]
[Route("api/finance/tax-codes")]
[Authorize(Policy = "AuthenticatedUser")]
public sealed class TaxCodesController : ControllerBase
{
    private readonly ITaxCodeService _taxCodeService;

    public TaxCodesController(ITaxCodeService taxCodeService) => _taxCodeService = taxCodeService;

    [HttpGet]
    public async Task<IActionResult> GetTaxCodes([FromQuery] GetTaxCodesQuery query, CancellationToken cancellationToken)
    {
        var result = await _taxCodeService.GetTaxCodesAsync(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetTaxCode(long id, CancellationToken cancellationToken)
    {
        var taxCode = await _taxCodeService.GetTaxCodeByIdAsync(id, cancellationToken);
        return taxCode is null ? NotFound(new { message = "Tax code not found." }) : Ok(taxCode);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTaxCode([FromBody] CreateTaxCodeRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = User.FindFirst("user_id")?.Value ?? "system";
        var result = await _taxCodeService.CreateTaxCodeAsync(request, currentUserId, cancellationToken);
        if (result.IsFailure) return BadRequest(new { message = result.Error });
        return CreatedAtAction(nameof(GetTaxCode), new { id = result.Value.Id }, result.Value);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> UpdateTaxCode(long id, [FromBody] UpdateTaxCodeRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = User.FindFirst("user_id")?.Value ?? "system";
        var result = await _taxCodeService.UpdateTaxCodeAsync(id, request, currentUserId, cancellationToken);
        if (result.IsFailure) return BadRequest(new { message = result.Error });
        return Ok(result.Value);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteTaxCode(long id, CancellationToken cancellationToken)
    {
        var currentUserId = User.FindFirst("user_id")?.Value ?? "system";
        var result = await _taxCodeService.DeleteTaxCodeAsync(id, currentUserId, cancellationToken);
        if (result.IsFailure) return BadRequest(new { message = result.Error });
        return NoContent();
    }

    [HttpPost("{id:long}/activate")]
    public async Task<IActionResult> ActivateTaxCode(long id, CancellationToken cancellationToken)
    {
        var currentUserId = User.FindFirst("user_id")?.Value ?? "system";
        var result = await _taxCodeService.ActivateTaxCodeAsync(id, currentUserId, cancellationToken);
        if (result.IsFailure) return BadRequest(new { message = result.Error });
        return Ok(new { message = "Tax code activated successfully." });
    }

    [HttpPost("{id:long}/deactivate")]
    public async Task<IActionResult> DeactivateTaxCode(long id, CancellationToken cancellationToken)
    {
        var currentUserId = User.FindFirst("user_id")?.Value ?? "system";
        var result = await _taxCodeService.DeactivateTaxCodeAsync(id, currentUserId, cancellationToken);
        if (result.IsFailure) return BadRequest(new { message = result.Error });
        return Ok(new { message = "Tax code deactivated successfully." });
    }
}
