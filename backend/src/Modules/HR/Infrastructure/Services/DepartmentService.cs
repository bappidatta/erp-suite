using ErpSuite.BuildingBlocks.Application.Common;
using ErpSuite.BuildingBlocks.Domain.Results;
using ErpSuite.Modules.Admin.Infrastructure.Persistence;
using ErpSuite.Modules.HR.Application.Departments;
using ErpSuite.Modules.HR.Application.Departments.Dtos;
using ErpSuite.Modules.HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ErpSuite.Modules.HR.Infrastructure.Services;

public sealed class DepartmentService : IDepartmentService
{
    private readonly ErpDbContext _dbContext;

    public DepartmentService(ErpDbContext dbContext) => _dbContext = dbContext;

    public async Task<PagedResult<DepartmentResponse>> GetDepartmentsAsync(GetDepartmentsQuery query, CancellationToken cancellationToken = default)
    {
        var queryable = _dbContext.Departments.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var term = query.SearchTerm.ToLower();
            queryable = queryable.Where(d => d.Code.ToLower().Contains(term) || d.Name.ToLower().Contains(term));
        }

        if (query.IsActive.HasValue)
            queryable = queryable.Where(d => d.IsActive == query.IsActive.Value);

        queryable = queryable.OrderBy(d => d.Name);

        var totalCount = await queryable.CountAsync(cancellationToken);
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 200);

        var items = await queryable.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

        return new PagedResult<DepartmentResponse>(items.Select(MapToResponse).ToList(), totalCount, page, pageSize);
    }

    public async Task<DepartmentResponse?> GetDepartmentByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var dept = await _dbContext.Departments.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
        return dept is null ? null : MapToResponse(dept);
    }

    public async Task<Result<DepartmentResponse>> CreateDepartmentAsync(CreateDepartmentRequest request, string currentUserId, CancellationToken cancellationToken = default)
    {
        var normalizedCode = request.Code.Trim().ToUpperInvariant();

        if (await _dbContext.Departments.AnyAsync(d => d.Code.ToUpper() == normalizedCode, cancellationToken))
            return Result.Failure<DepartmentResponse>("A department with this code already exists.");

        if (request.ParentDepartmentId.HasValue && !await _dbContext.Departments.AnyAsync(d => d.Id == request.ParentDepartmentId.Value, cancellationToken))
            return Result.Failure<DepartmentResponse>("Parent department not found.");

        var dept = Department.Create(normalizedCode, request.Name.Trim(), request.Description?.Trim(), request.ParentDepartmentId);
        dept.SetAudit(currentUserId);
        _dbContext.Departments.Add(dept);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(MapToResponse(dept));
    }

    public async Task<Result<DepartmentResponse>> UpdateDepartmentAsync(long id, UpdateDepartmentRequest request, string currentUserId, CancellationToken cancellationToken = default)
    {
        var dept = await _dbContext.Departments.FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
        if (dept is null) return Result.Failure<DepartmentResponse>("Department not found.");

        if (request.ParentDepartmentId.HasValue)
        {
            if (request.ParentDepartmentId.Value == id)
                return Result.Failure<DepartmentResponse>("A department cannot be its own parent.");
            if (!await _dbContext.Departments.AnyAsync(d => d.Id == request.ParentDepartmentId.Value, cancellationToken))
                return Result.Failure<DepartmentResponse>("Parent department not found.");
        }

        dept.Update(request.Name.Trim(), request.Description?.Trim(), request.ParentDepartmentId);
        dept.SetAudit(currentUserId);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(MapToResponse(dept));
    }

    public async Task<Result> DeleteDepartmentAsync(long id, string currentUserId, CancellationToken cancellationToken = default)
    {
        var dept = await _dbContext.Departments.FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
        if (dept is null) return Result.Failure("Department not found.");

        var hasEmployees = await _dbContext.Employees.AnyAsync(e => e.DepartmentId == id, cancellationToken);
        if (hasEmployees) return Result.Failure("Cannot delete a department that has employees assigned to it.");

        dept.SoftDelete(currentUserId);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> ActivateDepartmentAsync(long id, string currentUserId, CancellationToken cancellationToken = default)
    {
        var dept = await _dbContext.Departments.FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
        if (dept is null) return Result.Failure("Department not found.");
        dept.Activate();
        dept.SetAudit(currentUserId);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> DeactivateDepartmentAsync(long id, string currentUserId, CancellationToken cancellationToken = default)
    {
        var dept = await _dbContext.Departments.FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
        if (dept is null) return Result.Failure("Department not found.");
        dept.Deactivate();
        dept.SetAudit(currentUserId);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private static DepartmentResponse MapToResponse(Department d) => new(
        d.Id, d.Code, d.Name, d.Description, d.ParentDepartmentId,
        d.IsActive, d.CreatedAt, d.CreatedBy, d.UpdatedAt);
}
