namespace ErpSuite.Modules.Sales.Application.Customers.Dtos;

public record CreateCustomerRequest(
    string Code,
    string Name,
    string? ContactPerson = null,
    string? Email = null,
    string? Phone = null,
    string? Website = null,
    string? TaxId = null,
    string? AddressLine1 = null,
    string? AddressLine2 = null,
    string? City = null,
    string? State = null,
    string? PostalCode = null,
    string? Country = null,
    decimal CreditLimit = 0,
    string Currency = "USD",
    string? PaymentTerms = null,
    long? DefaultTaxCodeId = null,
    string? Notes = null);
