using ErpSuite.BuildingBlocks.Domain.Entities;

namespace ErpSuite.Modules.Admin.Domain.Entities;

public class RolePermission : BaseEntity
{
    public long RoleId { get; private set; }
    public Role Role { get; private set; } = null!;
    public long PermissionId { get; private set; }
    public Permission Permission { get; private set; } = null!;

    private RolePermission() { } // EF Core

    public static RolePermission Create(long roleId, long permissionId)
    {
        return new RolePermission { RoleId = roleId, PermissionId = permissionId };
    }
}
