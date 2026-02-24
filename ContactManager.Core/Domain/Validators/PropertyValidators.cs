using FluentValidation;

using System.Text.RegularExpressions;

namespace ContactManager.Core.Domain.Validators {

public static class CommonValidationRules {
    private const RegexOptions REGEX_OPTIONS = RegexOptions.IgnoreCase | RegexOptions.CultureInvariant;

    private const string REGEX_VALID_PERSON_NAME = @"^\p{L}+([ '-]\p{L}+)*$";
    private const string REGEX_VALID_STREET_NAME = @"^[\p{L}0-9]+([ .,'/-][\p{L}0-9]+)*$";

    public static IRuleBuilderOptions<T, string?> IsValidPersonName<T>(
        this IRuleBuilder<T, string?> ruleBuilder) {
        return ruleBuilder.Matches(REGEX_VALID_PERSON_NAME, REGEX_OPTIONS)
            .WithMessage("Please provide a valid name.");
    }

    public static IRuleBuilderOptions<T, string?> IsValidStreetName<T>(
        this IRuleBuilder<T, string?> ruleBuilder) {
        return ruleBuilder.Matches(REGEX_VALID_STREET_NAME, REGEX_OPTIONS)
            .WithMessage("Please provide a valid street name.");
    }
}

public class FirstNameValidator : AbstractValidator<string?> {
    private const int FIRST_NAME_LENGTH_MIN = 2;
    private const int FIRST_NAME_LENGTH_MAX = 30;

    public FirstNameValidator() {
        RuleFor(firstName => firstName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Please provide a First Name.")
            .Must(firstName => {
                var trimmed = firstName!.Trim();
                return trimmed.Length >= FIRST_NAME_LENGTH_MIN && trimmed.Length <= FIRST_NAME_LENGTH_MAX;
            })
            .WithMessage($"Please provide a First Name between {FIRST_NAME_LENGTH_MIN} and {FIRST_NAME_LENGTH_MAX} characters.")
            .Must(firstName => firstName!.Trim() == firstName)
            .WithMessage("Please remove leading and trailing spaces from First Name.")
            .IsValidPersonName()
            .WithMessage("Please provide a First Name that contains only letters.");
    }
}

public class LastNameValidator : AbstractValidator<string?> {
    private const int LAST_NAME_LENGTH_MIN = 2;
    private const int LAST_NAME_LENGTH_MAX = 30;

    public LastNameValidator() {
        RuleFor(lastName => lastName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Please provide a Last Name.")
            .Must(lastName => {
                var trimmed = lastName!.Trim();
                return trimmed.Length >= LAST_NAME_LENGTH_MIN && trimmed.Length <= LAST_NAME_LENGTH_MAX;
            })
            .WithMessage($"Please provide a Last Name between {LAST_NAME_LENGTH_MIN} and {LAST_NAME_LENGTH_MAX} characters.")
            .Must(lastName => lastName!.Trim() == lastName)
            .WithMessage("Please remove leading and trailing spaces from Last Name.")
            .IsValidPersonName()
            .WithMessage("Please provide a Last Name that contains only letters.");
    }
}

public class BirthDateValidator : AbstractValidator<DateTime?> {
    public BirthDateValidator() {
        RuleFor(birthDate => birthDate)
            .NotEmpty()
            .WithMessage("Please provide a Birth Date.")
            .LessThan(DateTime.Today)
            .WithMessage("Please provide a valid Birth Date in the past.");
    }
}

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
    private const int STREET_NAME_LENGTH_MIN = 2;
    private const int STREET_NAME_LENGTH_MAX = 30;

