namespace ErpSuite.Modules.Finance.Application.FinancialPeriods.Dtos;

public sealed record UpdateFinancialPeriodRequest(
    string Name,
    DateTime StartDate,
    DateTime EndDate);
