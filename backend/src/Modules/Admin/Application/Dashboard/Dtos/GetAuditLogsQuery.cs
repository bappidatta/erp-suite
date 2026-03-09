namespace ErpSuite.Modules.Admin.Application.Dashboard.Dtos;

public class GetAuditLogsQuery
{
    public string? Module { get; init; }
    public long? UserId { get; init; }
    public DateTime? DateFrom { get; init; }
    public DateTime? DateTo { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 50;
}
