using ErpSuite.BuildingBlocks.Domain.Entities;

namespace ErpSuite.Modules.Finance.Domain.Entities;

public class JournalEntry : BaseAuditableEntity
{
    public string Number { get; private set; } = string.Empty;
    public DateTime EntryDate { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public string? Reference { get; private set; }
    public JournalEntryStatus Status { get; private set; } = JournalEntryStatus.Draft;
    public DateTime? PostedAt { get; private set; }
    public string? PostedBy { get; private set; }
    public decimal TotalDebit { get; private set; }
    public decimal TotalCredit { get; private set; }

    private readonly List<JournalEntryLine> _lines = [];
    public IReadOnlyCollection<JournalEntryLine> Lines => _lines.AsReadOnly();

    private JournalEntry() { }

    public static JournalEntry Create(
        string number,
        DateTime entryDate,
        string description,
        string? reference,
        IEnumerable<JournalEntryLine> lines)
    {
        var entry = new JournalEntry
        {
            Number = number,
            EntryDate = entryDate,
            Description = description,
            Reference = reference,
            Status = JournalEntryStatus.Draft
        };

        entry.ReplaceLines(lines);
        return entry;
    }

    public void Update(DateTime entryDate, string description, string? reference, IEnumerable<JournalEntryLine> lines)
    {
        EntryDate = entryDate;
        Description = description;
        Reference = reference;
        ReplaceLines(lines);
    }

    public void Post(string postedBy, DateTime postedAt)
    {
        Status = JournalEntryStatus.Posted;
        PostedBy = postedBy;
        PostedAt = postedAt;
    }

    public void ReplaceLines(IEnumerable<JournalEntryLine> lines)
    {
        _lines.Clear();
        _lines.AddRange(lines.OrderBy(l => l.LineNumber));
        TotalDebit = _lines.Sum(l => l.DebitAmount);
        TotalCredit = _lines.Sum(l => l.CreditAmount);
    }
}
