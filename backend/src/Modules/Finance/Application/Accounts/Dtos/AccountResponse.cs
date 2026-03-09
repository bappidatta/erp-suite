using ErpSuite.Modules.Finance.Domain.Entities;

namespace ErpSuite.Modules.Finance.Application.Accounts.Dtos;

public record AccountResponse(
    long Id,
    string Code,
    string Name,
    AccountType Type,
    string TypeName,
    string? Description,
    long? ParentId,
    string? ParentName,
    bool IsHeader,
    int Level,
    bool IsActive,
    DateTime CreatedAt,
    string CreatedBy,
    DateTime? UpdatedAt);
