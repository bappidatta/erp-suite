using ErpSuite.Modules.Procurement.Application.Vendors.Dtos;
using ErpSuite.Modules.Procurement.Infrastructure.Services;
using ErpSuite.Tests.Unit.Helpers;
using FluentAssertions;

namespace ErpSuite.Tests.Unit.Procurement;

public class VendorServiceTests
{
    private readonly VendorService _sut;
    private readonly ErpSuite.Modules.Admin.Infrastructure.Persistence.ErpDbContext _dbContext;

    public VendorServiceTests()
    {
        _dbContext = TestDbContextFactory.Create();
        _sut = new VendorService(_dbContext);
    }

    [Fact]
    public async Task CreateVendor_WithValidRequest_ShouldReturnSuccess()
    {
        var request = new CreateVendorRequest("VND-001", "Supplier Co", Email: "info@supplier.com");

        var result = await _sut.CreateVendorAsync(request, "user-1");

        result.IsSuccess.Should().BeTrue();
        result.Value.Code.Should().Be("VND-001");
        result.Value.Name.Should().Be("Supplier Co");
        result.Value.Email.Should().Be("info@supplier.com");
        result.Value.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task CreateVendor_WithDuplicateCode_ShouldReturnFailure()
    {
        await _sut.CreateVendorAsync(new CreateVendorRequest("VND-001", "Supplier Co"), "user-1");

        var result = await _sut.CreateVendorAsync(new CreateVendorRequest("vnd-001", "Other Supplier"), "user-1");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("already exists");
    }

    [Fact]
    public async Task CreateVendor_ShouldSetAllFields()
    {
        var request = new CreateVendorRequest(
            "VND-FULL", "Full Vendor",
            ContactPerson: "Jane Doe",
            Email: "jane@vendor.com",
            Phone: "+9876543210",
            Website: "https://vendor.com",
            TaxId: "VND-TAX-456",
            AddressLine1: "456 Supply Rd",
            City: "Vendorville",
            Country: "UK",
            Currency: "GBP",
            BankName: "Royal Bank",
            BankAccountNumber: "1234567890",
            BankRoutingNumber: "ROUTING-123",
            BankSwiftCode: "RBOS",
            LeadTimeDays: 14,
            Notes: "Preferred supplier");

        var result = await _sut.CreateVendorAsync(request, "user-1");

        result.IsSuccess.Should().BeTrue();
        var vendor = result.Value;
        vendor.ContactPerson.Should().Be("Jane Doe");
        vendor.BankName.Should().Be("Royal Bank");
        vendor.BankSwiftCode.Should().Be("RBOS");
        vendor.LeadTimeDays.Should().Be(14);
        vendor.Currency.Should().Be("GBP");
    }

    [Fact]
    public async Task UpdateVendor_WhenExists_ShouldReturnSuccess()
    {
        var createResult = await _sut.CreateVendorAsync(
            new CreateVendorRequest("VND-001", "Supplier Co"), "user-1");
        var id = createResult.Value.Id;

        var updateRequest = new UpdateVendorRequest("Supplier Co Updated", LeadTimeDays: 7);
        var result = await _sut.UpdateVendorAsync(id, updateRequest, "user-2");

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("Supplier Co Updated");
        result.Value.LeadTimeDays.Should().Be(7);
    }

    [Fact]
    public async Task UpdateVendor_WhenNotExists_ShouldReturnFailure()
    {
        var result = await _sut.UpdateVendorAsync(999,
            new UpdateVendorRequest("Test"), "user-1");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("not found");
    }

    [Fact]
    public async Task DeleteVendor_WhenExists_ShouldSoftDelete()
    {
        var createResult = await _sut.CreateVendorAsync(
            new CreateVendorRequest("VND-001", "Supplier Co"), "user-1");
        var id = createResult.Value.Id;

        var result = await _sut.DeleteVendorAsync(id, "user-1");

        result.IsSuccess.Should().BeTrue();
        var vendor = await _sut.GetVendorByIdAsync(id);
        vendor.Should().BeNull();
    }

    [Fact]
    public async Task ActivateVendor_ShouldSetIsActiveTrue()
    {
        var createResult = await _sut.CreateVendorAsync(
            new CreateVendorRequest("VND-001", "Supplier Co"), "user-1");
        var id = createResult.Value.Id;
        await _sut.DeactivateVendorAsync(id, "user-1");

        var result = await _sut.ActivateVendorAsync(id, "user-1");

        result.IsSuccess.Should().BeTrue();
        var vendor = await _sut.GetVendorByIdAsync(id);
        vendor!.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task DeactivateVendor_ShouldSetIsActiveFalse()
    {
        var createResult = await _sut.CreateVendorAsync(
            new CreateVendorRequest("VND-001", "Supplier Co"), "user-1");
        var id = createResult.Value.Id;

        var result = await _sut.DeactivateVendorAsync(id, "user-1");

        result.IsSuccess.Should().BeTrue();
        var vendor = await _sut.GetVendorByIdAsync(id);
        vendor!.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task GetVendors_WithSearchTerm_ShouldFilterResults()
    {
        await _sut.CreateVendorAsync(new CreateVendorRequest("VND-001", "Alpha Supplies"), "user-1");
        await _sut.CreateVendorAsync(new CreateVendorRequest("VND-002", "Beta Parts"), "user-1");

        var result = await _sut.GetVendorsAsync(new GetVendorsQuery { SearchTerm = "alpha" });

        result.Items.Should().HaveCount(1);
        result.Items[0].Name.Should().Be("Alpha Supplies");
    }

    [Fact]
    public async Task GetVendors_WithPagination_ShouldReturnCorrectPage()
    {
        for (int i = 1; i <= 5; i++)
            await _sut.CreateVendorAsync(new CreateVendorRequest($"VND-{i:D3}", $"Vendor {i}"), "user-1");

        var result = await _sut.GetVendorsAsync(new GetVendorsQuery { Page = 1, PageSize = 3, SortBy = "code" });

        result.Items.Should().HaveCount(3);
        result.TotalCount.Should().Be(5);
        result.HasNextPage.Should().BeTrue();
        result.HasPreviousPage.Should().BeFalse();
    }
}
