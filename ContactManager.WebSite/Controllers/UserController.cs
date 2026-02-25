using ContactManager.Core.Domain.Entities;
using ContactManager.WebSite.Utilities;
using ContactManager.WebSite.ViewModels.User;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContactManager.WebSite.Controllers;

[Authorize(Roles = AppRole.AdministratorName)]
public class UserController(
    UserManager<AppUser> userManager,
    RoleManager<AppRole> roleManager) : Controller {
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly RoleManager<AppRole> _roleManager = roleManager;

    [HttpGet]
    public async Task<IActionResult> Manage() {
        var users = await _userManager.Users
            .AsNoTracking()
            .OrderBy(user => user.UserName)
            .ToListAsync();

        var userItems = new List<UserItem>(users.Count);

        foreach (var user in users) {
            var roleName = (await _userManager.GetRolesAsync(user)).SingleOrDefault();

            userItems.Add(new UserItem {
                Id = user.Id,
                UserName = user.UserName!.Trim(),
                RoleName = roleName ?? "No role",
            });
        }

        return View(userItems);
    }

    [HttpGet]
    public IActionResult Create() {
        var generatedPassword = PasswordGenerator.Generate();

        return View(new UserCreate() {
            Password = generatedPassword,
            PasswordConfirmation = generatedPassword
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UserCreate viewModel) {
        if (!ModelState.IsValid) {
            return View(viewModel);
        }

        var role = await _roleManager.FindByIdAsync(viewModel.RoleId.ToString());
        var roleName = role?.Name;

        if (string.IsNullOrWhiteSpace(roleName)) {
            this.AddModelError("Selected role was not found.");
            return View(viewModel);
        }

        if (!AppRole.IsSupported(roleName)) {
            this.AddModelError($"Unsupported role {roleName}.");
            return View(viewModel);
        }

        var userToCreate = AppUser.Create(viewModel.UserName, roleName);
        var createResult = await _userManager.CreateAsync(userToCreate, viewModel.Password);

        if (!createResult.Succeeded) {
            this.AddModelErrors(createResult.Errors.Select(error => error.Description));
            return View(viewModel);
        }

        var addToRoleResult = await _userManager.AddToRoleAsync(userToCreate, roleName);

        if (!addToRoleResult.Succeeded) {
            this.AddModelErrors(
                addToRoleResult.Errors
                    .Select(error => error.Description)
                    .Append($"Unable to add the user to the role {roleName}."));
            return View(viewModel);
        }

        this.SetSuccessMessage($"User '{userToCreate.UserName}' created successfully.");
        return RedirectToAction(nameof(Manage));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(Guid id) {
        var userToReset = await _userManager.FindByIdAsync(id.ToString());
        if (userToReset is null) {
            this.SetErrorMessage("User was not found.");
            return RedirectToAction(nameof(Manage));
        }

        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(userToReset);

        var generatedPassword = PasswordGenerator.Generate();

        var resetResult = await _userManager.ResetPasswordAsync(userToReset, resetToken, generatedPassword);

        if (!resetResult.Succeeded) {
            this.SetErrorMessage(string.Join(" ", resetResult.Errors.Select(error => error.Description)));
            return RedirectToAction(nameof(Manage));
        }

        return View(new ResetPassword {
            UserName = userToReset.UserName!.Trim(),
            NewPassword = generatedPassword,
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(Guid id) {
        var currentUserId = this.GetRequiredUserId();

        if (currentUserId == id) {
            this.SetErrorMessage("You cannot remove your own account.");
            return RedirectToAction(nameof(Manage));
        }

        var userToRemove = await _userManager.FindByIdAsync(id.ToString());
        if (userToRemove is null) {
            this.SetErrorMessage("User was not found.");
            return RedirectToAction(nameof(Manage));
        }

        var roles = await _userManager.GetRolesAsync(userToRemove);
        var isAdministrator = roles.Any(roleName => roleName == AppRole.AdministratorName);

        if (isAdministrator) {
            var administrators = await _userManager.GetUsersInRoleAsync(AppRole.AdministratorName);
            var administratorCount = administrators.Count;

            if (administratorCount <= 1) {
                this.SetErrorMessage("You cannot remove the last administrator.");
                return RedirectToAction(nameof(Manage));
            }
        }

        var deleteResult = await _userManager.DeleteAsync(userToRemove);

        if (!deleteResult.Succeeded) {
            this.SetErrorMessage(string.Join(" ", deleteResult.Errors.Select(error => error.Description)));
            return RedirectToAction(nameof(Manage));
        }

        this.SetSuccessMessage($"User '{userToRemove.UserName}' removed successfully.");
        return RedirectToAction(nameof(Manage));
    }
}
