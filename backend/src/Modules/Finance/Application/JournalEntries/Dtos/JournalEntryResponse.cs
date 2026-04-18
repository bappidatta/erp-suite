using ErpSuite.Modules.Finance.Domain.Entities;

namespace ErpSuite.Modules.Finance.Application.JournalEntries.Dtos;

public sealed record JournalEntryResponse(
    long Id,
    string Number,
    DateTime EntryDate,
    string Description,
    string? Reference,
    JournalEntryStatus Status,
    string StatusName,
    DateTime? PostedAt,
    string? PostedBy,
    decimal TotalDebit,
    decimal TotalCredit,
    IReadOnlyList<JournalEntryLineResponse> Lines,
    DateTime CreatedAt,
    string CreatedBy,
    DateTime? UpdatedAt);
