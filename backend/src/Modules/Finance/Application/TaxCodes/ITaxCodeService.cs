using ErpSuite.BuildingBlocks.Domain.Results;
using ErpSuite.BuildingBlocks.Application.Common;
using ErpSuite.Modules.Finance.Application.TaxCodes.Dtos;

namespace ErpSuite.Modules.Finance.Application.TaxCodes;

public interface ITaxCodeService
{
    Task<PagedResult<TaxCodeResponse>> GetTaxCodesAsync(GetTaxCodesQuery query, CancellationToken cancellationToken = default);
    Task<TaxCodeResponse?> GetTaxCodeByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<Result<TaxCodeResponse>> CreateTaxCodeAsync(CreateTaxCodeRequest request, string currentUserId, CancellationToken cancellationToken = default);
    Task<Result<TaxCodeResponse>> UpdateTaxCodeAsync(long id, UpdateTaxCodeRequest request, string currentUserId, CancellationToken cancellationToken = default);
    Task<Result> DeleteTaxCodeAsync(long id, string currentUserId, CancellationToken cancellationToken = default);
    Task<Result> ActivateTaxCodeAsync(long id, string currentUserId, CancellationToken cancellationToken = default);
    Task<Result> DeactivateTaxCodeAsync(long id, string currentUserId, CancellationToken cancellationToken = default);
}
