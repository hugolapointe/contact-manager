using ContactManager.Core.Domain.Validators.Identity;

using FluentValidation;

using System.ComponentModel.DataAnnotations;

namespace ContactManager.WebSite.ViewModels.Account;

public class LogIn {
    [Display(Name = "Username")]
    public string? UserName { get; set; }

    [Display(Name = "Password")]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    [Display(Name = "Remember Me?")]
    public bool RememberMe { get; set; } = false;

    public class Validator : AbstractValidator<LogIn> {
        public Validator() {
            RuleFor(x => x.UserName)
                .SetValidator(new UsernameValidator());

            RuleFor(vm => vm.Password)
                .SetValidator(new PasswordValidator());
        }
    }
}
