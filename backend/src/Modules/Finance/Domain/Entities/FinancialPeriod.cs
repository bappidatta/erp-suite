using ErpSuite.BuildingBlocks.Domain.Entities;

namespace ErpSuite.Modules.Finance.Domain.Entities;

public class FinancialPeriod : BaseAuditableEntity
{
    public string Name { get; private set; } = string.Empty;
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public FinancialPeriodStatus Status { get; private set; } = FinancialPeriodStatus.Open;
    public DateTime? ClosedAt { get; private set; }
    public string? ClosedBy { get; private set; }

    private FinancialPeriod() { }

    public static FinancialPeriod Create(string name, DateTime startDate, DateTime endDate)
    {
        return new FinancialPeriod
        {
            Name = name,
            StartDate = startDate.Date,
            EndDate = endDate.Date,
            Status = FinancialPeriodStatus.Open
        };
    }

    public void Update(string name, DateTime startDate, DateTime endDate)
    {
        Name = name;
        StartDate = startDate.Date;
        EndDate = endDate.Date;
    }

    public void Close(string userId)
    {
        Status = FinancialPeriodStatus.Closed;
        ClosedAt = DateTime.UtcNow;
        ClosedBy = userId;
    }

    public void Reopen()
    {
        Status = FinancialPeriodStatus.Open;
        ClosedAt = null;
        ClosedBy = null;
    }
}
