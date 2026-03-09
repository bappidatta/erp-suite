using ErpSuite.BuildingBlocks.Domain.Results;
using ErpSuite.Modules.Admin.Application.Roles;
using ErpSuite.Modules.Admin.Application.Roles.Dtos;
using ErpSuite.Modules.Admin.Domain.Entities;
using ErpSuite.Modules.Admin.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ErpSuite.Modules.Admin.Infrastructure.Services;

public sealed class RoleService : IRoleService
{
    private readonly ErpDbContext _dbContext;

    public RoleService(ErpDbContext dbContext) => _dbContext = dbContext;

    public async Task<IReadOnlyList<RoleResponse>> GetRolesAsync(CancellationToken cancellationToken = default)
    {
        var roles = await _dbContext.Roles
            .Include(r => r.Users)
            .Include(r => r.RolePermissions)
            .ToListAsync(cancellationToken);

        return roles.Select(r => new RoleResponse(
            r.Id, r.Name, r.Description,
            r.Users.Count,
            r.RolePermissions.Count,
            r.IsSystem,
            r.CreatedAt)).ToList();
    }

    public async Task<RoleDetailResponse?> GetRoleByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var role = await _dbContext.Roles
            .Include(r => r.Users)
            .Include(r => r.RolePermissions).ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        if (role is null) return null;

        return new RoleDetailResponse(
            role.Id, role.Name, role.Description,
            role.Users.Count,
            role.IsSystem,
            role.RolePermissions
                .Select(rp => new PermissionResponse(rp.Permission.Id, rp.Permission.Name, rp.Permission.Module, rp.Permission.Action, rp.Permission.Description))
                .ToList(),
            role.CreatedAt);
    }

    public async Task<Result<RoleResponse>> CreateRoleAsync(CreateRoleRequest request, string currentUserId, CancellationToken cancellationToken = default)
    {
        if (await _dbContext.Roles.AnyAsync(r => r.Name == request.Name, cancellationToken))
            return Result.Failure<RoleResponse>("A role with this name already exists.");

        var role = Role.Create(request.Name, request.Description);
        role.SetAudit(currentUserId);
        _dbContext.Roles.Add(role);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(new RoleResponse(role.Id, role.Name, role.Description, 0, 0, role.IsSystem, role.CreatedAt));
    }

    public async Task<Result<RoleResponse>> UpdateRoleAsync(long id, UpdateRoleRequest request, string currentUserId, CancellationToken cancellationToken = default)
    {
        var role = await _dbContext.Roles
            .Include(r => r.Users)
            .Include(r => r.RolePermissions)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        if (role is null) return Result.Failure<RoleResponse>("Role not found.");
        if (role.IsSystem) return Result.Failure<RoleResponse>("System roles cannot be modified.");

        if (await _dbContext.Roles.AnyAsync(r => r.Name == request.Name && r.Id != id, cancellationToken))
            return Result.Failure<RoleResponse>("A role with this name already exists.");

        role.Update(request.Name, request.Description);
        role.SetAudit(currentUserId);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(new RoleResponse(role.Id, role.Name, role.Description,
            role.Users.Count, role.RolePermissions.Count, role.IsSystem, role.CreatedAt));
    }

    public async Task<Result> DeleteRoleAsync(long id, CancellationToken cancellationToken = default)
    {
        var role = await _dbContext.Roles
            .Include(r => r.Users)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        if (role is null) return Result.Failure("Role not found.");
        if (role.IsSystem) return Result.Failure("System roles cannot be deleted.");
        if (role.Users.Count > 0) return Result.Failure("Cannot delete a role that has users assigned to it.");

        role.SoftDelete("system");
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> AssignPermissionsAsync(long roleId, AssignPermissionsRequest request, CancellationToken cancellationToken = default)
    {
        if (!await _dbContext.Roles.AnyAsync(r => r.Id == roleId, cancellationToken))
            return Result.Failure("Role not found.");

        var existing = await _dbContext.RolePermissions
            .Where(rp => rp.RoleId == roleId)
            .ToListAsync(cancellationToken);

        _dbContext.RolePermissions.RemoveRange(existing);

        foreach (var permId in request.PermissionIds.Distinct())
        {
            if (await _dbContext.Permissions.AnyAsync(p => p.Id == permId, cancellationToken))
                _dbContext.RolePermissions.Add(RolePermission.Create(roleId, permId));
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<IReadOnlyList<PermissionResponse>> GetPermissionsAsync(CancellationToken cancellationToken = default)
    {
        var permissions = await _dbContext.Permissions.ToListAsync(cancellationToken);
        return permissions
            .Select(p => new PermissionResponse(p.Id, p.Name, p.Module, p.Action, p.Description))
            .ToList();
    }
}
