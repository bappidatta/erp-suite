namespace ErpSuite.Modules.HR.Application.Departments.Dtos;

public class GetDepartmentsQuery
{
    public string? SearchTerm { get; init; }
    public bool? IsActive { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 50;
}
