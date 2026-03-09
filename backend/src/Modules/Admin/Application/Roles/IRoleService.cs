using ErpSuite.BuildingBlocks.Domain.Results;
using ErpSuite.Modules.Admin.Application.Roles.Dtos;

namespace ErpSuite.Modules.Admin.Application.Roles;

public interface IRoleService
{
    Task<IReadOnlyList<RoleResponse>> GetRolesAsync(CancellationToken cancellationToken = default);
    Task<RoleDetailResponse?> GetRoleByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<Result<RoleResponse>> CreateRoleAsync(CreateRoleRequest request, string currentUserId, CancellationToken cancellationToken = default);
    Task<Result<RoleResponse>> UpdateRoleAsync(long id, UpdateRoleRequest request, string currentUserId, CancellationToken cancellationToken = default);
    Task<Result> DeleteRoleAsync(long id, CancellationToken cancellationToken = default);
    Task<Result> AssignPermissionsAsync(long roleId, AssignPermissionsRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PermissionResponse>> GetPermissionsAsync(CancellationToken cancellationToken = default);
}
