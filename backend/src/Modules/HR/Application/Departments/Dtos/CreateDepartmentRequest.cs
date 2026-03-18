namespace ErpSuite.Modules.HR.Application.Departments.Dtos;

public record CreateDepartmentRequest(
    string Code,
    string Name,
    string? Description = null,
    long? ParentDepartmentId = null);
