using ErpSuite.Modules.Finance.Domain.Entities;

namespace ErpSuite.Modules.Finance.Application.TaxCodes.Dtos;

public record TaxCodeResponse(
    long Id,
    string Code,
    string Name,
    decimal Rate,
    TaxType Type,
    string TypeName,
    string? Description,
    bool AppliesToSales,
    bool AppliesToPurchases,
    bool IsActive,
    DateTime CreatedAt,
    string CreatedBy,
    DateTime? UpdatedAt);
