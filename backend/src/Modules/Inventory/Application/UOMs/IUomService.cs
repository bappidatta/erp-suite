using ErpSuite.BuildingBlocks.Application.Common;
using ErpSuite.BuildingBlocks.Domain.Results;
using ErpSuite.Modules.Inventory.Application.UOMs.Dtos;

namespace ErpSuite.Modules.Inventory.Application.UOMs;

public interface IUomService
{
    Task<PagedResult<UomResponse>> GetUomsAsync(GetUomsQuery query, CancellationToken cancellationToken = default);
    Task<UomResponse?> GetUomByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<Result<UomResponse>> CreateUomAsync(CreateUomRequest request, string currentUserId, CancellationToken cancellationToken = default);
    Task<Result<UomResponse>> UpdateUomAsync(long id, UpdateUomRequest request, string currentUserId, CancellationToken cancellationToken = default);
    Task<Result> DeleteUomAsync(long id, string currentUserId, CancellationToken cancellationToken = default);
    Task<Result> ActivateUomAsync(long id, string currentUserId, CancellationToken cancellationToken = default);
    Task<Result> DeactivateUomAsync(long id, string currentUserId, CancellationToken cancellationToken = default);
}
