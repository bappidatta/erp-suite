using ErpSuite.BuildingBlocks.Application.Common;
using ErpSuite.BuildingBlocks.Domain.Results;
using ErpSuite.Modules.Admin.Infrastructure.Persistence;
using ErpSuite.Modules.Finance.Application.JournalEntries;
using ErpSuite.Modules.Finance.Application.JournalEntries.Dtos;
using ErpSuite.Modules.Finance.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ErpSuite.Modules.Finance.Infrastructure.Services;

public sealed class JournalEntryService : IJournalEntryService
{
    private const int MaxCreateAttempts = 3;
    private readonly ErpDbContext _dbContext;

    public JournalEntryService(ErpDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<JournalEntryResponse>> GetJournalEntriesAsync(GetJournalEntriesQuery query, CancellationToken cancellationToken = default)
    {
        var queryable = _dbContext.JournalEntries
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var term = query.SearchTerm.Trim().ToLowerInvariant();
            queryable = queryable.Where(x =>
                x.Number.ToLower().Contains(term) ||
                x.Description.ToLower().Contains(term) ||
                (x.Reference != null && x.Reference.ToLower().Contains(term)));
        }

        if (query.Status.HasValue)
        {
            queryable = queryable.Where(x => x.Status == query.Status.Value);
        }

        if (query.FromDate.HasValue)
        {
            var fromDate = query.FromDate.Value.Date;
            queryable = queryable.Where(x => x.EntryDate >= fromDate);
        }

        if (query.ToDate.HasValue)
        {
            var toDate = query.ToDate.Value.Date;
            queryable = queryable.Where(x => x.EntryDate <= toDate);
        }

        queryable = query.SortBy?.ToLowerInvariant() switch
        {
            "number" => query.SortDescending ? queryable.OrderByDescending(x => x.Number) : queryable.OrderBy(x => x.Number),
            "status" => query.SortDescending ? queryable.OrderByDescending(x => x.Status) : queryable.OrderBy(x => x.Status),
            _ => query.SortDescending ? queryable.OrderByDescending(x => x.EntryDate).ThenByDescending(x => x.Id) : queryable.OrderBy(x => x.EntryDate).ThenBy(x => x.Id)
        };

        var totalCount = await queryable.CountAsync(cancellationToken);
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);
        var items = await queryable
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(entry => new
            {
                entry.Id,
                entry.Number,
                entry.EntryDate,
                entry.Description,
                entry.Reference,
                entry.Status,
                entry.PostedAt,
                entry.PostedBy,
                entry.TotalDebit,
                entry.TotalCredit,
                entry.CreatedAt,
                entry.CreatedBy,
                entry.UpdatedAt
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<JournalEntryResponse>(
            items.Select(entry => new JournalEntryResponse(
                entry.Id,
                entry.Number,
                entry.EntryDate,
                entry.Description,
                entry.Reference,
                entry.Status,
                entry.Status.ToString(),
                entry.PostedAt,
                entry.PostedBy,
                entry.TotalDebit,
                entry.TotalCredit,
                Array.Empty<JournalEntryLineResponse>(),
                entry.CreatedAt,
                entry.CreatedBy,
                entry.UpdatedAt)).ToList(),
            totalCount,
            page,
            pageSize);
    }

    public async Task<JournalEntryResponse?> GetJournalEntryByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var entry = await LoadJournalEntryAsync(id, asNoTracking: true, cancellationToken);
        return entry is null ? null : MapToResponse(entry);
    }

    public async Task<Result<JournalEntryResponse>> CreateJournalEntryAsync(CreateJournalEntryRequest request, string currentUserId, CancellationToken cancellationToken = default)
    {
        for (var attempt = 0; attempt < MaxCreateAttempts; attempt++)
        {
            var validation = await ValidateLinesAsync(request.EntryDate, request.Lines, cancellationToken);
            if (validation.IsFailure)
            {
                return Result.Failure<JournalEntryResponse>(validation.Error);
            }

            var entry = JournalEntry.Create(
                await GenerateEntryNumberAsync(currentUserId, cancellationToken),
                request.EntryDate.Date,
                request.Description.Trim(),
                request.Reference?.Trim(),
                validation.Value);

            entry.SetAudit(currentUserId);
            _dbContext.JournalEntries.Add(entry);

            try
            {
                await _dbContext.SaveChangesAsync(cancellationToken);

                var created = await LoadJournalEntryAsync(entry.Id, asNoTracking: true, cancellationToken);
                return Result.Success(MapToResponse(created!));
            }
            catch (DbUpdateConcurrencyException) when (attempt < MaxCreateAttempts - 1)
            {
                _dbContext.ChangeTracker.Clear();
            }
            catch (DbUpdateConcurrencyException)
            {
                _dbContext.ChangeTracker.Clear();
            }
        }

        return Result.Failure<JournalEntryResponse>("Unable to create the journal entry because the number sequence was updated by another request. Please try again.");
    }

    public async Task<Result<JournalEntryResponse>> UpdateJournalEntryAsync(long id, UpdateJournalEntryRequest request, string currentUserId, CancellationToken cancellationToken = default)
    {
        var entry = await LoadJournalEntryAsync(id, asNoTracking: false, cancellationToken);
        if (entry is null)
        {
            return Result.Failure<JournalEntryResponse>("Journal entry not found.");
        }

        if (entry.Status == JournalEntryStatus.Posted)
        {
            return Result.Failure<JournalEntryResponse>("Posted journal entries cannot be edited.");
        }

        var validation = await ValidateLinesAsync(request.EntryDate, request.Lines, cancellationToken);
        if (validation.IsFailure)
        {
            return Result.Failure<JournalEntryResponse>(validation.Error);
        }

        entry.Update(
            request.EntryDate.Date,
            request.Description.Trim(),
            request.Reference?.Trim(),
            validation.Value);
        entry.SetAudit(currentUserId);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var updated = await LoadJournalEntryAsync(entry.Id, asNoTracking: true, cancellationToken);
        return Result.Success(MapToResponse(updated!));
    }

