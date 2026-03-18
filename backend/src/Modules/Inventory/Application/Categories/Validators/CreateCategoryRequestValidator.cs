using ErpSuite.Modules.Inventory.Application.Categories.Dtos;
using FluentValidation;

namespace ErpSuite.Modules.Inventory.Application.Categories.Validators;

public class CreateCategoryRequestValidator : AbstractValidator<CreateCategoryRequest>
{
    public CreateCategoryRequestValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(256);
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.ParentCategoryId).GreaterThan(0).When(x => x.ParentCategoryId.HasValue);
    }
}
