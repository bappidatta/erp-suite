using ErpSuite.Modules.Sales.Application.Customers;
using ErpSuite.Modules.Sales.Application.Customers.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Sales;

[ApiController]
[Route("api/sales/customers")]
[Authorize(Policy = "AuthenticatedUser")]
public sealed class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService) => _customerService = customerService;

    [HttpGet]
    public async Task<IActionResult> GetCustomers([FromQuery] GetCustomersQuery query, CancellationToken cancellationToken)
    {
        var result = await _customerService.GetCustomersAsync(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetCustomer(long id, CancellationToken cancellationToken)
    {
        var customer = await _customerService.GetCustomerByIdAsync(id, cancellationToken);
        return customer is null ? NotFound(new { message = "Customer not found." }) : Ok(customer);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = User.FindFirst("user_id")?.Value ?? "system";
        var result = await _customerService.CreateCustomerAsync(request, currentUserId, cancellationToken);
        if (result.IsFailure) return BadRequest(new { message = result.Error });
        return CreatedAtAction(nameof(GetCustomer), new { id = result.Value.Id }, result.Value);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> UpdateCustomer(long id, [FromBody] UpdateCustomerRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = User.FindFirst("user_id")?.Value ?? "system";
        var result = await _customerService.UpdateCustomerAsync(id, request, currentUserId, cancellationToken);
        if (result.IsFailure) return BadRequest(new { message = result.Error });
        return Ok(result.Value);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteCustomer(long id, CancellationToken cancellationToken)
    {
        var currentUserId = User.FindFirst("user_id")?.Value ?? "system";
        var result = await _customerService.DeleteCustomerAsync(id, currentUserId, cancellationToken);
        if (result.IsFailure) return BadRequest(new { message = result.Error });
        return NoContent();
    }

    [HttpPost("{id:long}/activate")]
    public async Task<IActionResult> ActivateCustomer(long id, CancellationToken cancellationToken)
    {
        var currentUserId = User.FindFirst("user_id")?.Value ?? "system";
        var result = await _customerService.ActivateCustomerAsync(id, currentUserId, cancellationToken);
        if (result.IsFailure) return BadRequest(new { message = result.Error });
        return Ok(new { message = "Customer activated successfully." });
    }

    [HttpPost("{id:long}/deactivate")]
    public async Task<IActionResult> DeactivateCustomer(long id, CancellationToken cancellationToken)
    {
        var currentUserId = User.FindFirst("user_id")?.Value ?? "system";
        var result = await _customerService.DeactivateCustomerAsync(id, currentUserId, cancellationToken);
        if (result.IsFailure) return BadRequest(new { message = result.Error });
        return Ok(new { message = "Customer deactivated successfully." });
    }
}
