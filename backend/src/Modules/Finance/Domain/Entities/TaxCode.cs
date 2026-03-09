using ErpSuite.BuildingBlocks.Domain.Entities;

namespace ErpSuite.Modules.Finance.Domain.Entities;

public class TaxCode : BaseAuditableEntity
{
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public decimal Rate { get; private set; }
    public TaxType Type { get; private set; } = TaxType.Percentage;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; } = true;

    // Applicability
    public bool AppliesToSales { get; private set; } = true;
    public bool AppliesToPurchases { get; private set; } = true;

    private TaxCode() { } // EF Core

    public static TaxCode Create(string code, string name, decimal rate, TaxType type,
        string? description, bool appliesToSales, bool appliesToPurchases)
    {
        return new TaxCode
        {
            Code = code,
            Name = name,
            Rate = rate,
            Type = type,
            Description = description,
            IsActive = true,
            AppliesToSales = appliesToSales,
            AppliesToPurchases = appliesToPurchases
        };
    }

    public void Update(string name, decimal rate, TaxType type, string? description,
        bool appliesToSales, bool appliesToPurchases)
    {
        Name = name;
        Rate = rate;
        Type = type;
        Description = description;
        AppliesToSales = appliesToSales;
        AppliesToPurchases = appliesToPurchases;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}

public enum TaxType
{
    Percentage = 0,
    Fixed = 1
}
