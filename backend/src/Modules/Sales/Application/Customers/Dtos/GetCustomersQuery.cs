namespace ErpSuite.Modules.Sales.Application.Customers.Dtos;

public class GetCustomersQuery
{
    public string? SearchTerm { get; init; }
    public bool? IsActive { get; init; }
    public string? Country { get; init; }
    public string? SortBy { get; init; } = "name";
    public bool SortDescending { get; init; } = false;
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
