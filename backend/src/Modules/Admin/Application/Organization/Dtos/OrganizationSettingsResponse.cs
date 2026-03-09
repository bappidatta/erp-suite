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
