namespace ErpSuite.Modules.Admin.Application.Roles.Dtos;

public record RoleDetailResponse(
    long Id,
    string Name,
    string? Description,
    int UserCount,
    bool IsSystem,
    IReadOnlyList<PermissionResponse> Permissions,
    DateTime CreatedAt);
