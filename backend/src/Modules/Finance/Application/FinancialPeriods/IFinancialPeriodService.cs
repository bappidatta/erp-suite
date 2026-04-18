using ErpSuite.BuildingBlocks.Application.Common;
using ErpSuite.BuildingBlocks.Domain.Results;
using ErpSuite.Modules.Finance.Application.FinancialPeriods.Dtos;

namespace ErpSuite.Modules.Finance.Application.FinancialPeriods;

public interface IFinancialPeriodService
{
    Task<PagedResult<FinancialPeriodResponse>> GetFinancialPeriodsAsync(GetFinancialPeriodsQuery query, CancellationToken cancellationToken = default);
    Task<FinancialPeriodResponse?> GetFinancialPeriodByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<Result<FinancialPeriodResponse>> CreateFinancialPeriodAsync(CreateFinancialPeriodRequest request, string currentUserId, CancellationToken cancellationToken = default);
    Task<Result<FinancialPeriodResponse>> UpdateFinancialPeriodAsync(long id, UpdateFinancialPeriodRequest request, string currentUserId, CancellationToken cancellationToken = default);
    Task<Result> DeleteFinancialPeriodAsync(long id, string currentUserId, CancellationToken cancellationToken = default);
    Task<Result<FinancialPeriodResponse>> CloseFinancialPeriodAsync(long id, string currentUserId, CancellationToken cancellationToken = default);
    Task<Result<FinancialPeriodResponse>> ReopenFinancialPeriodAsync(long id, string currentUserId, CancellationToken cancellationToken = default);
}
