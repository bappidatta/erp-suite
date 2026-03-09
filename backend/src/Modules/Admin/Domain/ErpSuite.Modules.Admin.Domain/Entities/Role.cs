using ErpSuite.BuildingBlocks.Domain.Entities;

namespace ErpSuite.Modules.Admin.Domain.Entities;

public class Role : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    private readonly List<User> _users = new();
    public IReadOnlyCollection<User> Users => _users.AsReadOnly();

    private Role() { } // EF Core

    public static Role Create(string name, string? description = null)
    {
        return new Role
        {
            Name = name,
            Description = description
        };
    }
}
