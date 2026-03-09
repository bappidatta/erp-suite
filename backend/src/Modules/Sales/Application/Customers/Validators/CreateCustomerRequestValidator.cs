using ErpSuite.Modules.Sales.Application.Customers.Dtos;
using FluentValidation;

namespace ErpSuite.Modules.Sales.Application.Customers.Validators;

public class CreateCustomerRequestValidator : AbstractValidator<CreateCustomerRequest>
{
    public CreateCustomerRequestValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(256);
        RuleFor(x => x.ContactPerson).MaximumLength(256);
        RuleFor(x => x.Email).EmailAddress().MaximumLength(256).When(x => !string.IsNullOrWhiteSpace(x.Email));
        RuleFor(x => x.Phone).MaximumLength(50);
        RuleFor(x => x.Website).MaximumLength(500);
        RuleFor(x => x.TaxId).MaximumLength(100);
        RuleFor(x => x.AddressLine1).MaximumLength(256);
        RuleFor(x => x.AddressLine2).MaximumLength(256);
        RuleFor(x => x.City).MaximumLength(100);
        RuleFor(x => x.State).MaximumLength(100);
        RuleFor(x => x.PostalCode).MaximumLength(20);
        RuleFor(x => x.Country).MaximumLength(100);
        RuleFor(x => x.CreditLimit).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Currency).NotEmpty().MaximumLength(3);
        RuleFor(x => x.PaymentTerms).MaximumLength(100);
        RuleFor(x => x.Notes).MaximumLength(2000);
    }
}
