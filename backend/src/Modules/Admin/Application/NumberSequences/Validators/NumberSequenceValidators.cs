using ErpSuite.Modules.Admin.Application.NumberSequences.Dtos;
using FluentValidation;

namespace ErpSuite.Modules.Admin.Application.NumberSequences.Validators;

public sealed class CreateNumberSequenceRequestValidator : AbstractValidator<CreateNumberSequenceRequest>
{
    public CreateNumberSequenceRequestValidator()
    {
        RuleFor(x => x.Module).NotEmpty().MaximumLength(100);
        RuleFor(x => x.DocumentType).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Prefix).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Suffix).MaximumLength(50);
        RuleFor(x => x.StartingNumber).GreaterThan(0);
        RuleFor(x => x.NextNumber).GreaterThan(0);
        RuleFor(x => x.NextNumber).GreaterThanOrEqualTo(x => x.StartingNumber);
        RuleFor(x => x.PaddingLength).InclusiveBetween(1, 12);
        RuleFor(x => x.IncrementBy).InclusiveBetween(1, 100);
        RuleFor(x => x.ResetPolicy).IsInEnum();
    }
}

public sealed class UpdateNumberSequenceRequestValidator : AbstractValidator<UpdateNumberSequenceRequest>
{
    public UpdateNumberSequenceRequestValidator()
    {
        RuleFor(x => x.Module).NotEmpty().MaximumLength(100);
        RuleFor(x => x.DocumentType).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Prefix).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Suffix).MaximumLength(50);
        RuleFor(x => x.StartingNumber).GreaterThan(0);
        RuleFor(x => x.NextNumber).GreaterThan(0);
        RuleFor(x => x.NextNumber).GreaterThanOrEqualTo(x => x.StartingNumber);
        RuleFor(x => x.PaddingLength).InclusiveBetween(1, 12);
        RuleFor(x => x.IncrementBy).InclusiveBetween(1, 100);
        RuleFor(x => x.ResetPolicy).IsInEnum();
    }
}
