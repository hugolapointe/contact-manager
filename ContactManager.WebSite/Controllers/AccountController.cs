using ContactManager.Core.Domain.Entities;
using ContactManager.WebSite.Controllers;
using ContactManager.WebSite.ViewModels.Account;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ContactManager.WebSite.Controllers;

[Authorize]
public class AccountController(
    UserManager<AppUser> userManager,
    SignInManager<AppUser> signInManager) : Controller {

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
    public async Task<IActionResult> Login(Login viewModel) {
        // ReturnUrl provient du champ caché du formulaire (rempli lors du GET).
        if (!ModelState.IsValid) {
            return View(viewModel);
        }

        var signInResult = await signInManager.PasswordSignInAsync(
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

        var newUser = AppUser.Create(viewModel.UserName!);
        var createResult = await userManager.CreateAsync(newUser, viewModel.Password!);

        if (!createResult.Succeeded) {
            this.AddModelErrors(createResult.Errors.Select(error => error.Description));
            return View(viewModel);
        }

        // Assign default role "User"
        var roleAssignResult = await userManager.AddToRoleAsync(newUser, AppRole.UserName);
        if (!roleAssignResult.Succeeded) {
            this.AddModelErrors(roleAssignResult.Errors.Select(error => error.Description));
            await userManager.DeleteAsync(newUser);
            return View(viewModel);
        }

        await signInManager.SignInAsync(newUser, true);

        return RedirectToAction(nameof(ContactController.Index), "Contact");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LogOut() {
        await signInManager.SignOutAsync();
        this.AddNotification("Logout successful.", NotificationType.Success);
        return RedirectToAction(nameof(HomeController.Index), "Home");
    }
}
