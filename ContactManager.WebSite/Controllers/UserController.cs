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
    public async Task<IActionResult> Index() {
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

        this.AddNotification($"User '{userToCreate.UserName}' created successfully.", NotificationType.Success);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(Guid id) {
        var userToReset = await _userManager.FindByIdAsync(id.ToString());
        if (userToReset is null) {
            this.AddNotification("User was not found.", NotificationType.Error);
            return RedirectToAction(nameof(Index));
        }

        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(userToReset);

        var generatedPassword = PasswordGenerator.Generate();

        var resetResult = await _userManager.ResetPasswordAsync(userToReset, resetToken, generatedPassword);

        if (!resetResult.Succeeded) {
            this.AddNotification(string.Join(" ", resetResult.Errors.Select(error => error.Description)), NotificationType.Error);
            return RedirectToAction(nameof(Index));
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
            this.AddNotification("You cannot remove your own account.", NotificationType.Error);
            return RedirectToAction(nameof(Index));
        }

        var userToRemove = await _userManager.FindByIdAsync(id.ToString());
        if (userToRemove is null) {
            this.AddNotification("User was not found.", NotificationType.Error);
            return RedirectToAction(nameof(Index));
        }

        var roles = await _userManager.GetRolesAsync(userToRemove);
        var isAdministrator = roles.Any(roleName => roleName == AppRole.AdministratorName);

        if (isAdministrator) {
            var administrators = await _userManager.GetUsersInRoleAsync(AppRole.AdministratorName);
            var administratorCount = administrators.Count;

            if (administratorCount <= 1) {
                this.AddNotification("You cannot remove the last administrator.", NotificationType.Error);
                return RedirectToAction(nameof(Index));
            }
        }

        var deleteResult = await _userManager.DeleteAsync(userToRemove);

        if (!deleteResult.Succeeded) {
            this.AddNotification(string.Join(" ", deleteResult.Errors.Select(error => error.Description)), NotificationType.Error);
            return RedirectToAction(nameof(Index));
        }

        this.AddNotification($"User '{userToRemove.UserName}' removed successfully.", NotificationType.Success);
        return RedirectToAction(nameof(Index));
    }
}
