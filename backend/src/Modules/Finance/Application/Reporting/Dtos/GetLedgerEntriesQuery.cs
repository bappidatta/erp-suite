namespace ErpSuite.Modules.Finance.Application.Reporting.Dtos;

public sealed class GetLedgerEntriesQuery
{
    public long AccountId { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
}
