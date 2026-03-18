using ErpSuite.Modules.Admin.Application.Roles.Dtos;
using FluentValidation;

namespace ErpSuite.Modules.Admin.Application.Roles.Validators;

public class UpdateRoleRequestValidator : AbstractValidator<UpdateRoleRequest>
{
    public UpdateRoleRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(256)
            .When(x => x.Description is not null);
    }
}