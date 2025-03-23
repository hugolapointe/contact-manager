using ContactManager.Core.Domain.Entities;
using ContactManager.WebSite.Utilities;
using ContactManager.WebSite.ViewModels.User;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ContactManager.WebSite.Controllers;

[Authorize(Roles = "Administrator")]
public class UserController(
    UserManager<User> userManager,
    RoleManager<IdentityRole<Guid>> roleManager,
    DomainAsserts asserts) : Controller {
    private readonly UserManager<User> userManager = userManager;
    private readonly RoleManager<IdentityRole<Guid>> roleManager = roleManager;
    private readonly DomainAsserts asserts = asserts;

    [HttpGet]
    public async Task<IActionResult> Manage() {
        var vm = new List<UserItem>();

        foreach (var user in userManager.Users) {
            var userRoles = await userManager.GetRolesAsync(user);
            vm.Add(new UserItem {
                Id = user.Id,
                UserName = user.UserName!.Trim(),
                RoleName = userRoles.SingleOrDefault(string.Empty)
            });
        }

        return View(vm);
    }

    [HttpGet]
    public IActionResult Create() {
        var passwordGenerated = PasswordGenerator.Generate();

        return View(new UserCreate() {
            Password = passwordGenerated,
            PasswordConfirmation = passwordGenerated
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UserCreate vm) {
        if (!ModelState.IsValid) {
            return View(vm);
        }

        var role = await roleManager.FindByIdAsync(vm.RoleId.ToString());

        asserts.Exists(role, "Role not found.");

        var toCreate = new User(vm.UserName);
        var result = await userManager.CreateAsync(toCreate, vm.Password);

        if (!result.Succeeded) {
            foreach (var error in result.Errors) {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(vm);
        }

        result = await userManager.AddToRoleAsync(toCreate, role.Name);

        if (!result.Succeeded) {
            ModelState.AddModelError(string.Empty, $"Unable to add the user to the role {role.Name}.");
            return View(vm);
        }

        return RedirectToAction(nameof(Manage));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(Guid id) {
        var user = await userManager.FindByIdAsync(id.ToString());

        asserts.Exists(user, "User not found.");

        var token = await userManager.GeneratePasswordResetTokenAsync(user);

        var newPassword = PasswordGenerator.Generate();

        var result = await userManager.ResetPasswordAsync(user, token, newPassword);

        if (!result.Succeeded) {
            throw new Exception("Unable to reset password.");
        }

        return View(new ResetPassword {
            UserName = user.UserName!.Trim(),
            NewPassword = newPassword,
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(Guid id) {
        var user = await userManager.FindByIdAsync(id.ToString());

        asserts.Exists(user, "User not found.");

        var result = await userManager.DeleteAsync(user!);

        if (!result.Succeeded) {
            throw new Exception("Unable to remove the user.");
        }

        return RedirectToAction(nameof(Manage));
    }
}
