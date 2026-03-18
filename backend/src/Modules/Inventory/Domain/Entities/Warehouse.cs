using ErpSuite.BuildingBlocks.Domain.Entities;

namespace ErpSuite.Modules.Inventory.Domain.Entities;

public class Warehouse : BaseAuditableEntity
{
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Location { get; private set; }
    public string? Address { get; private set; }
    public string? ContactPerson { get; private set; }
    public string? Phone { get; private set; }
    public bool IsActive { get; private set; } = true;
    public string? Notes { get; private set; }

    private Warehouse() { }

    public static Warehouse Create(string code, string name, string? location, string? address,
        string? contactPerson, string? phone, string? notes)
    {
        return new Warehouse
        {
            Code = code,
            Name = name,
            Location = location,
            Address = address,
            ContactPerson = contactPerson,
            Phone = phone,
            Notes = notes,
            IsActive = true
        };
    }

    public void Update(string name, string? location, string? address,
        string? contactPerson, string? phone, string? notes)
    {
        Name = name;
        Location = location;
        Address = address;
        ContactPerson = contactPerson;
        Phone = phone;
        Notes = notes;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}
