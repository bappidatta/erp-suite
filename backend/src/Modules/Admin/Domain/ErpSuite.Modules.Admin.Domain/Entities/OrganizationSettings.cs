using ErpSuite.BuildingBlocks.Domain.Entities;

namespace ErpSuite.Modules.Admin.Domain.Entities;

public class OrganizationSettings : BaseAuditableEntity
{
    public string CompanyName { get; private set; } = string.Empty;
    public string? LegalName { get; private set; }
    public string? RegistrationNumber { get; private set; }
    public string? Address { get; private set; }
    public string? Phone { get; private set; }
    public string? Email { get; private set; }
    public string? Website { get; private set; }
    public string? LogoPath { get; private set; }
    public string Currency { get; private set; } = "USD";
    public string? FiscalYearStart { get; private set; }
    public string? DateFormat { get; private set; }
    public string? TimeZone { get; private set; }

    private OrganizationSettings() { } // EF Core

    public static OrganizationSettings Create(string companyName)
    {
        return new OrganizationSettings { CompanyName = companyName };
    }

    public void Update(
        string companyName,
        string? legalName,
        string? registrationNumber,
        string? address,
        string? phone,
        string? email,
        string? website,
        string currency,
        string? fiscalYearStart,
        string? dateFormat,
        string? timeZone)
    {
        CompanyName = companyName;
        LegalName = legalName;
        RegistrationNumber = registrationNumber;
        Address = address;
        Phone = phone;
        Email = email;
        Website = website;
        Currency = currency;
        FiscalYearStart = fiscalYearStart;
        DateFormat = dateFormat;
        TimeZone = timeZone;
    }

    public void SetLogoPath(string? logoPath) => LogoPath = logoPath;
}
