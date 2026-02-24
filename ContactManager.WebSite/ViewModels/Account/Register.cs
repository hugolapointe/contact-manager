using ContactManager.Core.Domain.Validators.Identity;

using FluentValidation;

using System.ComponentModel.DataAnnotations;

namespace ContactManager.WebSite.ViewModels.Account;

public class Register {
    [Display(Name = "Username")]
    public string? UserName { get; set; }

    [Display(Name = "Password")]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    [Display(Name = "Confirm Password")]
    [DataType(DataType.Password)]
    public string? PasswordConfirmation { get; set; }

    [Display(Name = "Accept terms and conditions")]
    public bool TermsAccepted { get; set; } = false;

    public class Validator : AbstractValidator<Register> {
        public Validator() {
            RuleFor(x => x.UserName)
                .SetValidator(new UsernameValidator());

            RuleFor(vm => vm.Password)
                .SetValidator(new PasswordValidator());

            RuleFor(vm => vm.PasswordConfirmation)
                .NotEmpty()
                    .WithMessage("Please confirm your password.")
                .Equal(vm => vm.Password)
                    .WithMessage("The password and confirmation password do not match.");

            RuleFor(vm => vm.TermsAccepted)
                .Must(terms => terms == true)
                    .WithMessage("You must accept the Terms.");
        }
    }
}
