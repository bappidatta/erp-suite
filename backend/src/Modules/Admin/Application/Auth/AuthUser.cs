namespace ErpSuite.Modules.Admin.Application.Auth;

public sealed record AuthUser(
    long Id,
    string Email,
    string FullName,
    string Role,
    long CompanyId,
    string CompanyName);
