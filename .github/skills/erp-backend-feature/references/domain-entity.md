# Domain Entity Template

```csharp
using ErpSuite.BuildingBlocks.Domain.Entities;

namespace ErpSuite.Modules.{Module}.Domain.Entities;

public class {Entity} : BaseAuditableEntity
{
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    // Add entity-specific properties with private setters
    public bool IsActive { get; private set; } = true;

    private {Entity}() { } // EF Core

    public static {Entity} Create(string code, string name /*, other required params */)
    {
        return new {Entity}
        {
            Code = code,
            Name = name,
            IsActive = true
        };
    }

    public void Update(string name /*, other mutable params */)
    {
        Name = name;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}
```

## Rules

- Every property uses `private set`
- No public constructor — only `private {Entity}() { }` for EF Core
- Creation goes through `static Create()` factory method
- Mutations go through named methods (`Update`, `Activate`, `ChangeStatus`, etc.)
- `Code` fields are typically string identifiers (normalized to uppercase in the service layer)
- Navigation properties use `null!` default: `public ParentEntity Parent { get; private set; } = null!;`
- Collections use private backing field:
  ```csharp
  private readonly List<ChildEntity> _children = new();
  public IReadOnlyCollection<ChildEntity> Children => _children.AsReadOnly();
  ```
