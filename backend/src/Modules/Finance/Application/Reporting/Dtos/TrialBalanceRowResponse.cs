namespace ErpSuite.Modules.Finance.Application.Reporting.Dtos;

public sealed record TrialBalanceRowResponse(
    long AccountId,
    string AccountCode,
    string AccountName,
    decimal TotalDebit,
    decimal TotalCredit,
    decimal NetBalance);
