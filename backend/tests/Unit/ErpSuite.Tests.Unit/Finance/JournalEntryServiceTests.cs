using ErpSuite.Modules.Admin.Infrastructure.Persistence;
using ErpSuite.Modules.Finance.Application.Accounts.Dtos;
using ErpSuite.Modules.Finance.Application.FinancialPeriods.Dtos;
using ErpSuite.Modules.Finance.Application.JournalEntries.Dtos;
using ErpSuite.Modules.Finance.Domain.Entities;
using ErpSuite.Modules.Finance.Infrastructure.Services;
using ErpSuite.Tests.Unit.Helpers;
using FluentAssertions;

namespace ErpSuite.Tests.Unit.Finance;

public sealed class JournalEntryServiceTests
{
    private readonly ErpDbContext _dbContext;
    private readonly AccountService _accountService;
    private readonly FinancialPeriodService _financialPeriodService;
    private readonly JournalEntryService _sut;

    public JournalEntryServiceTests()
    {
        _dbContext = TestDbContextFactory.Create();
        _accountService = new AccountService(_dbContext);
        _financialPeriodService = new FinancialPeriodService(_dbContext);
        _sut = new JournalEntryService(_dbContext);
    }

    [Fact]
    public async Task CreateJournalEntry_WithBalancedLines_ShouldReturnSuccess()
    {
        var (cashAccountId, revenueAccountId) = await CreatePostableAccountsAsync();

        var result = await _sut.CreateJournalEntryAsync(
            new CreateJournalEntryRequest(
                DateTime.UtcNow.Date,
                "Sales booking",
                "REF-1",
                [
                    new JournalEntryLineRequest(1, cashAccountId, 100m, 0m, "Debit cash"),
                    new JournalEntryLineRequest(2, revenueAccountId, 0m, 100m, "Credit revenue")
                ]),
            "user-1");

        result.IsSuccess.Should().BeTrue();
        result.Value.TotalDebit.Should().Be(100m);
        result.Value.TotalCredit.Should().Be(100m);
        result.Value.Status.Should().Be(JournalEntryStatus.Draft);
    }

    [Fact]
    public async Task CreateJournalEntry_WithUnbalancedLines_ShouldReturnFailure()
    {
        var (cashAccountId, revenueAccountId) = await CreatePostableAccountsAsync();

        var result = await _sut.CreateJournalEntryAsync(
            new CreateJournalEntryRequest(
                DateTime.UtcNow.Date,
                "Broken entry",
                Lines:
                [
                    new JournalEntryLineRequest(1, cashAccountId, 100m, 0m),
                    new JournalEntryLineRequest(2, revenueAccountId, 0m, 90m)
                ]),
            "user-1");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Total debit must equal total credit");
    }

    [Fact]
    public async Task PostJournalEntry_WithOpenPeriod_ShouldReturnSuccess()
    {
        var (cashAccountId, revenueAccountId) = await CreatePostableAccountsAsync();
        await _financialPeriodService.CreateFinancialPeriodAsync(
            new CreateFinancialPeriodRequest("FY 2026", DateTime.UtcNow.Date.AddDays(-5), DateTime.UtcNow.Date.AddDays(5)),
            "user-1");

        var create = await _sut.CreateJournalEntryAsync(
            new CreateJournalEntryRequest(
                DateTime.UtcNow.Date,
                "Post me",
                Lines:
                [
                    new JournalEntryLineRequest(1, cashAccountId, 250m, 0m),
                    new JournalEntryLineRequest(2, revenueAccountId, 0m, 250m)
                ]),
            "user-1");

        var result = await _sut.PostJournalEntryAsync(create.Value.Id, "user-1");

        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be(JournalEntryStatus.Posted);
        result.Value.PostedBy.Should().Be("user-1");
    }

    [Fact]
    public async Task PostJournalEntry_WithoutOpenPeriod_ShouldReturnFailure()
    {
        var (cashAccountId, revenueAccountId) = await CreatePostableAccountsAsync();
        var create = await _sut.CreateJournalEntryAsync(
            new CreateJournalEntryRequest(
                DateTime.UtcNow.Date,
                "No period",
                Lines:
                [
                    new JournalEntryLineRequest(1, cashAccountId, 250m, 0m),
                    new JournalEntryLineRequest(2, revenueAccountId, 0m, 250m)
                ]),
            "user-1");

        var result = await _sut.PostJournalEntryAsync(create.Value.Id, "user-1");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("accounting period");
    }

    private async Task<(long CashAccountId, long RevenueAccountId)> CreatePostableAccountsAsync()
    {
        var cash = await _accountService.CreateAccountAsync(new CreateAccountRequest("1000", "Cash", AccountType.Asset), "user-1");
        var revenue = await _accountService.CreateAccountAsync(new CreateAccountRequest("4000", "Revenue", AccountType.Revenue), "user-1");

        return (cash.Value.Id, revenue.Value.Id);
    }
}
