using ErpSuite.BuildingBlocks.Domain.Entities;

namespace ErpSuite.Modules.Inventory.Domain.Entities;

public class Category : BaseAuditableEntity
{
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public long? ParentCategoryId { get; private set; }
    public bool IsActive { get; private set; } = true;

    private Category() { }

    public static Category Create(string code, string name, string? description, long? parentCategoryId)
    {
        return new Category
        {
            Code = code,
            Name = name,
            Description = description,
            ParentCategoryId = parentCategoryId,
            IsActive = true
        };
    }

    public void Update(string name, string? description, long? parentCategoryId)
    {
        Name = name;
        Description = description;
        ParentCategoryId = parentCategoryId;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}
