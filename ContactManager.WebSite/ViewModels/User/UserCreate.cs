using ContactManager.Core.Domain.Validators.Identity;

using FluentValidation;

using System.ComponentModel.DataAnnotations;

namespace ContactManager.WebSite.ViewModels.User;

public class UserCreate {
    [Display(Name = "UserName")]
    public string? UserName { get; set; }

    [Display(Name = "Role")]
    public Guid RoleId { get; set; }

    [Display(Name = "Password")]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    [Display(Name = "Password Confirmation")]
    [DataType(DataType.Password)]
    public string? PasswordConfirmation { get; set; }

    public class Validator : AbstractValidator<UserCreate> {
        public Validator() {
            RuleFor(vm => vm.UserName)
                .SetValidator(new UsernameValidator());

            RuleFor(vm => vm.RoleId)
                .NotEmpty()
                    .WithMessage("Please provide a RoleId.");

            RuleFor(vm => vm.Password)
                .SetValidator(new PasswordValidator());

            RuleFor(vm => vm.PasswordConfirmation)
                .Equal(vm => vm.Password)
                    .WithMessage("The password and confirmation password do not match.");
        }
    }
}
