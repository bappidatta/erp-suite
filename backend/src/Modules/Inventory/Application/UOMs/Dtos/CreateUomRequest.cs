namespace ErpSuite.Modules.Inventory.Application.UOMs.Dtos;

public record CreateUomRequest(
    string Code,
    string Name,
    string? Description = null);
