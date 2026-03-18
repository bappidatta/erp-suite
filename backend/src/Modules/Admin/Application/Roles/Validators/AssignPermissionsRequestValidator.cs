using ErpSuite.Modules.Admin.Application.Roles.Dtos;
using FluentValidation;

namespace ErpSuite.Modules.Admin.Application.Roles.Validators;

public class AssignPermissionsRequestValidator : AbstractValidator<AssignPermissionsRequest>
{
    public AssignPermissionsRequestValidator()
    {
        RuleFor(x => x.PermissionIds)
            .NotNull();

        RuleForEach(x => x.PermissionIds)
            .GreaterThan(0);
    }
}