    public async Task<Result> DeleteJournalEntryAsync(long id, string currentUserId, CancellationToken cancellationToken = default)
    {
        var entry = await _dbContext.JournalEntries.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entry is null)
        {
            return Result.Failure("Journal entry not found.");
        }

        if (entry.Status == JournalEntryStatus.Posted)
        {
            return Result.Failure("Posted journal entries cannot be deleted.");
        }

        entry.SoftDelete(currentUserId);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result<JournalEntryResponse>> PostJournalEntryAsync(long id, string currentUserId, CancellationToken cancellationToken = default)
    {
        var entry = await LoadJournalEntryAsync(id, asNoTracking: false, cancellationToken);
        if (entry is null)
        {
            return Result.Failure<JournalEntryResponse>("Journal entry not found.");
        }

        if (entry.Status == JournalEntryStatus.Posted)
        {
            return Result.Failure<JournalEntryResponse>("Journal entry is already posted.");
        }

        var openPeriodExists = await _dbContext.FinancialPeriods.AnyAsync(
            x => x.Status == FinancialPeriodStatus.Open &&
                x.StartDate <= entry.EntryDate &&
                x.EndDate >= entry.EntryDate,
            cancellationToken);

        if (!openPeriodExists)
        {
            return Result.Failure<JournalEntryResponse>("The accounting period for the journal date is closed or not configured.");
        }

        entry.Post(currentUserId, DateTime.UtcNow);
        entry.SetAudit(currentUserId);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var posted = await LoadJournalEntryAsync(id, asNoTracking: true, cancellationToken);
        return Result.Success(MapToResponse(posted!));
    }

    private async Task<Result<List<JournalEntryLine>>> ValidateLinesAsync(DateTime entryDate, IReadOnlyList<JournalEntryLineRequest>? lineRequests, CancellationToken cancellationToken)
    {
        if (lineRequests is null || lineRequests.Count < 2)
        {
            return Result.Failure<List<JournalEntryLine>>("At least two journal lines are required.");
        }

        var accountIds = lineRequests.Select(x => x.AccountId).Distinct().ToList();
        var accounts = await _dbContext.Accounts
            .Where(x => accountIds.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id, cancellationToken);

        if (accounts.Count != accountIds.Count)
        {
            return Result.Failure<List<JournalEntryLine>>("One or more selected accounts do not exist.");
        }

        foreach (var request in lineRequests)
        {
            var account = accounts[request.AccountId];
            if (!account.IsActive)
            {
                return Result.Failure<List<JournalEntryLine>>("Inactive accounts cannot be used in journal entries.");
            }

            if (account.IsHeader)
            {
                return Result.Failure<List<JournalEntryLine>>("Header accounts cannot be used in journal entries.");
            }
        }

        var totalDebit = lineRequests.Sum(x => x.DebitAmount);
        var totalCredit = lineRequests.Sum(x => x.CreditAmount);
        if (totalDebit <= 0m || totalCredit <= 0m || totalDebit != totalCredit)
        {
            return Result.Failure<List<JournalEntryLine>>("Total debit must equal total credit and both must be greater than zero.");
        }

        var lines = lineRequests
            .OrderBy(x => x.LineNumber)
            .Select(x => JournalEntryLine.Create(
                x.LineNumber,
                x.AccountId,
                x.DebitAmount,
                x.CreditAmount,
                x.Description?.Trim()))
            .ToList();

        return Result.Success(lines);
    }

    private async Task<string> GenerateEntryNumberAsync(string currentUserId, CancellationToken cancellationToken)
    {
        var sequence = await _dbContext.NumberSequences.FirstOrDefaultAsync(
            x => x.IsActive &&
                x.Module.ToLower() == "finance" &&
                x.DocumentType.ToLower() == "journalentry",
            cancellationToken);

        if (sequence is null)
        {
            return $"JE-{DateTime.UtcNow:yyyyMMddHHmmssfff}-{Guid.NewGuid():N}"[..35];
        }

        var next = sequence.ConsumeNext(DateTime.UtcNow);
        sequence.SetAudit(currentUserId);
        return next;
    }

    private async Task<JournalEntry?> LoadJournalEntryAsync(long id, bool asNoTracking, CancellationToken cancellationToken)
    {
        IQueryable<JournalEntry> query = _dbContext.JournalEntries;
        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        return await query
            .Include(x => x.Lines)
                .ThenInclude(x => x.Account)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    private static JournalEntryResponse MapToResponse(JournalEntry entry) => new(
        entry.Id,
        entry.Number,
        entry.EntryDate,
        entry.Description,
        entry.Reference,
        entry.Status,
        entry.Status.ToString(),
        entry.PostedAt,
        entry.PostedBy,
        entry.TotalDebit,
        entry.TotalCredit,
        entry.Lines.OrderBy(x => x.LineNumber).Select(x => new JournalEntryLineResponse(
            x.Id,
            x.LineNumber,
            x.AccountId,
            x.Account.Code,
            x.Account.Name,
            x.DebitAmount,
            x.CreditAmount,
            x.Description)).ToList(),
        entry.CreatedAt,
        entry.CreatedBy,
        entry.UpdatedAt);
}
