using ErpSuite.Modules.Procurement.Application.Vendors.Dtos;
using FluentValidation;

namespace ErpSuite.Modules.Procurement.Application.Vendors.Validators;

public class UpdateVendorRequestValidator : AbstractValidator<UpdateVendorRequest>
{
    public UpdateVendorRequestValidator()
    {
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
        RuleFor(x => x.Currency).NotEmpty().MaximumLength(3);
        RuleFor(x => x.PaymentTerms).MaximumLength(100);
        RuleFor(x => x.BankName).MaximumLength(256);
        RuleFor(x => x.BankAccountNumber).MaximumLength(50);
        RuleFor(x => x.BankRoutingNumber).MaximumLength(50);
        RuleFor(x => x.BankSwiftCode).MaximumLength(20);
        RuleFor(x => x.LeadTimeDays).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Notes).MaximumLength(2000);
    }
}
