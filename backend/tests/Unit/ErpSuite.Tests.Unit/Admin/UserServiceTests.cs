using ErpSuite.Modules.Admin.Application.Users.Dtos;
using ErpSuite.Modules.Admin.Domain.Entities;
using ErpSuite.Modules.Admin.Infrastructure.Security;
using ErpSuite.Modules.Admin.Infrastructure.Services;
using ErpSuite.Tests.Unit.Helpers;
using FluentAssertions;

namespace ErpSuite.Tests.Unit.Admin;

public class UserServiceTests
{
    private readonly UserService _sut;
    private readonly ErpSuite.Modules.Admin.Infrastructure.Persistence.ErpDbContext _dbContext;

    public UserServiceTests()
    {
        _dbContext = TestDbContextFactory.Create();
        _sut = new UserService(_dbContext, new BcryptPasswordHasher());
    }

    [Fact]
    public async Task CreateUser_WithValidRequest_ShouldReturnSuccess()
    {
        var roleId = await SeedRoleAsync("Admin");
        var request = new CreateUserRequest(
            "  Alice@example.com ",
            "Passw0rd!",
            "Alice",
            "Nguyen",
            roleId,
            Phone: "+123456789");

        var result = await _sut.CreateUserAsync(request, "user-1");

        result.IsSuccess.Should().BeTrue();
        result.Value.Email.Should().Be("alice@example.com");
        result.Value.FirstName.Should().Be("Alice");
        result.Value.LastName.Should().Be("Nguyen");
        result.Value.RoleId.Should().Be(roleId);
        result.Value.RoleName.Should().Be("Admin");
        result.Value.Status.Should().Be(UserStatus.Active);
        result.Value.CreatedBy.Should().Be("user-1");
    }

    [Fact]
    public async Task CreateUser_WithDuplicateEmail_ShouldReturnFailure()
    {
        var roleId = await SeedRoleAsync("Admin");
        await _sut.CreateUserAsync(
            new CreateUserRequest("alice@example.com", "Passw0rd!", "Alice", "Nguyen", roleId),
            "user-1");

        var result = await _sut.CreateUserAsync(
            new CreateUserRequest("ALICE@EXAMPLE.COM", "Passw0rd!", "Other", "User", roleId),
            "user-2");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("already exists");
    }

    [Fact]
    public async Task CreateUser_WithMissingManager_ShouldReturnFailure()
    {
        var roleId = await SeedRoleAsync("Admin");

        var result = await _sut.CreateUserAsync(
            new CreateUserRequest("alice@example.com", "Passw0rd!", "Alice", "Nguyen", roleId, ManagerId: 999),
            "user-1");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("manager");
    }

    [Fact]
    public async Task GetUserById_WhenExists_ShouldReturnUser()
    {
        var roleId = await SeedRoleAsync("Admin");
        var createResult = await _sut.CreateUserAsync(
            new CreateUserRequest("alice@example.com", "Passw0rd!", "Alice", "Nguyen", roleId),
            "user-1");

        var result = await _sut.GetUserByIdAsync(createResult.Value.Id);

        result.Should().NotBeNull();
        result!.Email.Should().Be("alice@example.com");
        result.RoleName.Should().Be("Admin");
    }

    [Fact]
    public async Task UpdateUser_WhenExists_ShouldReturnSuccess()
    {
        var adminRoleId = await SeedRoleAsync("Admin");
        var managerRoleId = await SeedRoleAsync("Manager");
        var manager = await _sut.CreateUserAsync(
            new CreateUserRequest("manager@example.com", "Passw0rd!", "Maya", "Manager", managerRoleId),
            "seed");
        var createResult = await _sut.CreateUserAsync(
            new CreateUserRequest("alice@example.com", "Passw0rd!", "Alice", "Nguyen", adminRoleId),
            "user-1");

        var updateRequest = new UpdateUserRequest(
            "Alicia",
            "Tran",
            managerRoleId,
            Phone: "+987654321",
            ManagerId: manager.Value.Id,
            Status: UserStatus.Inactive);

        var result = await _sut.UpdateUserAsync(createResult.Value.Id, updateRequest, "user-2");

        result.IsSuccess.Should().BeTrue();
        result.Value.FirstName.Should().Be("Alicia");
        result.Value.LastName.Should().Be("Tran");
        result.Value.RoleId.Should().Be(managerRoleId);
        result.Value.RoleName.Should().Be("Manager");
        result.Value.Phone.Should().Be("+987654321");
        result.Value.ManagerId.Should().Be(manager.Value.Id);
        result.Value.Status.Should().Be(UserStatus.Inactive);
    }

