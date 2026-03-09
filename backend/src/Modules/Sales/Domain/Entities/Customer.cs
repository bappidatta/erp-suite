using ErpSuite.BuildingBlocks.Domain.Entities;

namespace ErpSuite.Modules.Sales.Domain.Entities;

public class Customer : BaseAuditableEntity
{
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? ContactPerson { get; private set; }
    public string? Email { get; private set; }
    public string? Phone { get; private set; }
    public string? Website { get; private set; }
    public string? TaxId { get; private set; }

    // Address
    public string? AddressLine1 { get; private set; }
    public string? AddressLine2 { get; private set; }
    public string? City { get; private set; }
    public string? State { get; private set; }
    public string? PostalCode { get; private set; }
    public string? Country { get; private set; }

    // Financial
    public decimal CreditLimit { get; private set; }
    public string Currency { get; private set; } = "USD";
    public string? PaymentTerms { get; private set; }
    public long? DefaultTaxCodeId { get; private set; }

    public bool IsActive { get; private set; } = true;
    public string? Notes { get; private set; }

    private Customer() { } // EF Core

    public static Customer Create(
        string code, string name, string? contactPerson, string? email, string? phone,
        string? website, string? taxId, string? addressLine1, string? addressLine2,
        string? city, string? state, string? postalCode, string? country,
        decimal creditLimit, string currency, string? paymentTerms,
        long? defaultTaxCodeId, string? notes)
    {
        return new Customer
        {
            Code = code,
            Name = name,
            ContactPerson = contactPerson,
            Email = email,
            Phone = phone,
            Website = website,
            TaxId = taxId,
            AddressLine1 = addressLine1,
            AddressLine2 = addressLine2,
            City = city,
            State = state,
            PostalCode = postalCode,
            Country = country,
            CreditLimit = creditLimit,
            Currency = currency,
            PaymentTerms = paymentTerms,
            DefaultTaxCodeId = defaultTaxCodeId,
            Notes = notes,
            IsActive = true
        };
    }

    public void Update(
        string name, string? contactPerson, string? email, string? phone, string? website,
        string? taxId, string? addressLine1, string? addressLine2, string? city,
        string? state, string? postalCode, string? country, decimal creditLimit,
        string currency, string? paymentTerms, long? defaultTaxCodeId, string? notes)
    {
        Name = name;
        ContactPerson = contactPerson;
        Email = email;
        Phone = phone;
        Website = website;
        TaxId = taxId;
        AddressLine1 = addressLine1;
        AddressLine2 = addressLine2;
        City = city;
        State = state;
        PostalCode = postalCode;
        Country = country;
        CreditLimit = creditLimit;
        Currency = currency;
        PaymentTerms = paymentTerms;
        DefaultTaxCodeId = defaultTaxCodeId;
        Notes = notes;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}
