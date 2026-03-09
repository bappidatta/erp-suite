namespace ErpSuite.Modules.Admin.Application.Auth;

public sealed record AuthResponse(DateTime ExpiresAtUtc, AuthUser User);
