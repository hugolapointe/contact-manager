using FluentValidation;

using System.Text.RegularExpressions;

namespace ContactManager.Core.Domain.Validators;

public class StreetNumberValidator : AbstractValidator<int?> {

    public StreetNumberValidator() {
        RuleFor(streetNumber => streetNumber)
            .NotEmpty()
            .WithMessage("Please provide a Street Number.")
            .GreaterThan(0)
            .WithMessage("Please provide a positive Street Number.");
    }
}

public class StreetNameValidator : AbstractValidator<string?> {
    private const int STREET_NAME_LENGTH_MIN = 5;
    private const int STREET_NAME_LENGTH_MAX = 30;

    public StreetNameValidator() {
        Transform(streetName => streetName, streetName => streetName!.Trim())
            .NotEmpty()
            .WithMessage("Please provide a Street Name.")
            .Length(STREET_NAME_LENGTH_MIN, STREET_NAME_LENGTH_MAX)
            .WithMessage($"Please provide a Street Name between {STREET_NAME_LENGTH_MIN} and {STREET_NAME_LENGTH_MAX} characters.")
            .IsValidName()
            .WithMessage("Please provide a Street Name that contains only letters and spaces.");
    }
}

public class CityNameValidator : AbstractValidator<string?> {
    private const int CITY_NAME_LENGTH_MIN = 5;
    private const int CITY_NAME_LENGTH_MAX = 30;

    public CityNameValidator() {
        Transform(cityName => cityName, cityName => cityName!.Trim())
            .NotEmpty()
            .WithMessage("Please provide a City Name.")
            .Length(CITY_NAME_LENGTH_MIN, CITY_NAME_LENGTH_MAX)
            .WithMessage($"Please provide a City Name between {CITY_NAME_LENGTH_MIN} and {CITY_NAME_LENGTH_MAX} characters.")
            .IsValidName()
            .WithMessage("Please provide a City Name that contains only letters and spaces.");
    }
}

public class PostalCodeValidator : AbstractValidator<string?> {
    private const RegexOptions REGEX_OPTIONS = RegexOptions.IgnoreCase | RegexOptions.CultureInvariant;
    private const string REGEX_POSTAL_CODE = @"^[A-Z]\d[A-Z] ?\d[A-Z]\d$";

    public PostalCodeValidator() {
        Transform(postalCode => postalCode, postalCode => postalCode!.Trim().ToUpper())
            .NotEmpty()
                    .WithMessage("Please provide a Postal Code.")
                    .Matches(REGEX_POSTAL_CODE, REGEX_OPTIONS)
                    .WithMessage("Please provide a valid Postal Code.");
    }
}
