namespace ErpSuite.Modules.Inventory.Application.Categories.Dtos;

public record CreateCategoryRequest(
    string Code,
    string Name,
    string? Description = null,
    long? ParentCategoryId = null);
