using ErpSuite.Modules.Admin.Application.Organization.Dtos;
using ErpSuite.Modules.Admin.Infrastructure.Services;
using ErpSuite.Tests.Unit.Helpers;
using FluentAssertions;

namespace ErpSuite.Tests.Unit.Admin;

public class OrganizationSettingsServiceTests
{
    private readonly OrganizationSettingsService _sut;
    private readonly ErpSuite.Modules.Admin.Infrastructure.Persistence.ErpDbContext _dbContext;

    public OrganizationSettingsServiceTests()
    {
        _dbContext = TestDbContextFactory.Create();
        _sut = new OrganizationSettingsService(_dbContext);
    }

    [Fact]
    public async Task GetAsync_WhenSettingsDoNotExist_ShouldReturnNull()
    {
        var result = await _sut.GetAsync();

        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_WhenSettingsDoNotExist_ShouldCreateSettings()
    {
        var request = new UpdateOrganizationSettingsRequest(
            "Acme Corp",
            LegalName: "Acme Corporation Ltd",
            Email: "info@acme.test",
            Currency: "EUR",
            TimeZone: "Europe/Paris");

        var result = await _sut.UpdateAsync(request, "user-1");

        result.IsSuccess.Should().BeTrue();
        result.Value.CompanyName.Should().Be("Acme Corp");
        result.Value.LegalName.Should().Be("Acme Corporation Ltd");
        result.Value.Email.Should().Be("info@acme.test");
        result.Value.Currency.Should().Be("EUR");
        result.Value.TimeZone.Should().Be("Europe/Paris");
    }

    [Fact]
    public async Task UpdateAsync_WhenSettingsExist_ShouldUpdateExistingRecord()
    {
        await _sut.UpdateAsync(new UpdateOrganizationSettingsRequest("Acme Corp", Currency: "USD"), "user-1");

        var result = await _sut.UpdateAsync(
            new UpdateOrganizationSettingsRequest("Acme Group", Address: "Main Street", Currency: "GBP"),
            "user-2");

        result.IsSuccess.Should().BeTrue();
        result.Value.CompanyName.Should().Be("Acme Group");
        result.Value.Address.Should().Be("Main Street");
        result.Value.Currency.Should().Be("GBP");

        var settingsCount = _dbContext.OrganizationSettings.Count();
        settingsCount.Should().Be(1);
    }

    [Fact]
    public async Task UploadLogoAsync_WithInvalidExtension_ShouldReturnFailure()
    {
        await using var stream = new MemoryStream([1, 2, 3]);

        var result = await _sut.UploadLogoAsync(stream, "logo.gif", "user-1");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Only JPG, PNG, and SVG files are allowed");
    }

    [Fact]
    public async Task UploadLogoAsync_WithFileOverLimit_ShouldReturnFailure()
    {
        await using var stream = new MemoryStream(new byte[5 * 1024 * 1024 + 1]);

        var result = await _sut.UploadLogoAsync(stream, "logo.png", "user-1");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("must not exceed 5MB");
    }

    [Fact]
    public async Task UploadLogoAsync_WithValidFile_ShouldPersistLogoPath()
    {
        await using var stream = new MemoryStream([1, 2, 3, 4]);

        var result = await _sut.UploadLogoAsync(stream, "logo.png", "user-1");

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().StartWith("/uploads/logos/");

        var filePath = Path.Combine(Directory.GetCurrentDirectory(), result.Value.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        File.Exists(filePath).Should().BeTrue();

        var settings = await _sut.GetAsync();
        settings.Should().NotBeNull();
        settings!.LogoPath.Should().Be(result.Value);

        File.Delete(filePath);
    }
}