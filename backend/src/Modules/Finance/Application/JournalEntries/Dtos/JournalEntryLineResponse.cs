namespace ErpSuite.Modules.Finance.Application.JournalEntries.Dtos;

public sealed record JournalEntryLineResponse(
    long Id,
    int LineNumber,
    long AccountId,
    string AccountCode,
    string AccountName,
    decimal DebitAmount,
    decimal CreditAmount,
    string? Description);
