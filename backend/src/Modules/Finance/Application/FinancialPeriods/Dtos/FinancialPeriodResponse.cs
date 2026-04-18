using ErpSuite.Modules.Finance.Domain.Entities;

namespace ErpSuite.Modules.Finance.Application.FinancialPeriods.Dtos;

public sealed record FinancialPeriodResponse(
    long Id,
    string Name,
    DateTime StartDate,
    DateTime EndDate,
    FinancialPeriodStatus Status,
    string StatusName,
    DateTime? ClosedAt,
    string? ClosedBy,
    DateTime CreatedAt,
    string CreatedBy,
    DateTime? UpdatedAt);
