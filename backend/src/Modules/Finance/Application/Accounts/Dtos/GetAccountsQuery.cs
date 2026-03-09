using ErpSuite.Modules.Finance.Domain.Entities;

namespace ErpSuite.Modules.Finance.Application.Accounts.Dtos;

public class GetAccountsQuery
{
    public string? SearchTerm { get; init; }
    public AccountType? Type { get; init; }
    public bool? IsActive { get; init; }
    public bool? IsHeader { get; init; }
    public long? ParentId { get; init; }
    public string? SortBy { get; init; } = "code";
    public bool SortDescending { get; init; } = false;
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 50;
}
