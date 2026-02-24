using ContactManager.Core.Domain.Validators;

using FluentValidation;

namespace ContactManager.WebSite.ViewModels.Address;

public interface IAddressInput {
    int? StreetNumber { get; }
    string? StreetName { get; }
    string? CityName { get; }
    string? PostalCode { get; }
}

public static class AddressValidationRules {
    public static void ApplyAddressRules<T>(this AbstractValidator<T> validator)
        where T : IAddressInput {
        validator.RuleFor(vm => vm.StreetNumber)
            .NotNull()
                .WithMessage("Please provide a street number.")
            .SetValidator(new StreetNumberValidator());

        validator.RuleFor(vm => vm.StreetName)
            .NotNull()
                .WithMessage("Please provide a street name.")
            .SetValidator(new StreetNameValidator());

        validator.RuleFor(vm => vm.CityName)
            .NotNull()
                .WithMessage("Please provide a city name.")
            .SetValidator(new CityNameValidator());

        validator.RuleFor(vm => vm.PostalCode)
            .NotNull()
                .WithMessage("Please provide a postal code.")
            .SetValidator(new PostalCodeValidator());
    }
}