using ErpSuite.BuildingBlocks.Application.Common;
using ErpSuite.BuildingBlocks.Domain.Results;
using ErpSuite.Modules.Admin.Infrastructure.Persistence;
using ErpSuite.Modules.Inventory.Application.Categories;
using ErpSuite.Modules.Inventory.Application.Categories.Dtos;
using ErpSuite.Modules.Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ErpSuite.Modules.Inventory.Infrastructure.Services;

public sealed class CategoryService : ICategoryService
{
    private readonly ErpDbContext _dbContext;

    public CategoryService(ErpDbContext dbContext) => _dbContext = dbContext;

    public async Task<PagedResult<CategoryResponse>> GetCategoriesAsync(GetCategoriesQuery query, CancellationToken cancellationToken = default)
    {
        var queryable = _dbContext.Categories.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var term = query.SearchTerm.ToLower();
            queryable = queryable.Where(c => c.Code.ToLower().Contains(term) || c.Name.ToLower().Contains(term));
        }

        if (query.IsActive.HasValue)
            queryable = queryable.Where(c => c.IsActive == query.IsActive.Value);

        queryable = query.SortBy?.ToLower() switch
        {
            "code" => query.SortDescending ? queryable.OrderByDescending(c => c.Code) : queryable.OrderBy(c => c.Code),
            _ => query.SortDescending ? queryable.OrderByDescending(c => c.Name) : queryable.OrderBy(c => c.Name)
        };

        var totalCount = await queryable.CountAsync(cancellationToken);
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var items = await queryable.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

        return new PagedResult<CategoryResponse>(items.Select(MapToResponse).ToList(), totalCount, page, pageSize);
    }

    public async Task<CategoryResponse?> GetCategoryByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var category = await _dbContext.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        return category is null ? null : MapToResponse(category);
    }

    public async Task<Result<CategoryResponse>> CreateCategoryAsync(CreateCategoryRequest request, string currentUserId, CancellationToken cancellationToken = default)
    {
        var normalizedCode = request.Code.Trim().ToUpperInvariant();

        if (await _dbContext.Categories.AnyAsync(c => c.Code.ToUpper() == normalizedCode, cancellationToken))
            return Result.Failure<CategoryResponse>("A category with this code already exists.");

        if (request.ParentCategoryId.HasValue && !await _dbContext.Categories.AnyAsync(c => c.Id == request.ParentCategoryId.Value, cancellationToken))
            return Result.Failure<CategoryResponse>("Parent category not found.");

        var category = Category.Create(normalizedCode, request.Name.Trim(), request.Description?.Trim(), request.ParentCategoryId);
        category.SetAudit(currentUserId);
        _dbContext.Categories.Add(category);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(MapToResponse(category));
    }

    public async Task<Result<CategoryResponse>> UpdateCategoryAsync(long id, UpdateCategoryRequest request, string currentUserId, CancellationToken cancellationToken = default)
    {
        var category = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (category is null) return Result.Failure<CategoryResponse>("Category not found.");

        if (request.ParentCategoryId.HasValue)
        {
            if (request.ParentCategoryId.Value == id)
                return Result.Failure<CategoryResponse>("A category cannot be its own parent.");
            if (!await _dbContext.Categories.AnyAsync(c => c.Id == request.ParentCategoryId.Value, cancellationToken))
                return Result.Failure<CategoryResponse>("Parent category not found.");
        }

        category.Update(request.Name.Trim(), request.Description?.Trim(), request.ParentCategoryId);
        category.SetAudit(currentUserId);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(MapToResponse(category));
    }

    public async Task<Result> DeleteCategoryAsync(long id, string currentUserId, CancellationToken cancellationToken = default)
    {
        var category = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (category is null) return Result.Failure("Category not found.");

        var hasItems = await _dbContext.Items.AnyAsync(i => i.CategoryId == id, cancellationToken);
        if (hasItems) return Result.Failure("Cannot delete a category that has items assigned to it.");

        category.SoftDelete(currentUserId);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> ActivateCategoryAsync(long id, string currentUserId, CancellationToken cancellationToken = default)
    {
        var category = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (category is null) return Result.Failure("Category not found.");
        category.Activate();
        category.SetAudit(currentUserId);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> DeactivateCategoryAsync(long id, string currentUserId, CancellationToken cancellationToken = default)
    {
        var category = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (category is null) return Result.Failure("Category not found.");
        category.Deactivate();
        category.SetAudit(currentUserId);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private static CategoryResponse MapToResponse(Category c) => new(
        c.Id, c.Code, c.Name, c.Description, c.ParentCategoryId,
        c.IsActive, c.CreatedAt, c.CreatedBy, c.UpdatedAt);
}
