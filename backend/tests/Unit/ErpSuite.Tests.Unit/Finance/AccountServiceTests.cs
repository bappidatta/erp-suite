using ErpSuite.Modules.Finance.Application.Accounts.Dtos;
using ErpSuite.Modules.Finance.Domain.Entities;
using ErpSuite.Modules.Finance.Infrastructure.Services;
using ErpSuite.Tests.Unit.Helpers;
using FluentAssertions;

namespace ErpSuite.Tests.Unit.Finance;

public class AccountServiceTests
{
    private readonly AccountService _sut;
    private readonly ErpSuite.Modules.Admin.Infrastructure.Persistence.ErpDbContext _dbContext;

    public AccountServiceTests()
    {
        _dbContext = TestDbContextFactory.Create();
        _sut = new AccountService(_dbContext);
    }

    [Fact]
    public async Task CreateAccount_WithValidRequest_ShouldReturnSuccess()
    {
        var request = new CreateAccountRequest("1000", "Cash", AccountType.Asset);

        var result = await _sut.CreateAccountAsync(request, "user-1");

        result.IsSuccess.Should().BeTrue();
        result.Value.Code.Should().Be("1000");
        result.Value.Name.Should().Be("Cash");
        result.Value.Type.Should().Be(AccountType.Asset);
        result.Value.Level.Should().Be(0);
        result.Value.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task CreateAccount_WithDuplicateCode_ShouldReturnFailure()
    {
        await _sut.CreateAccountAsync(new CreateAccountRequest("1000", "Cash", AccountType.Asset), "user-1");

        var result = await _sut.CreateAccountAsync(
            new CreateAccountRequest("1000", "Another Cash", AccountType.Asset), "user-1");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("already exists");
    }

    [Fact]
    public async Task CreateAccount_WithParent_ShouldCalculateLevel()
    {
        var parent = await _sut.CreateAccountAsync(
            new CreateAccountRequest("1000", "Assets", AccountType.Asset, IsHeader: true), "user-1");

        var result = await _sut.CreateAccountAsync(
            new CreateAccountRequest("1001", "Cash", AccountType.Asset, ParentId: parent.Value.Id), "user-1");

        result.IsSuccess.Should().BeTrue();
        result.Value.Level.Should().Be(1);
        result.Value.ParentId.Should().Be(parent.Value.Id);
    }

    [Fact]
    public async Task CreateAccount_WithInvalidParent_ShouldReturnFailure()
    {
        var result = await _sut.CreateAccountAsync(
            new CreateAccountRequest("1000", "Cash", AccountType.Asset, ParentId: 999), "user-1");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Parent account not found");
    }

    [Fact]
    public async Task CreateAccount_ShouldNormalizeCode()
    {
        var result = await _sut.CreateAccountAsync(
            new CreateAccountRequest("  acc-100  ", "Test", AccountType.Asset), "user-1");

        result.IsSuccess.Should().BeTrue();
        result.Value.Code.Should().Be("ACC-100");
    }

    [Fact]
    public async Task UpdateAccount_WhenExists_ShouldReturnSuccess()
    {
        var created = await _sut.CreateAccountAsync(
            new CreateAccountRequest("1000", "Old Name", AccountType.Asset), "user-1");

        var result = await _sut.UpdateAccountAsync(created.Value.Id,
            new UpdateAccountRequest("New Name", AccountType.Liability, Description: "Updated"), "user-2");

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("New Name");
        result.Value.Type.Should().Be(AccountType.Liability);
        result.Value.Description.Should().Be("Updated");
    }

    [Fact]
    public async Task UpdateAccount_WhenNotExists_ShouldReturnFailure()
    {
        var result = await _sut.UpdateAccountAsync(999,
            new UpdateAccountRequest("Test", AccountType.Asset), "user-1");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("not found");
    }

    [Fact]
    public async Task UpdateAccount_SelfParent_ShouldReturnFailure()
    {
        var created = await _sut.CreateAccountAsync(
            new CreateAccountRequest("1000", "Cash", AccountType.Asset), "user-1");
        var id = created.Value.Id;

        var result = await _sut.UpdateAccountAsync(id,
            new UpdateAccountRequest("Cash", AccountType.Asset, ParentId: id), "user-1");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("cannot be its own parent");
    }

    [Fact]
    public async Task UpdateAccount_CircularParent_ShouldReturnFailure()
    {
        // Create A -> B -> C, then try to set A's parent to C (circular)
        var a = await _sut.CreateAccountAsync(
            new CreateAccountRequest("A", "Account A", AccountType.Asset, IsHeader: true), "user-1");
        var b = await _sut.CreateAccountAsync(
            new CreateAccountRequest("B", "Account B", AccountType.Asset, ParentId: a.Value.Id), "user-1");
        var c = await _sut.CreateAccountAsync(
            new CreateAccountRequest("C", "Account C", AccountType.Asset, ParentId: b.Value.Id), "user-1");

        var result = await _sut.UpdateAccountAsync(a.Value.Id,
            new UpdateAccountRequest("Account A", AccountType.Asset, ParentId: c.Value.Id), "user-1");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Circular parent reference");
    }

    [Fact]
    public async Task DeleteAccount_WhenExists_ShouldSoftDelete()
    {
        var created = await _sut.CreateAccountAsync(
            new CreateAccountRequest("1000", "Cash", AccountType.Asset), "user-1");

        var result = await _sut.DeleteAccountAsync(created.Value.Id, "user-1");

        result.IsSuccess.Should().BeTrue();
        var account = await _sut.GetAccountByIdAsync(created.Value.Id);
        account.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAccount_WhenNotExists_ShouldReturnFailure()
    {
        var result = await _sut.DeleteAccountAsync(999, "user-1");

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAccount_WithChildren_ShouldReturnFailure()
    {
        var parent = await _sut.CreateAccountAsync(
            new CreateAccountRequest("1000", "Assets", AccountType.Asset, IsHeader: true), "user-1");
        await _sut.CreateAccountAsync(
            new CreateAccountRequest("1001", "Cash", AccountType.Asset, ParentId: parent.Value.Id), "user-1");

        var result = await _sut.DeleteAccountAsync(parent.Value.Id, "user-1");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("child accounts");
    }

    [Fact]
    public async Task ActivateAccount_ShouldSetIsActiveTrue()
    {
        var created = await _sut.CreateAccountAsync(
            new CreateAccountRequest("1000", "Cash", AccountType.Asset), "user-1");
        await _sut.DeactivateAccountAsync(created.Value.Id, "user-1");

        var result = await _sut.ActivateAccountAsync(created.Value.Id, "user-1");

        result.IsSuccess.Should().BeTrue();
        var account = await _sut.GetAccountByIdAsync(created.Value.Id);
        account!.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task DeactivateAccount_ShouldSetIsActiveFalse()
    {
        var created = await _sut.CreateAccountAsync(
            new CreateAccountRequest("1000", "Cash", AccountType.Asset), "user-1");

        var result = await _sut.DeactivateAccountAsync(created.Value.Id, "user-1");

        result.IsSuccess.Should().BeTrue();
        var account = await _sut.GetAccountByIdAsync(created.Value.Id);
        account!.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task GetAccountTree_ShouldReturnHierarchy()
    {
        var parent = await _sut.CreateAccountAsync(
            new CreateAccountRequest("1000", "Assets", AccountType.Asset, IsHeader: true), "user-1");
        await _sut.CreateAccountAsync(
            new CreateAccountRequest("1001", "Cash", AccountType.Asset, ParentId: parent.Value.Id), "user-1");
        await _sut.CreateAccountAsync(
            new CreateAccountRequest("1002", "Bank", AccountType.Asset, ParentId: parent.Value.Id), "user-1");

        var tree = await _sut.GetAccountTreeAsync();

        tree.Should().HaveCount(1);
        tree[0].Code.Should().Be("1000");
        tree[0].Children.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAccounts_WithTypeFilter_ShouldFilterResults()
    {
        await _sut.CreateAccountAsync(
            new CreateAccountRequest("1000", "Cash", AccountType.Asset), "user-1");
        await _sut.CreateAccountAsync(
            new CreateAccountRequest("4000", "Revenue", AccountType.Revenue), "user-1");

        var result = await _sut.GetAccountsAsync(new GetAccountsQuery { Type = AccountType.Revenue });

        result.Items.Should().HaveCount(1);
        result.Items[0].Code.Should().Be("4000");
    }

    [Fact]
    public async Task GetAccounts_WithSearchTerm_ShouldFilterResults()
    {
        await _sut.CreateAccountAsync(
            new CreateAccountRequest("1000", "Cash", AccountType.Asset), "user-1");
        await _sut.CreateAccountAsync(
            new CreateAccountRequest("2000", "Accounts Payable", AccountType.Liability), "user-1");

        var result = await _sut.GetAccountsAsync(new GetAccountsQuery { SearchTerm = "payable" });

        result.Items.Should().HaveCount(1);
        result.Items[0].Code.Should().Be("2000");
    }
}
