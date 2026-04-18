using ErpSuite.Modules.Admin.Application.NumberSequences.Dtos;
using ErpSuite.Modules.Admin.Domain.Entities;
using ErpSuite.Modules.Admin.Infrastructure.Services;
using ErpSuite.Tests.Unit.Helpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace ErpSuite.Tests.Unit.Admin;

public sealed class NumberSequenceServiceTests
{
    private readonly ErpSuite.Modules.Admin.Infrastructure.Persistence.ErpDbContext _dbContext;
    private readonly NumberSequenceService _sut;

    public NumberSequenceServiceTests()
    {
        _dbContext = TestDbContextFactory.Create();
        _sut = new NumberSequenceService(_dbContext);
    }

    [Fact]
    public async Task CreateNumberSequence_WithValidRequest_ShouldReturnSuccess()
    {
        var request = new CreateNumberSequenceRequest("Finance", "JournalEntry", "JE-", 1, 1, 4, 1);

        var result = await _sut.CreateNumberSequenceAsync(request, "user-1");

        result.IsSuccess.Should().BeTrue();
        result.Value.Module.Should().Be("Finance");
        result.Value.DocumentType.Should().Be("JournalEntry");
        result.Value.Preview.Should().Be("JE-0001");
    }

    [Fact]
    public async Task CreateNumberSequence_WithDuplicateModuleAndDocumentType_ShouldReturnFailure()
    {
        await _sut.CreateNumberSequenceAsync(new CreateNumberSequenceRequest("Finance", "JournalEntry", "JE-"), "user-1");

        var result = await _sut.CreateNumberSequenceAsync(new CreateNumberSequenceRequest("finance", "journalentry", "JR-"), "user-1");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("already exists");
    }

    [Fact]
    public async Task GetNextNumberAsync_ShouldAdvanceSequence()
    {
        await _sut.CreateNumberSequenceAsync(new CreateNumberSequenceRequest("Finance", "JournalEntry", "JE-", 1, 1, 4, 1), "user-1");

        var first = await _sut.GetNextNumberAsync("Finance", "JournalEntry", "user-1");
        var second = await _sut.GetNextNumberAsync("Finance", "JournalEntry", "user-1");

        first.Value.Should().Be("JE-0001");
        second.Value.Should().Be("JE-0002");
    }

    [Fact]
    public async Task GetPreviewAsync_WithAnnualReset_ShouldResetAtNewYear()
    {
        var create = await _sut.CreateNumberSequenceAsync(
            new CreateNumberSequenceRequest("Finance", "PeriodClose", "PC-", 1, 5, 3, 1, ResetPolicy: NumberSequenceResetPolicy.Annual),
            "user-1");

        var preview = await _sut.GetPreviewAsync(create.Value.Id);

        preview.IsSuccess.Should().BeTrue();
        preview.Value.Should().Be("PC-005");
    }

    [Fact]
    public async Task GetPreviewAsync_ShouldNotMutateSequenceState_WhenAnnualResetApplies()
    {
        var create = await _sut.CreateNumberSequenceAsync(
            new CreateNumberSequenceRequest("Finance", "YearEnd", "YE-", 1, 5, 3, 1, ResetPolicy: NumberSequenceResetPolicy.Annual),
            "user-1");

        var sequence = await _dbContext.NumberSequences.FirstAsync(x => x.Id == create.Value.Id);
        sequence.GetType().GetProperty(nameof(NumberSequence.LastResetOn))!.SetValue(sequence, new DateTime(DateTime.UtcNow.Year - 1, 12, 31));
        await _dbContext.SaveChangesAsync();

        var preview = await _sut.GetPreviewAsync(create.Value.Id);
        var afterPreview = await _sut.GetNumberSequenceByIdAsync(create.Value.Id);
        var nextNumber = await _sut.GetNextNumberAsync("Finance", "YearEnd", "user-1");
        var afterConsumption = await _sut.GetNumberSequenceByIdAsync(create.Value.Id);

        preview.IsSuccess.Should().BeTrue();
        preview.Value.Should().Be("YE-001");
        afterPreview!.NextNumber.Should().Be(5);
        nextNumber.Value.Should().Be("YE-001");
        afterConsumption!.NextNumber.Should().Be(2);
    }
}
