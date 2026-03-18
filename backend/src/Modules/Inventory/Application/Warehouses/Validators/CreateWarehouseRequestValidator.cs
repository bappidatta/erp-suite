using ErpSuite.Modules.Inventory.Application.Warehouses.Dtos;
using FluentValidation;

namespace ErpSuite.Modules.Inventory.Application.Warehouses.Validators;

public class CreateWarehouseRequestValidator : AbstractValidator<CreateWarehouseRequest>
{
    public CreateWarehouseRequestValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(256);
        RuleFor(x => x.Location).MaximumLength(256);
        RuleFor(x => x.Address).MaximumLength(500);
        RuleFor(x => x.ContactPerson).MaximumLength(256);
        RuleFor(x => x.Phone).MaximumLength(50);
        RuleFor(x => x.Notes).MaximumLength(1000);
    }
}
