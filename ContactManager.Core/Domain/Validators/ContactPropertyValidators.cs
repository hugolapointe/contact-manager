using FluentValidation;

namespace ContactManager.Core.Domain.Validators;

public class FirstNameValidator : AbstractValidator<string?> {
    private const int FIRST_NAME_LENGTH_MIN = 2;
    private const int FIRST_NAME_LENGTH_MAX = 30;

    public FirstNameValidator() {
        Transform(firstName => firstName, firstName => firstName!.Trim())
            .NotEmpty()
            .WithMessage("Please provide a First Name.")
            .Length(FIRST_NAME_LENGTH_MIN, FIRST_NAME_LENGTH_MAX)
            .WithMessage($"Please provide a First Name between {FIRST_NAME_LENGTH_MIN} and {FIRST_NAME_LENGTH_MAX} characters.")
            .IsValidName()
            .WithMessage("Please provide a First Name that contains only letters.");
    }
}

public class LastNameValidator : AbstractValidator<string?> {
    private const int LAST_NAME_LENGTH_MIN = 2;
    private const int LAST_NAME_LENGTH_MAX = 30;

    public LastNameValidator() {
        Transform(lastName => lastName, lastName => lastName!.Trim())
            .NotEmpty()
            .WithMessage("Please provide a Last Name.")
            .Length(LAST_NAME_LENGTH_MIN, LAST_NAME_LENGTH_MAX)
            .WithMessage($"Please provide a Last Name between {LAST_NAME_LENGTH_MIN} and {LAST_NAME_LENGTH_MAX} characters.")
            .IsValidName()
            .WithMessage("Please provide a Last Name that contains only letters.");
    }
}

public class BirthDateValidator : AbstractValidator<DateTime?> {
    public BirthDateValidator() {
        RuleFor(birthDate => birthDate)
            .NotEmpty()
            .WithMessage("Please provide a Birth Date.")
            .LessThan(DateTime.Now)
            .WithMessage("Please provide a valid Birth Date in the past.");
    }
}