    [Fact]
    public async Task UpdateUser_WhenManagerIsSelf_ShouldReturnFailure()
    {
        var roleId = await SeedRoleAsync("Admin");
        var createResult = await _sut.CreateUserAsync(
            new CreateUserRequest("alice@example.com", "Passw0rd!", "Alice", "Nguyen", roleId),
            "user-1");

        var result = await _sut.UpdateUserAsync(
            createResult.Value.Id,
            new UpdateUserRequest("Alice", "Nguyen", roleId, ManagerId: createResult.Value.Id),
            "user-2");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("own manager");
    }

    [Fact]
    public async Task DeleteUser_WhenDeletingCurrentUser_ShouldReturnFailure()
    {
        var roleId = await SeedRoleAsync("Admin");
        var createResult = await _sut.CreateUserAsync(
            new CreateUserRequest("alice@example.com", "Passw0rd!", "Alice", "Nguyen", roleId),
            "123");

        var result = await _sut.DeleteUserAsync(createResult.Value.Id, createResult.Value.Id.ToString());

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("cannot delete your own account");
    }

    [Fact]
    public async Task DeleteUser_WhenExists_ShouldSoftDelete()
    {
        var roleId = await SeedRoleAsync("Admin");
        var createResult = await _sut.CreateUserAsync(
            new CreateUserRequest("alice@example.com", "Passw0rd!", "Alice", "Nguyen", roleId),
            "user-1");

        var result = await _sut.DeleteUserAsync(createResult.Value.Id, "user-2");

        result.IsSuccess.Should().BeTrue();
        var user = await _sut.GetUserByIdAsync(createResult.Value.Id);
        user.Should().BeNull();
    }

    [Fact]
    public async Task ActivateUser_ShouldSetStatusActive()
    {
        var roleId = await SeedRoleAsync("Admin");
        var createResult = await _sut.CreateUserAsync(
            new CreateUserRequest("alice@example.com", "Passw0rd!", "Alice", "Nguyen", roleId),
            "user-1");
        await _sut.DeactivateUserAsync(createResult.Value.Id, "user-2");

        var result = await _sut.ActivateUserAsync(createResult.Value.Id, "user-3");

        result.IsSuccess.Should().BeTrue();
        var user = await _sut.GetUserByIdAsync(createResult.Value.Id);
        user!.Status.Should().Be(UserStatus.Active);
    }

    [Fact]
    public async Task DeactivateUser_ShouldSetStatusInactive()
    {
        var roleId = await SeedRoleAsync("Admin");
        var createResult = await _sut.CreateUserAsync(
            new CreateUserRequest("alice@example.com", "Passw0rd!", "Alice", "Nguyen", roleId),
            "user-1");

        var result = await _sut.DeactivateUserAsync(createResult.Value.Id, "user-2");

        result.IsSuccess.Should().BeTrue();
        var user = await _sut.GetUserByIdAsync(createResult.Value.Id);
        user!.Status.Should().Be(UserStatus.Inactive);
    }

    private async Task<long> SeedRoleAsync(string name)
    {
        var role = Role.Create(name, $"{name} role");
        role.SetAudit("seed");
        _dbContext.Roles.Add(role);
        await _dbContext.SaveChangesAsync();
        return role.Id;
    }
}