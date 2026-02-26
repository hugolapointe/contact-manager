using ContactManager.Core.Domain.Entities;
using ContactManager.WebSite.Utilities;
using ContactManager.WebSite.ViewModels.Account;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ContactManager.WebSite.Controllers;

[Authorize]
public class AccountController(
    UserManager<AppUser> userManager,
    SignInManager<AppUser> signInManager) : Controller {
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly SignInManager<AppUser> _signInManager = signInManager;

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null) {
        return View(new Login {
            ReturnUrl = returnUrl
        });
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(Login viewModel, string? returnUrl = null) {
        viewModel.ReturnUrl = returnUrl ?? viewModel.ReturnUrl;

        if (!ModelState.IsValid) {
            return View(viewModel);
        }

        var signInResult = await _signInManager.PasswordSignInAsync(
            viewModel.UserName, viewModel.Password, isPersistent: viewModel.RememberMe, lockoutOnFailure: false);

        if (!signInResult.Succeeded) {
            if (signInResult.IsNotAllowed) {
                this.AddModelError("You are not allowed to log in.");
            } else if (signInResult.IsLockedOut) {
                this.AddModelError("Your account is locked out.");
            } else {
                this.AddModelError("Invalid username or password.");
            }
            return View(viewModel);
        }

        if (string.IsNullOrEmpty(viewModel.ReturnUrl) || !Url.IsLocalUrl(viewModel.ReturnUrl)) {
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        return LocalRedirect(viewModel.ReturnUrl);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register() {
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(Register viewModel) {
        if (!ModelState.IsValid) {
            return View(viewModel);
        }

        var userToCreate = AppUser.Create(viewModel.UserName, AppRole.UserName);
        var createResult = await _userManager.CreateAsync(userToCreate, viewModel.Password);

        if (!createResult.Succeeded) {
            this.AddModelErrors(createResult.Errors.Select(error => error.Description));
            return View(viewModel);
        }

        await _signInManager.SignInAsync(userToCreate, true);

        return RedirectToAction(nameof(ContactController.Index), "Contact");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LogOut() {
        await _signInManager.SignOutAsync();
        this.AddNotification("Logout successful.", NotificationType.Success);
        return RedirectToAction(nameof(HomeController.Index), "Home");
    }
}
