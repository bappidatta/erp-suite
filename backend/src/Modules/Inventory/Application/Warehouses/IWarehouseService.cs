using ErpSuite.BuildingBlocks.Application.Common;
using ErpSuite.BuildingBlocks.Domain.Results;
using ErpSuite.Modules.Inventory.Application.Warehouses.Dtos;

namespace ErpSuite.Modules.Inventory.Application.Warehouses;

public interface IWarehouseService
{
    Task<PagedResult<WarehouseResponse>> GetWarehousesAsync(GetWarehousesQuery query, CancellationToken cancellationToken = default);
    Task<WarehouseResponse?> GetWarehouseByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<Result<WarehouseResponse>> CreateWarehouseAsync(CreateWarehouseRequest request, string currentUserId, CancellationToken cancellationToken = default);
    Task<Result<WarehouseResponse>> UpdateWarehouseAsync(long id, UpdateWarehouseRequest request, string currentUserId, CancellationToken cancellationToken = default);
    Task<Result> DeleteWarehouseAsync(long id, string currentUserId, CancellationToken cancellationToken = default);
    Task<Result> ActivateWarehouseAsync(long id, string currentUserId, CancellationToken cancellationToken = default);
    Task<Result> DeactivateWarehouseAsync(long id, string currentUserId, CancellationToken cancellationToken = default);
}
