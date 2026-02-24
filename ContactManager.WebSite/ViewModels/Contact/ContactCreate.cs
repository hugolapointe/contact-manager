using ContactManager.Core.Domain.Validators;

using FluentValidation;

using System.ComponentModel.DataAnnotations;

namespace ContactManager.WebSite.ViewModels.Contact;

public class ContactCreate {
    [Display(Name = "First Name")]
    public string? FirstName { get; set; }

    [Display(Name = "Last Name")]
    public string? LastName { get; set; }

    [Display(Name = "Date of Birth")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime? DateOfBirth { get; set; }

    [Display(Name = "Street Number")]
    public int? Address_StreetNumber { get; set; }

    [Display(Name = "Street Name")]
    public string? Address_StreetName { get; set; }

    [Display(Name = "City")]
    public string? Address_CityName { get; set; }

    [Display(Name = "Postal Code")]
    public string? Address_PostalCode { get; set; }

    [Display(Name = "Accept terms and conditions")]
    public bool TermsAccepted { get; set; } = false;

    public class Validator : AbstractValidator<ContactCreate> {
        public Validator() {
            RuleFor(vm => vm.FirstName)
                .SetValidator(new FirstNameValidator());

            RuleFor(vm => vm.LastName)
                .SetValidator(new LastNameValidator());

            RuleFor(vm => vm.DateOfBirth)
                .SetValidator(new BirthDateValidator());

            RuleFor(vm => vm.Address_StreetNumber)
                .SetValidator(new StreetNumberValidator());

            RuleFor(vm => vm.Address_StreetName)
                .SetValidator(new StreetNameValidator());

            RuleFor(vm => vm.Address_CityName)
                .SetValidator(new CityNameValidator());

            RuleFor(vm => vm.Address_PostalCode)
                .SetValidator(new PostalCodeValidator());

            RuleFor(vm => vm.TermsAccepted)
                .Must(terms => terms == true)
                    .WithMessage("Please accept the terms and conditions.");
        }
    }
}
