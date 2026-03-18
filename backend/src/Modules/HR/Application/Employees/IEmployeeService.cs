using ErpSuite.BuildingBlocks.Application.Common;
using ErpSuite.BuildingBlocks.Domain.Results;
using ErpSuite.Modules.HR.Application.Employees.Dtos;

namespace ErpSuite.Modules.HR.Application.Employees;

public interface IEmployeeService
{
    Task<PagedResult<EmployeeResponse>> GetEmployeesAsync(GetEmployeesQuery query, CancellationToken cancellationToken = default);
    Task<EmployeeResponse?> GetEmployeeByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<Result<EmployeeResponse>> CreateEmployeeAsync(CreateEmployeeRequest request, string currentUserId, CancellationToken cancellationToken = default);
    Task<Result<EmployeeResponse>> UpdateEmployeeAsync(long id, UpdateEmployeeRequest request, string currentUserId, CancellationToken cancellationToken = default);
    Task<Result> DeleteEmployeeAsync(long id, string currentUserId, CancellationToken cancellationToken = default);
}
