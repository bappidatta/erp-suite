using ErpSuite.Modules.Finance.Application.Accounts;
using ErpSuite.Modules.Finance.Application.FinancialPeriods;
using ErpSuite.Modules.Finance.Application.JournalEntries;
using ErpSuite.Modules.Finance.Application.Reporting;
using ErpSuite.Modules.Finance.Application.TaxCodes;
using ErpSuite.Modules.Finance.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ErpSuite.Modules.Finance.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddFinanceInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<ITaxCodeService, TaxCodeService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IJournalEntryService, JournalEntryService>();
        services.AddScoped<IFinancialPeriodService, FinancialPeriodService>();
        services.AddScoped<IGeneralLedgerReportService, GeneralLedgerReportService>();

        return services;
    }
}
