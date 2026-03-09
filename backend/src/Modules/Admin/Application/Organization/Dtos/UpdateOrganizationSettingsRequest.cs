namespace ErpSuite.Modules.Admin.Application.Organization.Dtos;

public record UpdateOrganizationSettingsRequest(
    string CompanyName,
    string? LegalName = null,
    string? RegistrationNumber = null,
    string? Address = null,
    string? Phone = null,
    string? Email = null,
    string? Website = null,
    string Currency = "USD",
    string? FiscalYearStart = null,
    string? DateFormat = null,
    string? TimeZone = null);
