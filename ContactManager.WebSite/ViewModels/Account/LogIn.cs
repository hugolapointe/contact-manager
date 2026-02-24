using FluentValidation;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace ContactManager.WebSite.ViewModels.Account;

public class Login {
    [Display(Name = "Username")]
    public string? UserName { get; set; }

    [Display(Name = "Password")]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    [Display(Name = "Remember Me?")]
    public bool RememberMe { get; set; } = false;

    [HiddenInput(DisplayValue = false)]
    [Editable(false)]
    public string? ReturnUrl { get; set; }

    public class Validator : AbstractValidator<Login> {
        public Validator() {
            RuleFor(x => x.UserName)
                .NotEmpty()
                .WithMessage("Please provide your username.");

            RuleFor(vm => vm.Password)
                .NotEmpty()
                .WithMessage("Please provide your password.");
        }
    }
}
