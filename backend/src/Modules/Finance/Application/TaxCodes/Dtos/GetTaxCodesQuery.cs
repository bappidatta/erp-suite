namespace ErpSuite.Modules.Finance.Application.TaxCodes.Dtos;

public class GetTaxCodesQuery
{
    public string? SearchTerm { get; init; }
    public bool? IsActive { get; init; }
    public bool? AppliesToSales { get; init; }
    public bool? AppliesToPurchases { get; init; }
    public string? SortBy { get; init; } = "code";
    public bool SortDescending { get; init; } = false;
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
