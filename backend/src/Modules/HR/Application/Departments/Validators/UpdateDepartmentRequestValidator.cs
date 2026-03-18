using ErpSuite.Modules.HR.Application.Departments.Dtos;
using FluentValidation;

namespace ErpSuite.Modules.HR.Application.Departments.Validators;

public class UpdateDepartmentRequestValidator : AbstractValidator<UpdateDepartmentRequest>
{
    public UpdateDepartmentRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(256);
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.ParentDepartmentId).GreaterThan(0).When(x => x.ParentDepartmentId.HasValue);
    }
}
