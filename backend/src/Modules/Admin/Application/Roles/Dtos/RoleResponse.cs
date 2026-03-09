namespace ErpSuite.Modules.Admin.Application.Roles.Dtos;

public record RoleResponse(
    long Id,
    string Name,
    string? Description,
    int UserCount,
    int PermissionCount,
    bool IsSystem,
    DateTime CreatedAt);

public record RoleDetailResponse(
    long Id,
    string Name,
    string? Description,
    int UserCount,
    bool IsSystem,
    IReadOnlyList<PermissionResponse> Permissions,
    DateTime CreatedAt);

public record PermissionResponse(
    long Id,
    string Name,
    string Module,
    string Action,
    string? Description);
