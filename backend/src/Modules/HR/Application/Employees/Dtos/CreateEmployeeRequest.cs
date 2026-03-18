namespace ErpSuite.Modules.HR.Application.Employees.Dtos;

public record CreateEmployeeRequest(
    string EmployeeNumber,
    string FirstName,
    string LastName,
    string? Email = null,
    string? Phone = null,
    long? DepartmentId = null,
    string? Designation = null,
    int Status = 1,
    int EmploymentType = 1,
    DateTime DateOfJoining = default,
    DateTime? DateOfBirth = null,
    long? ManagerId = null,
    string? Notes = null);
