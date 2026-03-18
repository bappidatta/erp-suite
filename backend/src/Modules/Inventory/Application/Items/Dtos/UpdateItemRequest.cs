namespace ErpSuite.Modules.Inventory.Application.Items.Dtos;

public record UpdateItemRequest(
    string Name,
    string? Description = null,
    long? CategoryId = null,
    long UomId = 0,
    int Type = 1,
    int ValuationMethod = 1,
    decimal StandardCost = 0,
    decimal SalePrice = 0,
    decimal ReorderLevel = 0,
    string? Barcode = null,
    string? Notes = null);
