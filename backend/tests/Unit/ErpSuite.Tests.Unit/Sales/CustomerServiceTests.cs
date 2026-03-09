using ErpSuite.Modules.Sales.Application.Customers.Dtos;
using ErpSuite.Modules.Sales.Infrastructure.Services;
using ErpSuite.Tests.Unit.Helpers;
using FluentAssertions;

namespace ErpSuite.Tests.Unit.Sales;

public class CustomerServiceTests
{
    private readonly CustomerService _sut;
    private readonly ErpSuite.Modules.Admin.Infrastructure.Persistence.ErpDbContext _dbContext;

    public CustomerServiceTests()
    {
        _dbContext = TestDbContextFactory.Create();
        _sut = new CustomerService(_dbContext);
    }

    [Fact]
    public async Task CreateCustomer_WithValidRequest_ShouldReturnSuccess()
    {
        var request = new CreateCustomerRequest("CUST-001", "Acme Corp", Email: "acme@test.com");

        var result = await _sut.CreateCustomerAsync(request, "user-1");

        result.IsSuccess.Should().BeTrue();
        result.Value.Code.Should().Be("CUST-001");
        result.Value.Name.Should().Be("Acme Corp");
        result.Value.Email.Should().Be("acme@test.com");
        result.Value.IsActive.Should().BeTrue();
        result.Value.CreatedBy.Should().Be("user-1");
    }

    [Fact]
    public async Task CreateCustomer_WithDuplicateCode_ShouldReturnFailure()
    {
        var request = new CreateCustomerRequest("CUST-001", "Acme Corp");
        await _sut.CreateCustomerAsync(request, "user-1");

        var duplicateRequest = new CreateCustomerRequest("cust-001", "Other Corp");
        var result = await _sut.CreateCustomerAsync(duplicateRequest, "user-1");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("already exists");
    }

    [Fact]
    public async Task CreateCustomer_ShouldNormalizeCodeToUpperCase()
    {
        var request = new CreateCustomerRequest("  cust-abc  ", "Test Corp");

        var result = await _sut.CreateCustomerAsync(request, "user-1");

        result.IsSuccess.Should().BeTrue();
        result.Value.Code.Should().Be("CUST-ABC");
    }

    [Fact]
    public async Task CreateCustomer_ShouldSetAllFields()
    {
        var request = new CreateCustomerRequest(
            "CUST-FULL", "Full Customer",
            ContactPerson: "John Doe",
            Email: "john@test.com",
            Phone: "+1234567890",
            Website: "https://example.com",
            TaxId: "TAX-123",
            AddressLine1: "123 Main St",
            City: "Springfield",
            State: "IL",
            Country: "US",
            CreditLimit: 50000m,
            Currency: "EUR",
            PaymentTerms: "Net30",
            Notes: "VIP customer");

        var result = await _sut.CreateCustomerAsync(request, "user-1");

        result.IsSuccess.Should().BeTrue();
        var customer = result.Value;
        customer.ContactPerson.Should().Be("John Doe");
        customer.CreditLimit.Should().Be(50000m);
        customer.Currency.Should().Be("EUR");
        customer.Country.Should().Be("US");
        customer.PaymentTerms.Should().Be("Net30");
        customer.Notes.Should().Be("VIP customer");
    }

    [Fact]
    public async Task GetCustomerById_WhenExists_ShouldReturnCustomer()
    {
        var createResult = await _sut.CreateCustomerAsync(
            new CreateCustomerRequest("CUST-001", "Acme Corp"), "user-1");

        var customer = await _sut.GetCustomerByIdAsync(createResult.Value.Id);

        customer.Should().NotBeNull();
        customer!.Name.Should().Be("Acme Corp");
    }

    [Fact]
    public async Task GetCustomerById_WhenNotExists_ShouldReturnNull()
    {
        var customer = await _sut.GetCustomerByIdAsync(999);

        customer.Should().BeNull();
    }

    [Fact]
    public async Task UpdateCustomer_WhenExists_ShouldReturnSuccess()
    {
        var createResult = await _sut.CreateCustomerAsync(
            new CreateCustomerRequest("CUST-001", "Acme Corp"), "user-1");
        var id = createResult.Value.Id;

        var updateRequest = new UpdateCustomerRequest("Acme Corp Updated", Email: "new@acme.com", CreditLimit: 100000m);
        var result = await _sut.UpdateCustomerAsync(id, updateRequest, "user-2");

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("Acme Corp Updated");
        result.Value.Email.Should().Be("new@acme.com");
        result.Value.CreditLimit.Should().Be(100000m);
    }

