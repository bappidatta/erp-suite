using ErpSuite.Modules.Finance.Domain.Entities;

namespace ErpSuite.Modules.Finance.Application.TaxCodes.Dtos;

public record UpdateTaxCodeRequest(
    string Name,
    decimal Rate,
    TaxType Type = TaxType.Percentage,
    string? Description = null,
    bool AppliesToSales = true,
    bool AppliesToPurchases = true);
