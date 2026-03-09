namespace ErpSuite.Modules.Admin.Application.Roles.Dtos;

public record CreateRoleRequest(string Name, string? Description = null);

public record UpdateRoleRequest(string Name, string? Description = null);

public record AssignPermissionsRequest(IReadOnlyList<long> PermissionIds);
