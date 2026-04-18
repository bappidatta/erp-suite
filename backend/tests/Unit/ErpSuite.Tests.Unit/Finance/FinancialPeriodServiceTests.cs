using ErpSuite.Modules.Finance.Application.FinancialPeriods.Dtos;
using ErpSuite.Modules.Finance.Domain.Entities;
using ErpSuite.Modules.Finance.Infrastructure.Services;
using ErpSuite.Tests.Unit.Helpers;
using FluentAssertions;

namespace ErpSuite.Tests.Unit.Finance;

public sealed class FinancialPeriodServiceTests
{
    private readonly FinancialPeriodService _sut;

    public FinancialPeriodServiceTests()
    {
        _sut = new FinancialPeriodService(TestDbContextFactory.Create());
    }

    [Fact]
    public async Task CreateFinancialPeriod_WithOverlap_ShouldReturnFailure()
    {
        await _sut.CreateFinancialPeriodAsync(
            new CreateFinancialPeriodRequest("FY 2026", new DateTime(2026, 1, 1), new DateTime(2026, 1, 31)),
            "user-1");

        var result = await _sut.CreateFinancialPeriodAsync(
            new CreateFinancialPeriodRequest("FY 2026-2", new DateTime(2026, 1, 15), new DateTime(2026, 2, 15)),
            "user-1");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("overlaps");
    }

    [Fact]
    public async Task CloseFinancialPeriod_ShouldSetStatusClosed()
    {
        var created = await _sut.CreateFinancialPeriodAsync(
            new CreateFinancialPeriodRequest("FY 2026", new DateTime(2026, 1, 1), new DateTime(2026, 1, 31)),
            "user-1");

        var result = await _sut.CloseFinancialPeriodAsync(created.Value.Id, "user-2");

        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be(FinancialPeriodStatus.Closed);
        result.Value.ClosedBy.Should().Be("user-2");
    }
}
