using ErpSuite.BuildingBlocks.Domain.Entities;

namespace ErpSuite.Modules.Finance.Domain.Entities;

public class JournalEntryLine : BaseEntity
{
    public long JournalEntryId { get; private set; }
    public JournalEntry JournalEntry { get; private set; } = null!;
    public int LineNumber { get; private set; }
    public long AccountId { get; private set; }
    public Account Account { get; private set; } = null!;
    public string? Description { get; private set; }
    public decimal DebitAmount { get; private set; }
    public decimal CreditAmount { get; private set; }

    private JournalEntryLine() { }

    public static JournalEntryLine Create(int lineNumber, long accountId, decimal debitAmount, decimal creditAmount, string? description = null)
    {
        return new JournalEntryLine
        {
            LineNumber = lineNumber,
            AccountId = accountId,
            DebitAmount = debitAmount,
            CreditAmount = creditAmount,
            Description = description
        };
    }
}
