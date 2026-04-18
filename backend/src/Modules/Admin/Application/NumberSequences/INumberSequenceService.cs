using ErpSuite.BuildingBlocks.Application.Common;
using ErpSuite.BuildingBlocks.Domain.Results;
using ErpSuite.Modules.Admin.Application.NumberSequences.Dtos;

namespace ErpSuite.Modules.Admin.Application.NumberSequences;

public interface INumberSequenceService
{
    Task<PagedResult<NumberSequenceResponse>> GetNumberSequencesAsync(GetNumberSequencesQuery query, CancellationToken cancellationToken = default);
    Task<NumberSequenceResponse?> GetNumberSequenceByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<Result<NumberSequenceResponse>> CreateNumberSequenceAsync(CreateNumberSequenceRequest request, string currentUserId, CancellationToken cancellationToken = default);
    Task<Result<NumberSequenceResponse>> UpdateNumberSequenceAsync(long id, UpdateNumberSequenceRequest request, string currentUserId, CancellationToken cancellationToken = default);
    Task<Result> DeleteNumberSequenceAsync(long id, string currentUserId, CancellationToken cancellationToken = default);
    Task<Result> ActivateNumberSequenceAsync(long id, string currentUserId, CancellationToken cancellationToken = default);
    Task<Result> DeactivateNumberSequenceAsync(long id, string currentUserId, CancellationToken cancellationToken = default);
    Task<Result<string>> GetPreviewAsync(long id, CancellationToken cancellationToken = default);
    Task<Result<string>> GetNextNumberAsync(string module, string documentType, string currentUserId, CancellationToken cancellationToken = default);
}
