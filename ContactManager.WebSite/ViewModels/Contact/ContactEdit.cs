using ContactManager.Core.Domain.Validators;

using FluentValidation;

using System.ComponentModel.DataAnnotations;

namespace ContactManager.WebSite.ViewModels.Contact;

public class ContactEdit {
    [Display(Name = "First Name")]
    public string? FirstName { get; set; }

    [Display(Name = "Last Name")]
    public string? LastName { get; set; }

    [Display(Name = "Date Of Birth")]
    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }

    public class Validator : AbstractValidator<ContactEdit> {
        public Validator() {
            RuleFor(vm => vm.FirstName)
                .NotNull()
                    .WithMessage("Please provide a first name.")
                .SetValidator(new FirstNameValidator());

            RuleFor(vm => vm.LastName)
                .NotNull()
                    .WithMessage("Please provde a last name.")
                .SetValidator(new LastNameValidator());

            RuleFor(vm => vm.DateOfBirth)
                .NotNull()
                    .WithMessage("Please provide a Date Of Birth.")
                .SetValidator(new BirthDateValidator());
        }
    }
}
