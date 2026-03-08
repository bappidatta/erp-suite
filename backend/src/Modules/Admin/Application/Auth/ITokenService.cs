namespace ErpSuite.Modules.Admin.Application.Auth;

public interface ITokenService
{
    LoginResult CreateToken(AuthUser user);
}
