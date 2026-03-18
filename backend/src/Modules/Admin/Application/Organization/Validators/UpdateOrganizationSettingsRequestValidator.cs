using ErpSuite.Modules.Admin.Application.Organization.Dtos;
using FluentValidation;

namespace ErpSuite.Modules.Admin.Application.Organization.Validators;

public class UpdateOrganizationSettingsRequestValidator : AbstractValidator<UpdateOrganizationSettingsRequest>
{
    public UpdateOrganizationSettingsRequestValidator()
    {
        RuleFor(x => x.CompanyName)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(x => x.LegalName)
            .MaximumLength(256)
            .When(x => x.LegalName is not null);

        RuleFor(x => x.RegistrationNumber)
            .MaximumLength(100)
            .When(x => x.RegistrationNumber is not null);

        RuleFor(x => x.Address)
            .MaximumLength(500)
            .When(x => x.Address is not null);

        RuleFor(x => x.Phone)
            .MaximumLength(50)
            .When(x => x.Phone is not null);

        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.Website)
            .MaximumLength(256)
            .When(x => x.Website is not null);

        RuleFor(x => x.Currency)
            .NotEmpty()
            .Length(3);

        RuleFor(x => x.FiscalYearStart)
            .MaximumLength(10)
            .When(x => x.FiscalYearStart is not null);

        RuleFor(x => x.DateFormat)
            .MaximumLength(50)
            .When(x => x.DateFormat is not null);

        RuleFor(x => x.TimeZone)
            .MaximumLength(100)
            .When(x => x.TimeZone is not null);
    }
}