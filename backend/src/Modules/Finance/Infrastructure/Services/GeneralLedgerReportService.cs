using ErpSuite.Modules.Admin.Infrastructure.Persistence;
using ErpSuite.Modules.Finance.Application.Reporting;
using ErpSuite.Modules.Finance.Application.Reporting.Dtos;
using ErpSuite.Modules.Finance.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ErpSuite.Modules.Finance.Infrastructure.Services;

public sealed class GeneralLedgerReportService : IGeneralLedgerReportService
{
    private readonly ErpDbContext _dbContext;

    public GeneralLedgerReportService(ErpDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<TrialBalanceRowResponse>> GetTrialBalanceAsync(GetTrialBalanceQuery query, CancellationToken cancellationToken = default)
    {
        var lines = _dbContext.JournalEntryLines
            .AsNoTracking()
            .Where(x => x.JournalEntry.Status == JournalEntryStatus.Posted)
            .AsQueryable();

        if (query.FromDate.HasValue)
        {
            var from = query.FromDate.Value.Date;
            lines = lines.Where(x => x.JournalEntry.EntryDate >= from);
        }

        if (query.ToDate.HasValue)
        {
            var to = query.ToDate.Value.Date;
            lines = lines.Where(x => x.JournalEntry.EntryDate <= to);
        }

        var result = await lines
            .GroupBy(x => new { x.AccountId, x.Account.Code, x.Account.Name })
            .Select(group => new TrialBalanceRowResponse(
                group.Key.AccountId,
                group.Key.Code,
                group.Key.Name,
                group.Sum(x => x.DebitAmount),
                group.Sum(x => x.CreditAmount),
                group.Sum(x => x.DebitAmount - x.CreditAmount)))
            .OrderBy(x => x.AccountCode)
            .ToListAsync(cancellationToken);

        return result;
    }

    public async Task<IReadOnlyList<LedgerEntryResponse>> GetLedgerEntriesAsync(GetLedgerEntriesQuery query, CancellationToken cancellationToken = default)
    {
        var lines = _dbContext.JournalEntryLines
            .AsNoTracking()
            .Include(x => x.JournalEntry)
            .Where(x => x.AccountId == query.AccountId && x.JournalEntry.Status == JournalEntryStatus.Posted)
            .AsQueryable();

        if (query.FromDate.HasValue)
        {
            var from = query.FromDate.Value.Date;
            lines = lines.Where(x => x.JournalEntry.EntryDate >= from);
        }

        if (query.ToDate.HasValue)
        {
            var to = query.ToDate.Value.Date;
            lines = lines.Where(x => x.JournalEntry.EntryDate <= to);
        }

        var ordered = await lines
            .OrderBy(x => x.JournalEntry.EntryDate)
            .ThenBy(x => x.JournalEntryId)
            .ThenBy(x => x.LineNumber)
            .ToListAsync(cancellationToken);

        decimal runningBalance = 0m;
        return ordered.Select(x =>
        {
            runningBalance += x.DebitAmount - x.CreditAmount;
            return new LedgerEntryResponse(
                x.JournalEntryId,
                x.JournalEntry.Number,
                x.JournalEntry.EntryDate,
                x.JournalEntry.Description,
                x.JournalEntry.Reference,
                x.DebitAmount,
                x.CreditAmount,
                runningBalance);
        }).ToList();
    }
}
