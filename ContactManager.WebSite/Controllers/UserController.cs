using ContactManager.Core;
using ContactManager.Core.Domain.Entities;
using ContactManager.Core.Domain.Enums;
using ContactManager.WebSite.Utilities;
using ContactManager.WebSite.ViewModels.User;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContactManager.WebSite.Controllers;

[Authorize(Roles = Roles.Administrator)]
public class UserController(
    ContactManagerContext context,
    UserManager<AppUser> userManager,
    RoleManager<IdentityRole<Guid>> roleManager) : Controller {
    private readonly ContactManagerContext _context = context;
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager = roleManager;

    [HttpGet]
    public async Task<IActionResult> Manage() {
        var users = await _userManager.Users
            .AsNoTracking()
            .ToListAsync();

        var userIds = users.Select(user => user.Id).ToList();

        var userRoles = await _context.Set<IdentityUserRole<Guid>>()
            .AsNoTracking()
            .Where(userRole => userIds.Contains(userRole.UserId))
            .ToListAsync();

        var roleIds = userRoles
            .Select(userRole => userRole.RoleId)
            .Distinct()
            .ToList();

        var roleNamesById = await _roleManager.Roles
            .AsNoTracking()
            .Where(role => roleIds.Contains(role.Id))
            .ToDictionaryAsync(role => role.Id, role => role.Name ?? string.Empty);

        var roleNameByUserId = userRoles
            .GroupBy(userRole => userRole.UserId)
            .ToDictionary(
                group => group.Key,
                group => roleNamesById.TryGetValue(group.First().RoleId, out var roleName)
                    ? roleName
                    : string.Empty);

        var userItems = users
            .Select(user => new UserItem {
                Id = user.Id,
                UserName = user.UserName!.Trim(),
                RoleName = roleNameByUserId.TryGetValue(user.Id, out var roleName)
                    ? roleName
                    : string.Empty,
            })
            .ToList();

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

        var roleToAssign = await _roleManager.FindByIdAsync(viewModel.RoleId.ToString());
        if (roleToAssign is null) {
            ModelState.AddModelError(string.Empty, "Selected role was not found.");
            return View(viewModel);
        }

        var userToCreate = AppUser.Create(viewModel.UserName);
        var createResult = await _userManager.CreateAsync(userToCreate, viewModel.Password);

        if (!createResult.Succeeded) {
            foreach (var error in createResult.Errors) {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(viewModel);
        }

        var addToRoleResult = await _userManager.AddToRoleAsync(userToCreate, roleToAssign.Name);

        if (!addToRoleResult.Succeeded) {
            ModelState.AddModelError(string.Empty, $"Unable to add the user to the role {roleToAssign.Name}.");
            return View(viewModel);
        }

        return RedirectToAction(nameof(Manage));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(Guid id) {
        var userToReset = await _userManager.FindByIdAsync(id.ToString());
        if (userToReset is null) {
            TempData["Error"] = "User was not found.";
            return RedirectToAction(nameof(Manage));
        }

        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(userToReset);

        var generatedPassword = PasswordGenerator.Generate();

        var resetResult = await _userManager.ResetPasswordAsync(userToReset, resetToken, generatedPassword);

        if (!resetResult.Succeeded) {
            TempData["Error"] = string.Join(" ", resetResult.Errors.Select(error => error.Description));
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
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser is null) {
            return Challenge();
        }

        if (currentUser.Id == id) {
            TempData["Error"] = "You cannot remove your own account.";
            return RedirectToAction(nameof(Manage));
        }

        var userToRemove = await _userManager.FindByIdAsync(id.ToString());
        if (userToRemove is null) {
            TempData["Error"] = "User was not found.";
            return RedirectToAction(nameof(Manage));
        }

        var roles = await _userManager.GetRolesAsync(userToRemove);
        if (roles.Contains(Roles.Administrator)) {
            var administratorRoleId = await _roleManager.Roles
                .AsNoTracking()
                .Where(role => role.Name == Roles.Administrator)
                .Select(role => role.Id)
                .SingleOrDefaultAsync();

            if (administratorRoleId == Guid.Empty) {
                TempData["Error"] = "Administrator role was not found.";
                return RedirectToAction(nameof(Manage));
            }

            var administratorCount = await _context.Set<IdentityUserRole<Guid>>()
                .AsNoTracking()
                .CountAsync(userRole => userRole.RoleId == administratorRoleId);

            if (administratorCount <= 1) {
                TempData["Error"] = "You cannot remove the last administrator.";
                return RedirectToAction(nameof(Manage));
            }
        }

        var deleteResult = await _userManager.DeleteAsync(userToRemove);

        if (!deleteResult.Succeeded) {
            TempData["Error"] = string.Join(" ", deleteResult.Errors.Select(error => error.Description));
            return RedirectToAction(nameof(Manage));
        }

        return RedirectToAction(nameof(Manage));
    }
}
