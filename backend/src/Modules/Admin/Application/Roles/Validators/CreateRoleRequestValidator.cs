using ErpSuite.Modules.Admin.Application.Roles.Dtos;
using FluentValidation;

namespace ErpSuite.Modules.Admin.Application.Roles.Validators;

public class CreateRoleRequestValidator : AbstractValidator<CreateRoleRequest>
{
    public CreateRoleRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(256)
            .When(x => x.Description is not null);
    }
}