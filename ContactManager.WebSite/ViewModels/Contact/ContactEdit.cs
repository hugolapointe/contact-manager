using ContactManager.Core.Domain.Validators;

using FluentValidation;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace ContactManager.WebSite.ViewModels.Contact;

public class ContactEdit {
    [HiddenInput(DisplayValue = false)]
    [Editable(false)]
    public Guid ContactId { get; set; }

    [Display(Name = "First Name")]
    public string? FirstName { get; set; }

    [Display(Name = "Last Name")]
    public string? LastName { get; set; }

    [Display(Name = "Date of Birth")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime? DateOfBirth { get; set; }

    public class Validator : AbstractValidator<ContactEdit> {
        public Validator() {
            RuleFor(vm => vm.FirstName)
                .SetValidator(new FirstNameValidator());

            RuleFor(vm => vm.LastName)
                .SetValidator(new LastNameValidator());

            RuleFor(vm => vm.DateOfBirth)
                .SetValidator(new BirthDateValidator());
        }
    }
}
