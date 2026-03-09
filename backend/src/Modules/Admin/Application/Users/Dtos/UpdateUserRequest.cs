using ErpSuite.Modules.Admin.Domain.Entities;

namespace ErpSuite.Modules.Admin.Application.Users.Dtos;

public record UpdateUserRequest(
    string FirstName,
    string LastName,
    long RoleId,
    string? Phone = null,
    long? DepartmentId = null,
    long? ManagerId = null,
    UserStatus? Status = null);
