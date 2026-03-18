using ErpSuite.Modules.HR.Application.Employees.Dtos;
using FluentValidation;

namespace ErpSuite.Modules.HR.Application.Employees.Validators;

public class CreateEmployeeRequestValidator : AbstractValidator<CreateEmployeeRequest>
{
    public CreateEmployeeRequestValidator()
    {
        RuleFor(x => x.EmployeeNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(128);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(128);
        RuleFor(x => x.Email).EmailAddress().MaximumLength(256).When(x => !string.IsNullOrWhiteSpace(x.Email));
        RuleFor(x => x.Phone).MaximumLength(50);
        RuleFor(x => x.Designation).MaximumLength(256);
        RuleFor(x => x.Status).InclusiveBetween(1, 4);
        RuleFor(x => x.EmploymentType).InclusiveBetween(1, 4);
        RuleFor(x => x.DateOfJoining).NotEmpty().LessThanOrEqualTo(DateTime.Today.AddDays(1));
        RuleFor(x => x.Notes).MaximumLength(2000);
    }
}
