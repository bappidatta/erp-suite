namespace ErpSuite.Modules.Inventory.Application.UOMs.Dtos;

public record UomResponse(
    long Id,
    string Code,
    string Name,
    string? Description,
    bool IsActive,
    DateTime CreatedAt,
    string CreatedBy,
    DateTime? UpdatedAt);
