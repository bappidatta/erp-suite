using ErpSuite.Modules.Admin.Domain.Entities;

namespace ErpSuite.Modules.Admin.Application.Users.Dtos;

public class GetUsersQuery
{
    public string? SearchTerm { get; init; }
    public long? RoleId { get; init; }
    public long? DepartmentId { get; init; }
    public UserStatus? Status { get; init; }
    public string? SortBy { get; init; } = "fullName";
    public bool SortDescending { get; init; } = false;
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
