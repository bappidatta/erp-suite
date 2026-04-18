namespace ErpSuite.Modules.Finance.Application.Reporting.Dtos;

public sealed class GetTrialBalanceQuery
{
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
}
