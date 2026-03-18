namespace ErpSuite.Modules.Inventory.Application.UOMs.Dtos;

public class GetUomsQuery
{
    public string? SearchTerm { get; init; }
    public bool? IsActive { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 50;
}
