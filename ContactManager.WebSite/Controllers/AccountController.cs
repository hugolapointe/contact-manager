using ContactManager.Core.Domain.Entities;
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
                ModelState.AddModelError(string.Empty, "You are not allowed to log in.");
            } else if (signInResult.IsLockedOut) {
                ModelState.AddModelError(string.Empty, "Your account is locked out.");
            } else {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
            }
            return View(viewModel);
        }

        if (string.IsNullOrEmpty(viewModel.ReturnUrl) || !Url.IsLocalUrl(viewModel.ReturnUrl)) {
            return RedirectToAction("Index", "Home");
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

        var userToCreate = AppUser.CreateForUserName(viewModel.UserName);
        var createResult = await _userManager.CreateAsync(userToCreate, viewModel.Password);

        if (!createResult.Succeeded) {
            foreach (var error in createResult.Errors) {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(viewModel);
        }

        await _signInManager.SignInAsync(userToCreate, true);

        return RedirectToAction("Manage", "Contact");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LogOut() {
        await _signInManager.SignOutAsync();

        return RedirectToAction("Index", "Home");
    }
}
