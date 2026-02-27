using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ContactManager.WebSite.Controllers;

public enum NotificationType { Success, Error, Info, Warning }

public static class ControllerBaseExtensions {
    public static Guid GetRequiredUserId(this ControllerBase controller) {
        var userIdRaw = controller.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdRaw, out var userId)) {
            throw new InvalidOperationException("Authenticated user does not contain a valid user identifier claim.");
        }
        return userId;
    }
    public static void AddModelErrors(this Controller controller, IEnumerable<string> errors) {
        foreach (var error in errors.Where(error => !string.IsNullOrWhiteSpace(error))) {
            controller.ModelState.AddModelError(string.Empty, error);
        }
    }
    public static void AddModelError(this Controller controller, string error) {
        if (!string.IsNullOrWhiteSpace(error)) {
            controller.ModelState.AddModelError(string.Empty, error);
        }
    }
    public static void AddNotification(this Controller controller, string message, NotificationType type) {
        controller.TempData[$"Notification.{type}"] = message;
    }
}
