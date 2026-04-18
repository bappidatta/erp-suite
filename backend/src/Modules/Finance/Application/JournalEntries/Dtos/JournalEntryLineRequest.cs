namespace ErpSuite.Modules.Finance.Application.JournalEntries.Dtos;

public sealed record JournalEntryLineRequest(
    int LineNumber,
    long AccountId,
    decimal DebitAmount,
    decimal CreditAmount,
    string? Description = null);
