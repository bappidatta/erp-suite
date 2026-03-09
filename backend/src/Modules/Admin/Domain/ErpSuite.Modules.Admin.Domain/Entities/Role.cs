using ErpSuite.BuildingBlocks.Domain.Entities;

namespace ErpSuite.Modules.Admin.Domain.Entities;

public class Role : BaseAuditableEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsSystem { get; private set; }

    private readonly List<User> _users = new();
    public IReadOnlyCollection<User> Users => _users.AsReadOnly();

    private readonly List<RolePermission> _rolePermissions = new();
    public IReadOnlyCollection<RolePermission> RolePermissions => _rolePermissions.AsReadOnly();

    private Role() { } // EF Core

    public static Role Create(string name, string? description = null, bool isSystem = false)
    {
        return new Role
        {
            Name = name,
            Description = description,
            IsSystem = isSystem
        };
    }

    public void Update(string name, string? description)
    {
        Name = name;
        Description = description;
    }
}
