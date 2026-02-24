using Microsoft.AspNetCore.Mvc;

using System.Security.Claims;

namespace ContactManager.WebSite.Utilities;

public static class ControllerBaseExtensions {
    public static Guid GetUserId(this ControllerBase controller) {
        var userIdRaw = controller.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (Guid.TryParse(userIdRaw, out var userId)) {
            return userId;
        }

        throw new InvalidOperationException("Authenticated user does not contain a valid user identifier claim.");
    }
}
