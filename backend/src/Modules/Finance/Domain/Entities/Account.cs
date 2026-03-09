using ErpSuite.BuildingBlocks.Domain.Entities;

namespace ErpSuite.Modules.Finance.Domain.Entities;

public class Account : BaseAuditableEntity
{
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public AccountType Type { get; private set; }
    public string? Description { get; private set; }
    public long? ParentId { get; private set; }
    public Account? Parent { get; private set; }
    public bool IsActive { get; private set; } = true;
    public bool IsHeader { get; private set; }
    public int Level { get; private set; }

    private readonly List<Account> _children = new();
    public IReadOnlyCollection<Account> Children => _children.AsReadOnly();

    private Account() { } // EF Core

    public static Account Create(string code, string name, AccountType type,
        long? parentId = null, bool isHeader = false, string? description = null, int level = 0)
    {
        return new Account
        {
            Code = code,
            Name = name,
            Type = type,
            ParentId = parentId,
            IsHeader = isHeader,
            Description = description,
            Level = level,
            IsActive = true
        };
    }

    public void Update(string name, AccountType type, string? description,
        long? parentId, bool isHeader, int level)
    {
        Name = name;
        Type = type;
        Description = description;
        ParentId = parentId;
        IsHeader = isHeader;
        Level = level;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}

public enum AccountType
{
    Asset = 0,
    Liability = 1,
    Equity = 2,
    Revenue = 3,
    Expense = 4
}
