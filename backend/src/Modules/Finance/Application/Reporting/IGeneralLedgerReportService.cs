using ErpSuite.Modules.Finance.Application.Reporting.Dtos;

namespace ErpSuite.Modules.Finance.Application.Reporting;

public interface IGeneralLedgerReportService
{
    Task<IReadOnlyList<TrialBalanceRowResponse>> GetTrialBalanceAsync(GetTrialBalanceQuery query, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LedgerEntryResponse>> GetLedgerEntriesAsync(GetLedgerEntriesQuery query, CancellationToken cancellationToken = default);
}
