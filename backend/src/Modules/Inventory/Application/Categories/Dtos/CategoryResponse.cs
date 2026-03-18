namespace ErpSuite.Modules.Inventory.Application.Categories.Dtos;

public record CategoryResponse(
    long Id,
    string Code,
    string Name,
    string? Description,
    long? ParentCategoryId,
    bool IsActive,
    DateTime CreatedAt,
    string CreatedBy,
    DateTime? UpdatedAt);