    public StreetNameValidator() {
        RuleFor(streetName => streetName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Please provide a Street Name.")
            .Must(streetName => {
                var trimmed = streetName!.Trim();
                return trimmed.Length >= STREET_NAME_LENGTH_MIN && trimmed.Length <= STREET_NAME_LENGTH_MAX;
            })
            .WithMessage($"Please provide a Street Name between {STREET_NAME_LENGTH_MIN} and {STREET_NAME_LENGTH_MAX} characters.")
            .Must(streetName => streetName!.Trim() == streetName)
            .WithMessage("Please remove leading and trailing spaces from Street Name.")
            .IsValidStreetName()
            .WithMessage("Please provide a Street Name that contains only valid characters.");
    }
}

public class CityNameValidator : AbstractValidator<string?> {
    private const int CITY_NAME_LENGTH_MIN = 2;
    private const int CITY_NAME_LENGTH_MAX = 30;

    public CityNameValidator() {
        RuleFor(cityName => cityName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Please provide a City Name.")
            .Must(cityName => {
                var trimmed = cityName!.Trim();
                return trimmed.Length >= CITY_NAME_LENGTH_MIN && trimmed.Length <= CITY_NAME_LENGTH_MAX;
            })
            .WithMessage($"Please provide a City Name between {CITY_NAME_LENGTH_MIN} and {CITY_NAME_LENGTH_MAX} characters.")
            .Must(cityName => cityName!.Trim() == cityName)
            .WithMessage("Please remove leading and trailing spaces from City Name.")
            .IsValidPersonName()
            .WithMessage("Please provide a City Name that contains only letters and spaces.");
    }
}

public class PostalCodeValidator : AbstractValidator<string?> {
    private const RegexOptions REGEX_OPTIONS = RegexOptions.IgnoreCase | RegexOptions.CultureInvariant;
    private const string REGEX_POSTAL_CODE = @"^[A-Z]\d[A-Z] ?\d[A-Z]\d$";

    public PostalCodeValidator() {
        RuleFor(postalCode => postalCode)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
                .WithMessage("Please provide a Postal Code.")
            .Must(postalCode => Regex.IsMatch(postalCode!.Trim().ToUpper(), REGEX_POSTAL_CODE, REGEX_OPTIONS))
                .WithMessage("Please provide a valid Postal Code.");
    }
}

}

namespace ContactManager.Core.Domain.Validators.Identity {

public class UsernameValidator : AbstractValidator<string?> {
    private const RegexOptions REGEX_OPTIONS = RegexOptions.IgnoreCase | RegexOptions.CultureInvariant;

    private const int LENGTH_MIN = 6;
    private const string REGEX_USERNAME = @"^([a-z0-9_.-])+$";

    public UsernameValidator() {
        RuleFor(userName => userName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
                .WithMessage("The UserName cannot be empty.")
            .Must(userName => userName!.Trim().Length >= LENGTH_MIN)
                .WithMessage($"The UserName must have at least {LENGTH_MIN} characters.")
            .Must(userName => Regex.IsMatch(userName!.Trim().ToLower(), REGEX_USERNAME, REGEX_OPTIONS))
                .WithMessage("The UserName contains invalid characters.");
    }
}

public class PasswordValidator : AbstractValidator<string?> {
    private const int LENGTH_MIN = 8;
    private const string REGEX_UPPERCASE = @"[A-Z]+";
    private const string REGEX_LOWERCASE = @"[a-z]+";
    private const string REGEX_DIGIT = @"[0-9]+";
    private const string REGEX_SPECIAL = @"[^a-zA-Z0-9]+";
    private const string REGEX_NOT_SPACE = @"^[^\s]+$";

    public PasswordValidator() {
        RuleFor(password => password)
            .NotEmpty()
                .WithMessage("The Password cannot be empty.")
            .MinimumLength(LENGTH_MIN)
                .WithMessage($"The Password must have at least {LENGTH_MIN} characters.")
            .Matches(REGEX_UPPERCASE)
                .WithMessage("The Password must have at least one uppercase letter.")
            .Matches(REGEX_LOWERCASE)
                .WithMessage("The Password must have at least one lowercase letter.")
            .Matches(REGEX_DIGIT)
                .WithMessage("The Password must have at least one digit.")
            .Matches(REGEX_SPECIAL)
                .WithMessage("The Password must have at least one special character.")
            .Matches(REGEX_NOT_SPACE)
                .WithMessage("The Password cannot contain spaces.");
    }
}

}