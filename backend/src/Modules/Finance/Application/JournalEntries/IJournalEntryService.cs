using ErpSuite.BuildingBlocks.Application.Common;
using ErpSuite.BuildingBlocks.Domain.Results;
using ErpSuite.Modules.Finance.Application.JournalEntries.Dtos;

namespace ErpSuite.Modules.Finance.Application.JournalEntries;

public interface IJournalEntryService
{
    Task<PagedResult<JournalEntryResponse>> GetJournalEntriesAsync(GetJournalEntriesQuery query, CancellationToken cancellationToken = default);
    Task<JournalEntryResponse?> GetJournalEntryByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<Result<JournalEntryResponse>> CreateJournalEntryAsync(CreateJournalEntryRequest request, string currentUserId, CancellationToken cancellationToken = default);
    Task<Result<JournalEntryResponse>> UpdateJournalEntryAsync(long id, UpdateJournalEntryRequest request, string currentUserId, CancellationToken cancellationToken = default);
    Task<Result> DeleteJournalEntryAsync(long id, string currentUserId, CancellationToken cancellationToken = default);
    Task<Result<JournalEntryResponse>> PostJournalEntryAsync(long id, string currentUserId, CancellationToken cancellationToken = default);
}
