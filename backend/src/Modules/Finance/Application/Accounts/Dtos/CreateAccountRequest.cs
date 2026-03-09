using ErpSuite.Modules.Finance.Domain.Entities;

namespace ErpSuite.Modules.Finance.Application.Accounts.Dtos;

public record CreateAccountRequest(
    string Code,
    string Name,
    AccountType Type,
    long? ParentId = null,
    bool IsHeader = false,
    string? Description = null);
