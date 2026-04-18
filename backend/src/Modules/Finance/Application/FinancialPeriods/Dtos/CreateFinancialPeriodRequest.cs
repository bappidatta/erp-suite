namespace ErpSuite.Modules.Finance.Application.FinancialPeriods.Dtos;

public sealed record CreateFinancialPeriodRequest(
    string Name,
    DateTime StartDate,
    DateTime EndDate);
