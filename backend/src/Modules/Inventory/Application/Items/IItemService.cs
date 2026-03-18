using ErpSuite.BuildingBlocks.Application.Common;
using ErpSuite.BuildingBlocks.Domain.Results;
using ErpSuite.Modules.Inventory.Application.Items.Dtos;

namespace ErpSuite.Modules.Inventory.Application.Items;

public interface IItemService
{
    Task<PagedResult<ItemResponse>> GetItemsAsync(GetItemsQuery query, CancellationToken cancellationToken = default);
    Task<ItemResponse?> GetItemByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<Result<ItemResponse>> CreateItemAsync(CreateItemRequest request, string currentUserId, CancellationToken cancellationToken = default);
    Task<Result<ItemResponse>> UpdateItemAsync(long id, UpdateItemRequest request, string currentUserId, CancellationToken cancellationToken = default);
    Task<Result> DeleteItemAsync(long id, string currentUserId, CancellationToken cancellationToken = default);
    Task<Result> ActivateItemAsync(long id, string currentUserId, CancellationToken cancellationToken = default);
    Task<Result> DeactivateItemAsync(long id, string currentUserId, CancellationToken cancellationToken = default);
}
