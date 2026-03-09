using ErpSuite.Modules.Admin.Domain.Entities;

namespace ErpSuite.Modules.Admin.Application.Users.Dtos;

public record UserResponse(
    long Id,
    string Email,
    string FirstName,
    string LastName,
    string FullName,
    string? Phone,
    UserStatus Status,
    string StatusName,
    long RoleId,
    string RoleName,
    long? DepartmentId,
    long? ManagerId,
    bool MustChangePassword,
    DateTime? LastLoginAt,
    DateTime CreatedAt,
    string CreatedBy,
    DateTime? UpdatedAt);
