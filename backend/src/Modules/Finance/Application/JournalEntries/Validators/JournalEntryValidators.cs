using ErpSuite.Modules.Finance.Application.JournalEntries.Dtos;
using FluentValidation;

namespace ErpSuite.Modules.Finance.Application.JournalEntries.Validators;

public sealed class CreateJournalEntryRequestValidator : AbstractValidator<CreateJournalEntryRequest>
{
    public CreateJournalEntryRequestValidator()
    {
        RuleFor(x => x.Description).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Reference).MaximumLength(100);
        RuleFor(x => x.Lines).NotNull().Must(x => x!.Count >= 2).WithMessage("At least two journal lines are required.");
        RuleForEach(x => x.Lines!).SetValidator(new JournalEntryLineRequestValidator());
    }
}

public sealed class UpdateJournalEntryRequestValidator : AbstractValidator<UpdateJournalEntryRequest>
{
    public UpdateJournalEntryRequestValidator()
    {
        RuleFor(x => x.Description).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Reference).MaximumLength(100);
        RuleFor(x => x.Lines).NotNull().Must(x => x!.Count >= 2).WithMessage("At least two journal lines are required.");
        RuleForEach(x => x.Lines!).SetValidator(new JournalEntryLineRequestValidator());
    }
}

public sealed class JournalEntryLineRequestValidator : AbstractValidator<JournalEntryLineRequest>
{
    public JournalEntryLineRequestValidator()
    {
        RuleFor(x => x.LineNumber).GreaterThan(0);
        RuleFor(x => x.AccountId).GreaterThan(0);
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x)
            .Must(x => (x.DebitAmount > 0m && x.CreditAmount == 0m) || (x.CreditAmount > 0m && x.DebitAmount == 0m))
            .WithMessage("Each line must contain either a debit amount or a credit amount.");
    }
}
