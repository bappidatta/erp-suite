namespace ErpSuite.Modules.HR.Application.Departments.Dtos;

public record UpdateDepartmentRequest(
    string Name,
    string? Description = null,
    long? ParentDepartmentId = null);
