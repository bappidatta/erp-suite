using ErpSuite.Modules.Admin.Application.Roles;
using ErpSuite.Modules.Admin.Application.Roles.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Admin;

[ApiController]
[Route("api/admin/roles")]
[Authorize(Policy = "AdminOnly")]
public sealed class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RolesController(IRoleService roleService) => _roleService = roleService;

    [HttpGet]
    public async Task<IActionResult> GetRoles(CancellationToken cancellationToken)
    {
        var roles = await _roleService.GetRolesAsync(cancellationToken);
        return Ok(roles);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetRole(long id, CancellationToken cancellationToken)
    {
        var role = await _roleService.GetRoleByIdAsync(id, cancellationToken);
        return role is null ? NotFound(new { message = "Role not found." }) : Ok(role);
    }

    [HttpPost]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = User.FindFirst("user_id")?.Value ?? "system";
        var result = await _roleService.CreateRoleAsync(request, currentUserId, cancellationToken);
        if (result.IsFailure) return BadRequest(new { message = result.Error });
        return CreatedAtAction(nameof(GetRole), new { id = result.Value.Id }, result.Value);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> UpdateRole(long id, [FromBody] UpdateRoleRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = User.FindFirst("user_id")?.Value ?? "system";
        var result = await _roleService.UpdateRoleAsync(id, request, currentUserId, cancellationToken);
        if (result.IsFailure) return BadRequest(new { message = result.Error });
        return Ok(result.Value);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteRole(long id, CancellationToken cancellationToken)
    {
        var result = await _roleService.DeleteRoleAsync(id, cancellationToken);
        if (result.IsFailure) return BadRequest(new { message = result.Error });
        return NoContent();
    }

    [HttpGet("{id:long}/permissions")]
    public async Task<IActionResult> GetRolePermissions(long id, CancellationToken cancellationToken)
    {
        var role = await _roleService.GetRoleByIdAsync(id, cancellationToken);
        if (role is null) return NotFound(new { message = "Role not found." });
        return Ok(role.Permissions);
    }

    [HttpPost("{id:long}/permissions")]
    public async Task<IActionResult> AssignPermissions(long id, [FromBody] AssignPermissionsRequest request, CancellationToken cancellationToken)
    {
        var result = await _roleService.AssignPermissionsAsync(id, request, cancellationToken);
        if (result.IsFailure) return BadRequest(new { message = result.Error });
        return Ok(new { message = "Permissions assigned successfully." });
    }

    [HttpGet("permissions")]
    public async Task<IActionResult> GetAllPermissions(CancellationToken cancellationToken)
    {
        var permissions = await _roleService.GetPermissionsAsync(cancellationToken);
        return Ok(permissions);
    }
}
