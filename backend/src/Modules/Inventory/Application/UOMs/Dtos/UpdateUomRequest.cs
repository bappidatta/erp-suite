namespace ErpSuite.Modules.Inventory.Application.UOMs.Dtos;

public record UpdateUomRequest(
    string Name,
    string? Description = null);
