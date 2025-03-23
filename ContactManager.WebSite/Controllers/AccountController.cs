using ContactManager.Core.Domain.Entities;
using ContactManager.WebSite.ViewModels.Account;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ContactManager.WebSite.Controllers;

[Authorize]
public class AccountController(
    UserManager<User> userManager,
    SignInManager<User> signInManager) : Controller {
    private readonly UserManager<User> userManager = userManager;
    private readonly SignInManager<User> signInManager = signInManager;

    [HttpGet]
    [AllowAnonymous]
    public IActionResult LogIn(string? returnUrl = null) {

        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LogIn(LogIn vm, string? returnUrl = null) {
        if (!ModelState.IsValid) {
            ViewBag.ReturnUrl = returnUrl;
            return View(vm);
        }

        var result = await signInManager.PasswordSignInAsync(
            vm.UserName, vm.Password, isPersistent: vm.RememberMe, lockoutOnFailure: false);

        if (!result.Succeeded) {
            if (result.IsNotAllowed) {
                ModelState.AddModelError(string.Empty, "You are not allowed to log in.");
            } else if (result.IsLockedOut) {
                ModelState.AddModelError(string.Empty, "Your account is locked out.");
            } else {
                ModelState.AddModelError(string.Empty, "Invalid login failed.");
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(vm);
        }

        if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl)) {
            return RedirectToAction("Index", "Home");
        }

        return LocalRedirect(returnUrl);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register() {
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Register(Register vm) {
        if (!ModelState.IsValid) {
            return View(vm);
        }

        try {
            var newUser = new User(vm.UserName);
            var result = await userManager.CreateAsync(newUser, vm.Password);

            if (!result.Succeeded) {
                ModelState.AddModelError(string.Empty, "Unable to register a new user.");
                return View(vm);
            }

            await signInManager.SignInAsync(newUser, true);

        } catch {
            ModelState.AddModelError(string.Empty, "Something went wrong. Please try again.");
            return View(vm);
        }

        return RedirectToAction("Manage", "Contact");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LogOut() {
        await signInManager.SignOutAsync();

        return RedirectToAction("Index", "Home");
    }
}
