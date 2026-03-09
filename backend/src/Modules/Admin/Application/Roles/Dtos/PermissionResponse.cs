namespace ErpSuite.Modules.Admin.Application.Roles.Dtos;

public record PermissionResponse(
    long Id,
    string Name,
    string Module,
    string Action,
    string? Description);
