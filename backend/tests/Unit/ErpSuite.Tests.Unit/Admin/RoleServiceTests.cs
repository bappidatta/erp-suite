using ErpSuite.Modules.Admin.Application.Roles.Dtos;
using ErpSuite.Modules.Admin.Domain.Entities;
using ErpSuite.Modules.Admin.Infrastructure.Services;
using ErpSuite.Tests.Unit.Helpers;
using FluentAssertions;

namespace ErpSuite.Tests.Unit.Admin;

public class RoleServiceTests
{
    private readonly RoleService _sut;
    private readonly ErpSuite.Modules.Admin.Infrastructure.Persistence.ErpDbContext _dbContext;

    public RoleServiceTests()
    {
        _dbContext = TestDbContextFactory.Create();
        _sut = new RoleService(_dbContext);
    }

    [Fact]
    public async Task CreateRole_WithValidRequest_ShouldReturnSuccess()
    {
        var result = await _sut.CreateRoleAsync(new CreateRoleRequest("  Operations Manager  ", "Oversees operations"), "user-1");

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("Operations Manager");
        result.Value.Description.Should().Be("Oversees operations");
        result.Value.IsSystem.Should().BeFalse();
    }

    [Fact]
    public async Task CreateRole_WithDuplicateName_ShouldReturnFailure()
    {
        await _sut.CreateRoleAsync(new CreateRoleRequest("Admin"), "user-1");

        var result = await _sut.CreateRoleAsync(new CreateRoleRequest("admin"), "user-2");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("already exists");
    }

    [Fact]
    public async Task GetRoleById_WhenExists_ShouldReturnRoleWithPermissions()
    {
        var roleId = await SeedRoleAsync("Approver");
        var permissionId = await SeedPermissionAsync("Approve Sales Order", "Sales", "Approve");
        _dbContext.RolePermissions.Add(RolePermission.Create(roleId, permissionId));
        await _dbContext.SaveChangesAsync();

        var result = await _sut.GetRoleByIdAsync(roleId);

        result.Should().NotBeNull();
        result!.Name.Should().Be("Approver");
        result.Permissions.Should().ContainSingle();
        result.Permissions[0].Action.Should().Be("Approve");
    }

    [Fact]
    public async Task UpdateRole_WhenSystemRole_ShouldReturnFailure()
    {
        var roleId = await SeedRoleAsync("Admin", isSystem: true);

        var result = await _sut.UpdateRoleAsync(roleId, new UpdateRoleRequest("Changed"), "user-1");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("System roles cannot be modified");
    }

    [Fact]
    public async Task DeleteRole_WhenUsersAssigned_ShouldReturnFailure()
    {
        var roleId = await SeedRoleAsync("Manager");
        var user = User.Create("manager@example.com", "hash", "Maya", "Manager", roleId);
        user.SetAudit("seed");
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        var result = await _sut.DeleteRoleAsync(roleId, "user-1");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("users assigned");
    }

    [Fact]
    public async Task DeleteRole_WhenEligible_ShouldSoftDelete()
    {
        var roleId = await SeedRoleAsync("Purchasing Clerk");

        var result = await _sut.DeleteRoleAsync(roleId, "user-1");

        result.IsSuccess.Should().BeTrue();
        var role = await _sut.GetRoleByIdAsync(roleId);
        role.Should().BeNull();
    }

    [Fact]
    public async Task AssignPermissions_WithInvalidIds_ShouldReturnFailure()
    {
        var roleId = await SeedRoleAsync("Approver");

        var result = await _sut.AssignPermissionsAsync(roleId, new AssignPermissionsRequest([999]), "user-1");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Invalid permission ids");
    }

    [Fact]
    public async Task AssignPermissions_WithValidIds_ShouldReplaceAssignments()
    {
        var roleId = await SeedRoleAsync("Approver");
        var approveId = await SeedPermissionAsync("Approve Sales Order", "Sales", "Approve");
        var viewId = await SeedPermissionAsync("View Sales Order", "Sales", "View");
        _dbContext.RolePermissions.Add(RolePermission.Create(roleId, approveId));
        await _dbContext.SaveChangesAsync();

        var result = await _sut.AssignPermissionsAsync(roleId, new AssignPermissionsRequest([viewId]), "user-1");

        result.IsSuccess.Should().BeTrue();
        var role = await _sut.GetRoleByIdAsync(roleId);
        role!.Permissions.Should().ContainSingle();
        role.Permissions[0].Id.Should().Be(viewId);
    }

    private async Task<long> SeedRoleAsync(string name, bool isSystem = false)
    {
        var role = Role.Create(name, $"{name} role", isSystem);
        role.SetAudit("seed");
        _dbContext.Roles.Add(role);
        await _dbContext.SaveChangesAsync();
        return role.Id;
    }

    private async Task<long> SeedPermissionAsync(string name, string module, string action)
    {
        var permission = Permission.Create(name, module, action, $"{action} permission");
        _dbContext.Permissions.Add(permission);
        await _dbContext.SaveChangesAsync();
        return permission.Id;
    }
}