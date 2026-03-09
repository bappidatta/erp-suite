namespace ErpSuite.Modules.Admin.Application.Roles.Dtos;

public record AssignPermissionsRequest(IReadOnlyList<long> PermissionIds);
