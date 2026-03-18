using ErpSuite.Modules.Inventory.Application.UOMs.Dtos;
using FluentValidation;

namespace ErpSuite.Modules.Inventory.Application.UOMs.Validators;

public class CreateUomRequestValidator : AbstractValidator<CreateUomRequest>
{
    public CreateUomRequestValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).MaximumLength(256);
    }
}
