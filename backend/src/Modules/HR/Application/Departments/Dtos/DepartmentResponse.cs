namespace ErpSuite.Modules.HR.Application.Departments.Dtos;

public record DepartmentResponse(
    long Id,
    string Code,
    string Name,
    string? Description,
    long? ParentDepartmentId,
    bool IsActive,
    DateTime CreatedAt,
    string CreatedBy,
    DateTime? UpdatedAt);
