using ContactManager.Core.Domain.Entities;

using Microsoft.AspNetCore.Identity;

using System.Security.Claims;

namespace ContactManager.WebSite.Utilities;

public class DomainAsserts(UserManager<User> userManager) {
    private readonly UserManager<User> userManager = userManager;

    public void Exists(object entity, string errorMessage = "The resource cannot be found.") {
        if (entity is null) {
            throw new ArgumentNullException(errorMessage);
        }
    }

    public void IsOwnedByCurrentUser(object entity, ClaimsPrincipal user, string errorMessage = "You must own the resource.") {
        var userId = userManager.GetUserId(user);

        var ownerIdProp = entity.GetType().GetProperty("OwnerId");

        if (ownerIdProp is null) {
            throw new UnauthorizedAccessException(errorMessage);
        }

        var ownerIdValue = ownerIdProp.GetValue(entity);

        if (!Equals(ownerIdValue?.ToString(), userId)) {
            throw new UnauthorizedAccessException(errorMessage);
        }
    }
}