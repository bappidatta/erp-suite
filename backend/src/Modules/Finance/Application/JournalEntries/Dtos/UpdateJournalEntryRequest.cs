namespace ErpSuite.Modules.Finance.Application.JournalEntries.Dtos;

public sealed record UpdateJournalEntryRequest(
    DateTime EntryDate,
    string Description,
    string? Reference = null,
    IReadOnlyList<JournalEntryLineRequest>? Lines = null);
