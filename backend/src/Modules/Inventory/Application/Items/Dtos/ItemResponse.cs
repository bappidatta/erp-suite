using ErpSuite.Modules.Inventory.Domain.Entities;

namespace ErpSuite.Modules.Inventory.Application.Items.Dtos;

public record ItemResponse(
    long Id,
    string Code,
    string Name,
    string? Description,
    long? CategoryId,
    string? CategoryName,
    long UomId,
    string? UomCode,
    int Type,
    string TypeName,
    int ValuationMethod,
    string ValuationMethodName,
    decimal StandardCost,
    decimal SalePrice,
    decimal ReorderLevel,
    string? Barcode,
    bool IsActive,
    string? Notes,
    string? ImagePath,
    DateTime CreatedAt,
    string CreatedBy,
    DateTime? UpdatedAt);
