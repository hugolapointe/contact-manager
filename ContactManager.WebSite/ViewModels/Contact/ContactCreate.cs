using ContactManager.Core.Domain.Validators;

using FluentValidation;

using System.ComponentModel.DataAnnotations;

namespace ContactManager.WebSite.ViewModels.Contact;

public class ContactCreate {
    [Display(Name = "First Name")]
    public string? FirstName { get; set; }

    [Display(Name = "Last Name")]
    public string? LastName { get; set; }

    [Display(Name = "Date Of Birth")]
    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }

    [Display(Name = "Street Number")]
    public int? Address_StreetNumber { get; set; }

    [Display(Name = "Street Name")]
    public string? Address_StreetName { get; set; }

    [Display(Name = "City")]
    public string? Address_CityName { get; set; }

    [Display(Name = "Postal Code")]
    public string? Address_PostalCode { get; set; }

    [Display(Name = "Terms Accepted?")]
    public bool TermsAccepted { get; set; } = false;

    public class Validator : AbstractValidator<ContactCreate> {
        public Validator() {
            RuleFor(vm => vm.FirstName)
                .NotNull()
                    .WithMessage("Please provide a first name.")
                .SetValidator(new FirstNameValidator());

            RuleFor(vm => vm.LastName)
                .NotNull()
                    .WithMessage("Please provide a last name.")
                .SetValidator(new LastNameValidator());

            RuleFor(vm => vm.DateOfBirth)
                .NotNull()
                    .WithMessage("Please provide a date of birth.")
                .SetValidator(new BirthDateValidator());

            RuleFor(vm => vm.Address_StreetNumber)
                .NotNull()
                    .WithMessage("Please provide a street number.")
                .SetValidator(new StreetNumberValidator());

            RuleFor(vm => vm.Address_StreetName)
                .NotNull()
                    .WithMessage("Please provide a street name.")
                .SetValidator(new StreetNameValidator());

            RuleFor(vm => vm.Address_CityName)
                .NotNull()
                    .WithMessage("Please provide a city name.")
                .SetValidator(new CityNameValidator());

            RuleFor(vm => vm.Address_PostalCode)
                .NotNull()
                    .WithMessage("Please provide a postal code.")
                .SetValidator(new PostalCodeValidator());

            RuleFor(vm => vm.TermsAccepted)
                .Must(terms => terms == true)
                    .WithMessage("Make sure your contact accept the terms.");
        }
    }
}
