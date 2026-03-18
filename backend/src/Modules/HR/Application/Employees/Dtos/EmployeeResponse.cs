namespace ErpSuite.Modules.HR.Application.Employees.Dtos;

public record EmployeeResponse(
    long Id,
    string EmployeeNumber,
    string FirstName,
    string LastName,
    string FullName,
    string? Email,
    string? Phone,
    long? DepartmentId,
    string? DepartmentName,
    string? Designation,
    int Status,
    string StatusName,
    int EmploymentType,
    string EmploymentTypeName,
    DateTime DateOfJoining,
    DateTime? DateOfBirth,
    long? ManagerId,
    string? Notes,
    DateTime CreatedAt,
    string CreatedBy,
    DateTime? UpdatedAt);
