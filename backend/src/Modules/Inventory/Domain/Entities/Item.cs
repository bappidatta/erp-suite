using ErpSuite.BuildingBlocks.Domain.Entities;

namespace ErpSuite.Modules.Inventory.Domain.Entities;

public enum ItemType { Product = 1, Service = 2, RawMaterial = 3, SemiFinished = 4 }
public enum ValuationMethod { WeightedAverage = 1, FIFO = 2, StandardCost = 3 }

public class Item : BaseAuditableEntity
{
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public long? CategoryId { get; private set; }
    public long UomId { get; private set; }
    public ItemType Type { get; private set; } = ItemType.Product;
    public ValuationMethod ValuationMethod { get; private set; } = ValuationMethod.WeightedAverage;
    public decimal StandardCost { get; private set; }
    public decimal SalePrice { get; private set; }
    public decimal ReorderLevel { get; private set; }
    public string? Barcode { get; private set; }
    public bool IsActive { get; private set; } = true;
    public string? Notes { get; private set; }
    public string? ImagePath { get; private set; }

    // Navigation properties
    public Category? Category { get; private set; }
    public UnitOfMeasure? Uom { get; private set; }

    private Item() { }

    public static Item Create(string code, string name, string? description,
        long? categoryId, long uomId, ItemType type, ValuationMethod valuationMethod,
        decimal standardCost, decimal salePrice, decimal reorderLevel,
        string? barcode, string? notes)
    {
        return new Item
        {
            Code = code,
            Name = name,
            Description = description,
            CategoryId = categoryId,
            UomId = uomId,
            Type = type,
            ValuationMethod = valuationMethod,
            StandardCost = standardCost,
            SalePrice = salePrice,
            ReorderLevel = reorderLevel,
            Barcode = barcode,
            Notes = notes,
            IsActive = true
        };
    }

    public void Update(string name, string? description, long? categoryId, long uomId,
        ItemType type, ValuationMethod valuationMethod, decimal standardCost,
        decimal salePrice, decimal reorderLevel, string? barcode, string? notes)
    {
        Name = name;
        Description = description;
        CategoryId = categoryId;
        UomId = uomId;
        Type = type;
        ValuationMethod = valuationMethod;
        StandardCost = standardCost;
        SalePrice = salePrice;
        ReorderLevel = reorderLevel;
        Barcode = barcode;
        Notes = notes;
    }

    public void SetImagePath(string? imagePath) => ImagePath = imagePath;
    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}
