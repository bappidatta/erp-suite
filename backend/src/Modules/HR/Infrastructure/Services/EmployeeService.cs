using ErpSuite.BuildingBlocks.Application.Common;
using ErpSuite.BuildingBlocks.Domain.Results;
using ErpSuite.Modules.Admin.Infrastructure.Persistence;
using ErpSuite.Modules.HR.Application.Employees;
using ErpSuite.Modules.HR.Application.Employees.Dtos;
using ErpSuite.Modules.HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ErpSuite.Modules.HR.Infrastructure.Services;

public sealed class EmployeeService : IEmployeeService
{
    private readonly ErpDbContext _dbContext;

    public EmployeeService(ErpDbContext dbContext) => _dbContext = dbContext;

    public async Task<PagedResult<EmployeeResponse>> GetEmployeesAsync(GetEmployeesQuery query, CancellationToken cancellationToken = default)
    {
        var queryable = _dbContext.Employees
            .AsNoTracking()
            .Include(e => e.Department)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var term = query.SearchTerm.ToLower();
            queryable = queryable.Where(e =>
                e.EmployeeNumber.ToLower().Contains(term) ||
                e.FirstName.ToLower().Contains(term) ||
                e.LastName.ToLower().Contains(term) ||
                (e.Email != null && e.Email.ToLower().Contains(term)));
        }

        if (query.DepartmentId.HasValue)
            queryable = queryable.Where(e => e.DepartmentId == query.DepartmentId.Value);

        if (query.Status.HasValue)
            queryable = queryable.Where(e => (int)e.Status == query.Status.Value);

        queryable = query.SortBy?.ToLower() switch
        {
            "employeenumber" => query.SortDescending ? queryable.OrderByDescending(e => e.EmployeeNumber) : queryable.OrderBy(e => e.EmployeeNumber),
            "firstname" => query.SortDescending ? queryable.OrderByDescending(e => e.FirstName) : queryable.OrderBy(e => e.FirstName),
            _ => query.SortDescending ? queryable.OrderByDescending(e => e.LastName) : queryable.OrderBy(e => e.LastName)
        };

        var totalCount = await queryable.CountAsync(cancellationToken);
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var items = await queryable.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

        return new PagedResult<EmployeeResponse>(items.Select(MapToResponse).ToList(), totalCount, page, pageSize);
    }

    public async Task<EmployeeResponse?> GetEmployeeByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var employee = await _dbContext.Employees
            .AsNoTracking()
            .Include(e => e.Department)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        return employee is null ? null : MapToResponse(employee);
    }

    public async Task<Result<EmployeeResponse>> CreateEmployeeAsync(CreateEmployeeRequest request, string currentUserId, CancellationToken cancellationToken = default)
    {
        var normalizedNumber = request.EmployeeNumber.Trim().ToUpperInvariant();

        if (await _dbContext.Employees.AnyAsync(e => e.EmployeeNumber.ToUpper() == normalizedNumber, cancellationToken))
            return Result.Failure<EmployeeResponse>("An employee with this number already exists.");

        if (request.DepartmentId.HasValue && !await _dbContext.Departments.AnyAsync(d => d.Id == request.DepartmentId.Value, cancellationToken))
            return Result.Failure<EmployeeResponse>("The specified department does not exist.");

        if (request.ManagerId.HasValue && !await _dbContext.Employees.AnyAsync(e => e.Id == request.ManagerId.Value, cancellationToken))
            return Result.Failure<EmployeeResponse>("The specified manager does not exist.");

        var joinDate = request.DateOfJoining == default ? DateTime.Today : request.DateOfJoining;

        var employee = Employee.Create(normalizedNumber, request.FirstName.Trim(), request.LastName.Trim(),
            request.Email?.Trim().ToLowerInvariant(), request.Phone?.Trim(),
            request.DepartmentId, request.Designation?.Trim(),
            (EmploymentStatus)request.Status, (EmploymentType)request.EmploymentType,
            joinDate, request.DateOfBirth, request.ManagerId, request.Notes?.Trim());
        employee.SetAudit(currentUserId);
        _dbContext.Employees.Add(employee);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(MapToResponse(employee));
    }

    public async Task<Result<EmployeeResponse>> UpdateEmployeeAsync(long id, UpdateEmployeeRequest request, string currentUserId, CancellationToken cancellationToken = default)
    {
        var employee = await _dbContext.Employees
            .Include(e => e.Department)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        if (employee is null) return Result.Failure<EmployeeResponse>("Employee not found.");

        if (request.ManagerId == id)
            return Result.Failure<EmployeeResponse>("An employee cannot be their own manager.");

        if (request.DepartmentId.HasValue && !await _dbContext.Departments.AnyAsync(d => d.Id == request.DepartmentId.Value, cancellationToken))
            return Result.Failure<EmployeeResponse>("The specified department does not exist.");

        if (request.ManagerId.HasValue && !await _dbContext.Employees.AnyAsync(e => e.Id == request.ManagerId.Value, cancellationToken))
            return Result.Failure<EmployeeResponse>("The specified manager does not exist.");

        var joinDate = request.DateOfJoining == default ? employee.DateOfJoining : request.DateOfJoining;

        employee.Update(request.FirstName.Trim(), request.LastName.Trim(),
            request.Email?.Trim().ToLowerInvariant(), request.Phone?.Trim(),
            request.DepartmentId, request.Designation?.Trim(),
            (EmploymentStatus)request.Status, (EmploymentType)request.EmploymentType,
            joinDate, request.DateOfBirth, request.ManagerId, request.Notes?.Trim());
        employee.SetAudit(currentUserId);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(MapToResponse(employee));
    }

    public async Task<Result> DeleteEmployeeAsync(long id, string currentUserId, CancellationToken cancellationToken = default)
    {
        var employee = await _dbContext.Employees.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        if (employee is null) return Result.Failure("Employee not found.");

        employee.SoftDelete(currentUserId);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private static EmployeeResponse MapToResponse(Employee e) => new(
        e.Id, e.EmployeeNumber, e.FirstName, e.LastName,
        $"{e.FirstName} {e.LastName}",
        e.Email, e.Phone,
        e.DepartmentId, e.Department?.Name,
        e.Designation,
        (int)e.Status, e.Status.ToString(),
        (int)e.EmploymentType, e.EmploymentType.ToString(),
        e.DateOfJoining, e.DateOfBirth,
        e.ManagerId, e.Notes,
        e.CreatedAt, e.CreatedBy, e.UpdatedAt);
}
