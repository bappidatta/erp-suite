namespace ErpSuite.Modules.HR.Application.Employees.Dtos;

public class GetEmployeesQuery
{
    public string? SearchTerm { get; init; }
    public long? DepartmentId { get; init; }
    public int? Status { get; init; }
    public string? SortBy { get; init; } = "lastName";
    public bool SortDescending { get; init; } = false;
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
