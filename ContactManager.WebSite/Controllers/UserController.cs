using ContactManager.Core;
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
    RoleManager<AppRole> roleManager,
    ContactManagerContext context) : Controller {

    [HttpGet]
    public async Task<IActionResult> Index() {
        // Chargement en 3 requêtes au lieu d'une requête par utilisateur (N+1).
        var users = await userManager.Users
            .AsNoTracking()
            .OrderBy(user => user.UserName)
            .ToListAsync();

        var userRoles = await context.Set<IdentityUserRole<Guid>>().ToListAsync();
        var roles = await roleManager.Roles.ToListAsync();

        var userItems = users.Select(user => {
            var roleId = userRoles.FirstOrDefault(ur => ur.UserId == user.Id)?.RoleId;
            var roleName = roles.FirstOrDefault(r => r.Id == roleId)?.Name;

            return new UserItem {
                Id = user.Id,
                UserName = user.UserName!,
                RoleName = roleName ?? "No role",
            };
        }).ToList();

        return View(new UserList { Users = userItems });
    }

    [HttpGet]
    public IActionResult Create() {
        var generatedPassword = PasswordGenerator.Generate();

        return View(new UserCreate {
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

        var role = await roleManager.FindByIdAsync(viewModel.RoleId.ToString());
        var roleName = role?.Name;

        if (string.IsNullOrWhiteSpace(roleName)) {
            this.AddModelError("Selected role was not found.");
            return View(viewModel);
        }

        if (!AppRole.IsSupported(roleName)) {
            this.AddModelError($"Unsupported role '{roleName}'.");
            return View(viewModel);
        }

        var newUser = AppUser.Create(viewModel.UserName!);
        var createResult = await userManager.CreateAsync(newUser, viewModel.Password!);

        if (!createResult.Succeeded) {
            this.AddModelErrors(createResult.Errors.Select(error => error.Description));
            return View(viewModel);
        }

        var roleAssignResult = await userManager.AddToRoleAsync(newUser, roleName);
        if (!roleAssignResult.Succeeded) {
            this.AddModelErrors(roleAssignResult.Errors.Select(error => error.Description));
            // Rollback : supprime l'utilisateur créé si l'assignation du rôle échoue.
            await userManager.DeleteAsync(newUser);
            return View(viewModel);
        }

        this.AddNotification($"User '{newUser.UserName}' created successfully.", NotificationType.Success);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(Guid id) {
        var userToReset = await userManager.FindByIdAsync(id.ToString());
        if (userToReset is null) {
            this.AddNotification("User was not found.", NotificationType.Error);
            return RedirectToAction(nameof(Index));
        }

        var resetToken = await userManager.GeneratePasswordResetTokenAsync(userToReset);
        var generatedPassword = PasswordGenerator.Generate();
        var resetResult = await userManager.ResetPasswordAsync(userToReset, resetToken, generatedPassword);

        if (!resetResult.Succeeded) {
            this.AddNotification(string.Join(" ", resetResult.Errors.Select(error => error.Description)), NotificationType.Error);
            return RedirectToAction(nameof(Index));
        }

        return View(new ResetPassword {
            UserName = userToReset.UserName!,
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

        var userToRemove = await userManager.FindByIdAsync(id.ToString());
        if (userToRemove is null) {
            this.AddNotification("User was not found.", NotificationType.Error);
            return RedirectToAction(nameof(Index));
        }

        var roles = await userManager.GetRolesAsync(userToRemove);
        var isAdministrator = roles.Any(roleName => roleName == AppRole.AdministratorName);

        if (isAdministrator) {
            var administrators = await userManager.GetUsersInRoleAsync(AppRole.AdministratorName);

            if (administrators.Count <= 1) {
                this.AddNotification("You cannot remove the last administrator.", NotificationType.Error);
                return RedirectToAction(nameof(Index));
            }
        }

        var deleteResult = await userManager.DeleteAsync(userToRemove);

        if (!deleteResult.Succeeded) {
            this.AddNotification(string.Join(" ", deleteResult.Errors.Select(error => error.Description)), NotificationType.Error);
            return RedirectToAction(nameof(Index));
        }

        this.AddNotification($"User '{userToRemove.UserName}' removed successfully.", NotificationType.Success);
        return RedirectToAction(nameof(Index));
    }
}
