namespace ErpSuite.Modules.Admin.Application.Users.Dtos;

public record CreateUserRequest(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    long RoleId,
    string? Phone = null,
    long? DepartmentId = null,
    long? ManagerId = null,
    bool MustChangePassword = false);
