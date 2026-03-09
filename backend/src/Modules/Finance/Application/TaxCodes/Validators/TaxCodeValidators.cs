using ErpSuite.Modules.Finance.Application.TaxCodes.Dtos;
using ErpSuite.Modules.Finance.Domain.Entities;
using FluentValidation;

namespace ErpSuite.Modules.Finance.Application.TaxCodes.Validators;

public class CreateTaxCodeRequestValidator : AbstractValidator<CreateTaxCodeRequest>
{
    public CreateTaxCodeRequestValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(256);
        RuleFor(x => x.Rate).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Rate).LessThanOrEqualTo(100)
            .When(x => x.Type == TaxType.Percentage)
            .WithMessage("Percentage rate must be between 0 and 100.");
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.Type).IsInEnum();
    }
}

public class UpdateTaxCodeRequestValidator : AbstractValidator<UpdateTaxCodeRequest>
{
    public UpdateTaxCodeRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(256);
        RuleFor(x => x.Rate).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Rate).LessThanOrEqualTo(100)
            .When(x => x.Type == TaxType.Percentage)
            .WithMessage("Percentage rate must be between 0 and 100.");
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.Type).IsInEnum();
    }
}
