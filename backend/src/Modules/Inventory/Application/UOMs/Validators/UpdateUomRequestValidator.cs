using ErpSuite.Modules.Inventory.Application.UOMs.Dtos;
using FluentValidation;

namespace ErpSuite.Modules.Inventory.Application.UOMs.Validators;

public class UpdateUomRequestValidator : AbstractValidator<UpdateUomRequest>
{
    public UpdateUomRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).MaximumLength(256);
    }
}
