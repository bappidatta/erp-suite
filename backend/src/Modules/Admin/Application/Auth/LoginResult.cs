namespace ErpSuite.Modules.Admin.Application.Auth;

public sealed record LoginResult(string AccessToken, DateTime ExpiresAtUtc, AuthUser User, string Jti);
