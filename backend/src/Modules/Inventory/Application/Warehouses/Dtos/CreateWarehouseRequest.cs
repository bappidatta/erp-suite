namespace ErpSuite.Modules.Inventory.Application.Warehouses.Dtos;

public record CreateWarehouseRequest(
    string Code,
    string Name,
    string? Location = null,
    string? Address = null,
    string? ContactPerson = null,
    string? Phone = null,
    string? Notes = null);
