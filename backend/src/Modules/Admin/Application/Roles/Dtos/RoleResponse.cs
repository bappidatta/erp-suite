namespace ErpSuite.Modules.Admin.Application.Roles.Dtos;

public record RoleResponse(
    long Id,
    string Name,
    string? Description,
    int UserCount,
    int PermissionCount,
    bool IsSystem,
    DateTime CreatedAt);
