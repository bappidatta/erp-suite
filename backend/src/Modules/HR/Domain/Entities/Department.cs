using ErpSuite.BuildingBlocks.Domain.Entities;

namespace ErpSuite.Modules.HR.Domain.Entities;

public class Department : BaseAuditableEntity
{
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public long? ParentDepartmentId { get; private set; }
    public bool IsActive { get; private set; } = true;

    private Department() { }

    public static Department Create(string code, string name, string? description, long? parentDepartmentId)
    {
        return new Department
        {
            Code = code,
            Name = name,
            Description = description,
            ParentDepartmentId = parentDepartmentId,
            IsActive = true
        };
    }

    public void Update(string name, string? description, long? parentDepartmentId)
    {
        Name = name;
        Description = description;
        ParentDepartmentId = parentDepartmentId;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}
