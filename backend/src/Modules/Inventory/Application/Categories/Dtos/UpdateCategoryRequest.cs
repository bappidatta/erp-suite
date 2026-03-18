namespace ErpSuite.Modules.Inventory.Application.Categories.Dtos;

public record UpdateCategoryRequest(
    string Name,
    string? Description = null,
    long? ParentCategoryId = null);
