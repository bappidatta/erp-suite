namespace ErpSuite.Modules.Admin.Application.Organization.Dtos;

public record OrganizationSettingsResponse(
    long Id,
    string CompanyName,
    string? LegalName,
    string? RegistrationNumber,
    string? Address,
    string? Phone,
    string? Email,
    string? Website,
    string? LogoPath,
    string Currency,
    string? FiscalYearStart,
    string? DateFormat,
    string? TimeZone,
    DateTime? UpdatedAt);

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
