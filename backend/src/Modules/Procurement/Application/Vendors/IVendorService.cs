using ErpSuite.BuildingBlocks.Domain.Results;
using ErpSuite.BuildingBlocks.Application.Common;
using ErpSuite.Modules.Procurement.Application.Vendors.Dtos;

namespace ErpSuite.Modules.Procurement.Application.Vendors;

public interface IVendorService
{
    Task<PagedResult<VendorResponse>> GetVendorsAsync(GetVendorsQuery query, CancellationToken cancellationToken = default);
    Task<VendorResponse?> GetVendorByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<Result<VendorResponse>> CreateVendorAsync(CreateVendorRequest request, string currentUserId, CancellationToken cancellationToken = default);
    Task<Result<VendorResponse>> UpdateVendorAsync(long id, UpdateVendorRequest request, string currentUserId, CancellationToken cancellationToken = default);
    Task<Result> DeleteVendorAsync(long id, string currentUserId, CancellationToken cancellationToken = default);
    Task<Result> ActivateVendorAsync(long id, string currentUserId, CancellationToken cancellationToken = default);
    Task<Result> DeactivateVendorAsync(long id, string currentUserId, CancellationToken cancellationToken = default);
}