    [Fact]
    public async Task UpdateCustomer_WhenNotExists_ShouldReturnFailure()
    {
        var result = await _sut.UpdateCustomerAsync(999,
            new UpdateCustomerRequest("Test"), "user-1");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("not found");
    }

    [Fact]
    public async Task DeleteCustomer_WhenExists_ShouldSoftDelete()
    {
        var createResult = await _sut.CreateCustomerAsync(
            new CreateCustomerRequest("CUST-001", "Acme Corp"), "user-1");
        var id = createResult.Value.Id;

        var result = await _sut.DeleteCustomerAsync(id, "user-1");

        result.IsSuccess.Should().BeTrue();
        // Soft-deleted customer shouldn't appear in queries (global filter)
        var customer = await _sut.GetCustomerByIdAsync(id);
        customer.Should().BeNull();
    }

    [Fact]
    public async Task DeleteCustomer_WhenNotExists_ShouldReturnFailure()
    {
        var result = await _sut.DeleteCustomerAsync(999, "user-1");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("not found");
    }

    [Fact]
    public async Task ActivateCustomer_ShouldSetIsActiveTrue()
    {
        var createResult = await _sut.CreateCustomerAsync(
            new CreateCustomerRequest("CUST-001", "Acme Corp"), "user-1");
        var id = createResult.Value.Id;
        await _sut.DeactivateCustomerAsync(id, "user-1");

        var result = await _sut.ActivateCustomerAsync(id, "user-1");

        result.IsSuccess.Should().BeTrue();
        var customer = await _sut.GetCustomerByIdAsync(id);
        customer!.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task DeactivateCustomer_ShouldSetIsActiveFalse()
    {
        var createResult = await _sut.CreateCustomerAsync(
            new CreateCustomerRequest("CUST-001", "Acme Corp"), "user-1");
        var id = createResult.Value.Id;

        var result = await _sut.DeactivateCustomerAsync(id, "user-1");

        result.IsSuccess.Should().BeTrue();
        var customer = await _sut.GetCustomerByIdAsync(id);
        customer!.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task GetCustomers_WithSearchTerm_ShouldFilterResults()
    {
        await _sut.CreateCustomerAsync(new CreateCustomerRequest("CUST-001", "Acme Corp"), "user-1");
        await _sut.CreateCustomerAsync(new CreateCustomerRequest("CUST-002", "Beta Inc"), "user-1");

        var result = await _sut.GetCustomersAsync(new GetCustomersQuery { SearchTerm = "acme" });

        result.Items.Should().HaveCount(1);
        result.Items[0].Name.Should().Be("Acme Corp");
    }

    [Fact]
    public async Task GetCustomers_WithIsActiveFilter_ShouldFilterResults()
    {
        var createResult = await _sut.CreateCustomerAsync(
            new CreateCustomerRequest("CUST-001", "Active Corp"), "user-1");
        await _sut.CreateCustomerAsync(new CreateCustomerRequest("CUST-002", "Inactive Corp"), "user-1");
        await _sut.DeactivateCustomerAsync(
            (await _sut.GetCustomersAsync(new GetCustomersQuery { SearchTerm = "Inactive" })).Items[0].Id, "user-1");

        var result = await _sut.GetCustomersAsync(new GetCustomersQuery { IsActive = true });

        result.Items.Should().OnlyContain(c => c.IsActive);
    }

    [Fact]
    public async Task GetCustomers_WithPagination_ShouldReturnCorrectPage()
    {
        for (int i = 1; i <= 5; i++)
            await _sut.CreateCustomerAsync(new CreateCustomerRequest($"CUST-{i:D3}", $"Customer {i}"), "user-1");

        var result = await _sut.GetCustomersAsync(new GetCustomersQuery { Page = 2, PageSize = 2, SortBy = "code" });

        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(5);
        result.Page.Should().Be(2);
        result.TotalPages.Should().Be(3);
        result.HasPreviousPage.Should().BeTrue();
        result.HasNextPage.Should().BeTrue();
    }

    [Fact]
    public async Task GetCustomers_WithSortDescending_ShouldOrderCorrectly()
    {
        await _sut.CreateCustomerAsync(new CreateCustomerRequest("CUST-001", "Alpha"), "user-1");
        await _sut.CreateCustomerAsync(new CreateCustomerRequest("CUST-002", "Zeta"), "user-1");

        var result = await _sut.GetCustomersAsync(new GetCustomersQuery { SortBy = "name", SortDescending = true });

        result.Items[0].Name.Should().Be("Zeta");
        result.Items[1].Name.Should().Be("Alpha");
    }
}
