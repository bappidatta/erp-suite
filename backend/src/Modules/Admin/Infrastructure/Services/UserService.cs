using ErpSuite.BuildingBlocks.Domain.Results;
using ErpSuite.Modules.Admin.Application.Auth;
using ErpSuite.BuildingBlocks.Application.Common;
using ErpSuite.Modules.Admin.Application.Users;
using ErpSuite.Modules.Admin.Application.Users.Dtos;
using ErpSuite.Modules.Admin.Domain.Entities;
using ErpSuite.Modules.Admin.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ErpSuite.Modules.Admin.Infrastructure.Services;

public sealed class UserService : IUserService
{
    private readonly ErpDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;

    public UserService(ErpDbContext dbContext, IPasswordHasher passwordHasher)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
    }

    public async Task<PagedResult<UserResponse>> GetUsersAsync(GetUsersQuery query, CancellationToken cancellationToken = default)
    {
        var queryable = _dbContext.Users.Include(u => u.Role).AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var term = query.SearchTerm.ToLower();
            queryable = queryable.Where(u =>
                u.Email.ToLower().Contains(term) ||
                u.FirstName.ToLower().Contains(term) ||
                u.LastName.ToLower().Contains(term));
        }

        if (query.RoleId.HasValue)
            queryable = queryable.Where(u => u.RoleId == query.RoleId.Value);

        if (query.DepartmentId.HasValue)
            queryable = queryable.Where(u => u.DepartmentId == query.DepartmentId.Value);

        if (query.Status.HasValue)
            queryable = queryable.Where(u => u.Status == query.Status.Value);

        queryable = query.SortBy?.ToLower() switch
        {
            "email" => query.SortDescending
                ? queryable.OrderByDescending(u => u.Email)
                : queryable.OrderBy(u => u.Email),
            "createdat" => query.SortDescending
                ? queryable.OrderByDescending(u => u.CreatedAt)
                : queryable.OrderBy(u => u.CreatedAt),
            _ => query.SortDescending
                ? queryable.OrderByDescending(u => u.FirstName).ThenByDescending(u => u.LastName)
                : queryable.OrderBy(u => u.FirstName).ThenBy(u => u.LastName)
        };

        var totalCount = await queryable.CountAsync(cancellationToken);
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var users = await queryable
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<UserResponse>(
            users.Select(MapToResponse).ToList(),
            totalCount,
            page,
            pageSize);
    }

    public async Task<UserResponse?> GetUserByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        return user is null ? null : MapToResponse(user);
    }

    public async Task<Result<UserResponse>> CreateUserAsync(CreateUserRequest request, string currentUserId, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        if (await _dbContext.Users.AnyAsync(u => u.Email.ToLower() == normalizedEmail, cancellationToken))
            return Result.Failure<UserResponse>("A user with this email already exists.");

        if (!await _dbContext.Roles.AnyAsync(r => r.Id == request.RoleId, cancellationToken))
            return Result.Failure<UserResponse>("The specified role does not exist.");

        var passwordHash = _passwordHasher.Hash(request.Password);
        var user = User.Create(normalizedEmail, passwordHash, request.FirstName, request.LastName,
            request.RoleId, request.Phone, request.MustChangePassword);

        if (request.DepartmentId.HasValue || request.ManagerId.HasValue)
            user.UpdateProfile(request.FirstName, request.LastName, request.Phone, request.DepartmentId, request.ManagerId);

        user.SetAudit(currentUserId);
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _dbContext.Entry(user).Reference(u => u.Role).LoadAsync(cancellationToken);
        return Result.Success(MapToResponse(user));
    }

    public async Task<Result<UserResponse>> UpdateUserAsync(long id, UpdateUserRequest request, string currentUserId, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        if (user is null)
            return Result.Failure<UserResponse>("User not found.");

        if (!await _dbContext.Roles.AnyAsync(r => r.Id == request.RoleId, cancellationToken))
            return Result.Failure<UserResponse>("The specified role does not exist.");

        user.UpdateProfile(request.FirstName, request.LastName, request.Phone, request.DepartmentId, request.ManagerId);

        if (user.RoleId != request.RoleId)
            user.AssignRole(request.RoleId);

        if (request.Status.HasValue && request.Status.Value != user.Status)
            user.ChangeStatus(request.Status.Value);

        user.SetAudit(currentUserId);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _dbContext.Entry(user).Reference(u => u.Role).LoadAsync(cancellationToken);
        return Result.Success(MapToResponse(user));
    }

    public async Task<Result> DeleteUserAsync(long id, string currentUserId, CancellationToken cancellationToken = default)
    {
        if (long.TryParse(currentUserId, out var currentId) && currentId == id)
            return Result.Failure("You cannot delete your own account.");

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        if (user is null)
            return Result.Failure("User not found.");

        user.SoftDelete(currentUserId);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> ActivateUserAsync(long id, string currentUserId, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        if (user is null)
            return Result.Failure("User not found.");

        user.Activate();
        user.SetAudit(currentUserId);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private static UserResponse MapToResponse(User user) => new(
        user.Id,
        user.Email,
        user.FirstName,
        user.LastName,
        user.FullName,
        user.Phone,
        user.Status,
        user.Status.ToString(),
        user.RoleId,
        user.Role?.Name ?? string.Empty,
        user.DepartmentId,
        user.ManagerId,
        user.MustChangePassword,
        user.LastLoginAt,
        user.CreatedAt,
        user.CreatedBy,
        user.UpdatedAt);
}
