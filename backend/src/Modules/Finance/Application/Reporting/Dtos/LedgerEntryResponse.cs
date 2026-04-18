namespace ErpSuite.Modules.Finance.Application.Reporting.Dtos;

public sealed record LedgerEntryResponse(
    long JournalEntryId,
    string JournalNumber,
    DateTime EntryDate,
    string Description,
    string? Reference,
    decimal DebitAmount,
    decimal CreditAmount,
    decimal RunningBalance);
