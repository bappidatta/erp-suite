using ErpSuite.Modules.Inventory.Application.Items.Dtos;
using FluentValidation;

namespace ErpSuite.Modules.Inventory.Application.Items.Validators;

public class CreateItemRequestValidator : AbstractValidator<CreateItemRequest>
{
    public CreateItemRequestValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(256);
        RuleFor(x => x.Description).MaximumLength(1000);
        RuleFor(x => x.UomId).GreaterThan(0).WithMessage("A valid UOM must be selected.");
        RuleFor(x => x.Type).InclusiveBetween(1, 4);
        RuleFor(x => x.ValuationMethod).InclusiveBetween(1, 3);
        RuleFor(x => x.StandardCost).GreaterThanOrEqualTo(0);
        RuleFor(x => x.SalePrice).GreaterThanOrEqualTo(0);
        RuleFor(x => x.ReorderLevel).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Barcode).MaximumLength(100);
        RuleFor(x => x.Notes).MaximumLength(2000);
    }
}
