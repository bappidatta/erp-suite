using ErpSuite.BuildingBlocks.Application.Common;
using ErpSuite.BuildingBlocks.Domain.Results;
using ErpSuite.Modules.HR.Application.Departments.Dtos;

namespace ErpSuite.Modules.HR.Application.Departments;

public interface IDepartmentService
{
    Task<PagedResult<DepartmentResponse>> GetDepartmentsAsync(GetDepartmentsQuery query, CancellationToken cancellationToken = default);
    Task<DepartmentResponse?> GetDepartmentByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<Result<DepartmentResponse>> CreateDepartmentAsync(CreateDepartmentRequest request, string currentUserId, CancellationToken cancellationToken = default);
    Task<Result<DepartmentResponse>> UpdateDepartmentAsync(long id, UpdateDepartmentRequest request, string currentUserId, CancellationToken cancellationToken = default);
    Task<Result> DeleteDepartmentAsync(long id, string currentUserId, CancellationToken cancellationToken = default);
    Task<Result> ActivateDepartmentAsync(long id, string currentUserId, CancellationToken cancellationToken = default);
    Task<Result> DeactivateDepartmentAsync(long id, string currentUserId, CancellationToken cancellationToken = default);
}
