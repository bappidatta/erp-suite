using ErpSuite.BuildingBlocks.Domain.Entities;

namespace ErpSuite.Modules.Admin.Domain.Entities;

public class Company : BaseAuditableEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty;
    public string TaxId { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public string Address { get; private set; } = string.Empty;
    public string BaseCurrency { get; private set; } = "USD";
    public bool IsActive { get; private set; } = true;

    private Company() { } // EF Core

    public static Company Create(string name, string code, string taxId, string email, string phone)
    {
        return new Company
        {
            Name = name,
            Code = code,
            TaxId = taxId,
            Email = email,
            Phone = phone,
            IsActive = true
        };
    }

    public void Update(string name, string email, string phone, string address)
    {
        Name = name;
        Email = email;
        Phone = phone;
        Address = address;
    }

    public void SetBaseCurrency(string currency)
    {
        BaseCurrency = currency;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}
