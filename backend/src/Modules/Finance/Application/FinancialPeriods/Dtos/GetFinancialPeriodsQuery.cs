using ErpSuite.Modules.Finance.Domain.Entities;

namespace ErpSuite.Modules.Finance.Application.FinancialPeriods.Dtos;

public sealed class GetFinancialPeriodsQuery
{
    public int? Year { get; init; }
    public FinancialPeriodStatus? Status { get; init; }
    public string? SortBy { get; init; }
    public bool SortDescending { get; init; } = true;
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
