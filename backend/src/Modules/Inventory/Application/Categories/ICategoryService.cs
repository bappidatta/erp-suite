using ErpSuite.BuildingBlocks.Application.Common;
using ErpSuite.BuildingBlocks.Domain.Results;
using ErpSuite.Modules.Inventory.Application.Categories.Dtos;

namespace ErpSuite.Modules.Inventory.Application.Categories;

public interface ICategoryService
{
    Task<PagedResult<CategoryResponse>> GetCategoriesAsync(GetCategoriesQuery query, CancellationToken cancellationToken = default);
    Task<CategoryResponse?> GetCategoryByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<Result<CategoryResponse>> CreateCategoryAsync(CreateCategoryRequest request, string currentUserId, CancellationToken cancellationToken = default);
    Task<Result<CategoryResponse>> UpdateCategoryAsync(long id, UpdateCategoryRequest request, string currentUserId, CancellationToken cancellationToken = default);
    Task<Result> DeleteCategoryAsync(long id, string currentUserId, CancellationToken cancellationToken = default);
    Task<Result> ActivateCategoryAsync(long id, string currentUserId, CancellationToken cancellationToken = default);
    Task<Result> DeactivateCategoryAsync(long id, string currentUserId, CancellationToken cancellationToken = default);
}
