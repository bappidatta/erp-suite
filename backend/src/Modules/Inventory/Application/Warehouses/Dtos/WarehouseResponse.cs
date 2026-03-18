namespace ErpSuite.Modules.Inventory.Application.Warehouses.Dtos;

public record WarehouseResponse(
    long Id,
    string Code,
    string Name,
    string? Location,
    string? Address,
    string? ContactPerson,
    string? Phone,
    bool IsActive,
    string? Notes,
    DateTime CreatedAt,
    string CreatedBy,
    DateTime? UpdatedAt);
