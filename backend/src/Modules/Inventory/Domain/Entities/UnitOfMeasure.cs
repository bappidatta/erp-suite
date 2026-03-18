using ErpSuite.BuildingBlocks.Domain.Entities;

namespace ErpSuite.Modules.Inventory.Domain.Entities;

public class UnitOfMeasure : BaseAuditableEntity
{
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; } = true;

    private UnitOfMeasure() { }

    public static UnitOfMeasure Create(string code, string name, string? description)
    {
        return new UnitOfMeasure
        {
            Code = code,
            Name = name,
            Description = description,
            IsActive = true
        };
    }

    public void Update(string name, string? description)
    {
        Name = name;
        Description = description;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}
