using ErpSuite.Modules.Finance.Domain.Entities;

namespace ErpSuite.Modules.Finance.Application.Accounts.Dtos;

public record AccountTreeNode(
    long Id,
    string Code,
    string Name,
    AccountType Type,
    string TypeName,
    bool IsHeader,
    int Level,
    bool IsActive,
    IReadOnlyList<AccountTreeNode> Children);
