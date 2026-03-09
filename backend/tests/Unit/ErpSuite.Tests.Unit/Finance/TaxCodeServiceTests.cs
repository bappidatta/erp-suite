using ErpSuite.Modules.Finance.Application.TaxCodes.Dtos;
using ErpSuite.Modules.Finance.Domain.Entities;
using ErpSuite.Modules.Finance.Infrastructure.Services;
using ErpSuite.Tests.Unit.Helpers;
using FluentAssertions;

namespace ErpSuite.Tests.Unit.Finance;

public class TaxCodeServiceTests
{
    private readonly TaxCodeService _sut;
    private readonly ErpSuite.Modules.Admin.Infrastructure.Persistence.ErpDbContext _dbContext;

    public TaxCodeServiceTests()
    {
        _dbContext = TestDbContextFactory.Create();
        _sut = new TaxCodeService(_dbContext);
    }

    [Fact]
    public async Task CreateTaxCode_WithValidRequest_ShouldReturnSuccess()
    {
        var request = new CreateTaxCodeRequest("GST", "Goods & Services Tax", 10m, TaxType.Percentage);

        var result = await _sut.CreateTaxCodeAsync(request, "user-1");

        result.IsSuccess.Should().BeTrue();
        result.Value.Code.Should().Be("GST");
        result.Value.Name.Should().Be("Goods & Services Tax");
        result.Value.Rate.Should().Be(10m);
        result.Value.Type.Should().Be(TaxType.Percentage);
        result.Value.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task CreateTaxCode_WithDuplicateCode_ShouldReturnFailure()
    {
        await _sut.CreateTaxCodeAsync(new CreateTaxCodeRequest("GST", "Tax 1", 10m), "user-1");

        var result = await _sut.CreateTaxCodeAsync(new CreateTaxCodeRequest("gst", "Tax 2", 5m), "user-1");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("already exists");
    }

    [Fact]
    public async Task CreateTaxCode_WithFixedType_ShouldAllowHighRate()
    {
        var request = new CreateTaxCodeRequest("FLAT", "Flat Fee", 250m, TaxType.Fixed);

        var result = await _sut.CreateTaxCodeAsync(request, "user-1");

        result.IsSuccess.Should().BeTrue();
        result.Value.Rate.Should().Be(250m);
        result.Value.Type.Should().Be(TaxType.Fixed);
    }

    [Fact]
    public async Task CreateTaxCode_ShouldSetApplicabilityFields()
    {
        var request = new CreateTaxCodeRequest("SALES-ONLY", "Sales Only Tax", 5m,
            AppliesToSales: true, AppliesToPurchases: false);

        var result = await _sut.CreateTaxCodeAsync(request, "user-1");

        result.IsSuccess.Should().BeTrue();
        result.Value.AppliesToSales.Should().BeTrue();
        result.Value.AppliesToPurchases.Should().BeFalse();
    }

    [Fact]
    public async Task UpdateTaxCode_WhenExists_ShouldReturnSuccess()
    {
        var createResult = await _sut.CreateTaxCodeAsync(
            new CreateTaxCodeRequest("GST", "Old Name", 10m), "user-1");
        var id = createResult.Value.Id;

        var result = await _sut.UpdateTaxCodeAsync(id,
            new UpdateTaxCodeRequest("Updated Name", 12m, TaxType.Percentage, "Updated description"), "user-2");

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("Updated Name");
        result.Value.Rate.Should().Be(12m);
        result.Value.Description.Should().Be("Updated description");
    }

    [Fact]
    public async Task UpdateTaxCode_WhenNotExists_ShouldReturnFailure()
    {
        var result = await _sut.UpdateTaxCodeAsync(999,
            new UpdateTaxCodeRequest("Test", 5m), "user-1");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("not found");
    }

    [Fact]
    public async Task DeleteTaxCode_WhenExists_ShouldSoftDelete()
    {
        var createResult = await _sut.CreateTaxCodeAsync(
            new CreateTaxCodeRequest("GST", "Tax Code", 10m), "user-1");
        var id = createResult.Value.Id;

        var result = await _sut.DeleteTaxCodeAsync(id, "user-1");

        result.IsSuccess.Should().BeTrue();
        var taxCode = await _sut.GetTaxCodeByIdAsync(id);
        taxCode.Should().BeNull();
    }

    [Fact]
    public async Task ActivateTaxCode_ShouldSetIsActiveTrue()
    {
        var createResult = await _sut.CreateTaxCodeAsync(
            new CreateTaxCodeRequest("GST", "Tax Code", 10m), "user-1");
        var id = createResult.Value.Id;
        await _sut.DeactivateTaxCodeAsync(id, "user-1");

        var result = await _sut.ActivateTaxCodeAsync(id, "user-1");

        result.IsSuccess.Should().BeTrue();
        var taxCode = await _sut.GetTaxCodeByIdAsync(id);
        taxCode!.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task DeactivateTaxCode_ShouldSetIsActiveFalse()
    {
        var createResult = await _sut.CreateTaxCodeAsync(
            new CreateTaxCodeRequest("GST", "Tax Code", 10m), "user-1");
        var id = createResult.Value.Id;

        var result = await _sut.DeactivateTaxCodeAsync(id, "user-1");

        result.IsSuccess.Should().BeTrue();
        var taxCode = await _sut.GetTaxCodeByIdAsync(id);
        taxCode!.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task GetTaxCodes_WithSearchTerm_ShouldFilterResults()
    {
        await _sut.CreateTaxCodeAsync(new CreateTaxCodeRequest("GST", "Goods Tax", 10m), "user-1");
        await _sut.CreateTaxCodeAsync(new CreateTaxCodeRequest("VAT", "Value Added Tax", 20m), "user-1");

        var result = await _sut.GetTaxCodesAsync(new GetTaxCodesQuery { SearchTerm = "vat" });

        result.Items.Should().HaveCount(1);
        result.Items[0].Code.Should().Be("VAT");
    }

    [Fact]
    public async Task GetTaxCodes_WithAppliesToFilter_ShouldFilterResults()
    {
        await _sut.CreateTaxCodeAsync(new CreateTaxCodeRequest("SALES", "Sales Tax", 5m,
            AppliesToSales: true, AppliesToPurchases: false), "user-1");
        await _sut.CreateTaxCodeAsync(new CreateTaxCodeRequest("BOTH", "Both Tax", 10m,
            AppliesToSales: true, AppliesToPurchases: true), "user-1");

        var result = await _sut.GetTaxCodesAsync(new GetTaxCodesQuery { AppliesToPurchases = true });

        result.Items.Should().HaveCount(1);
        result.Items[0].Code.Should().Be("BOTH");
    }
}
