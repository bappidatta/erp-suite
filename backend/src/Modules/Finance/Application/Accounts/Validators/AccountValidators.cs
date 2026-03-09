using ErpSuite.Modules.Finance.Application.Accounts.Dtos;
using FluentValidation;

namespace ErpSuite.Modules.Finance.Application.Accounts.Validators;

public class CreateAccountRequestValidator : AbstractValidator<CreateAccountRequest>
{
    public CreateAccountRequestValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(256);
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.Type).IsInEnum();
    }
}

public class UpdateAccountRequestValidator : AbstractValidator<UpdateAccountRequest>
{
    public UpdateAccountRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(256);
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.Type).IsInEnum();
    }
}
