using ErpSuite.Modules.Finance.Application.FinancialPeriods.Dtos;
using FluentValidation;

namespace ErpSuite.Modules.Finance.Application.FinancialPeriods.Validators;

public sealed class CreateFinancialPeriodRequestValidator : AbstractValidator<CreateFinancialPeriodRequest>
{
    public CreateFinancialPeriodRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.EndDate).GreaterThanOrEqualTo(x => x.StartDate);
    }
}

public sealed class UpdateFinancialPeriodRequestValidator : AbstractValidator<UpdateFinancialPeriodRequest>
{
    public UpdateFinancialPeriodRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.EndDate).GreaterThanOrEqualTo(x => x.StartDate);
    }
}
