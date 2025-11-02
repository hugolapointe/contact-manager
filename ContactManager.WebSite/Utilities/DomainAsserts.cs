using ContactManager.Core.Domain.Entities;

using Microsoft.AspNetCore.Identity;

using System.Security.Claims;

namespace ContactManager.WebSite.Utilities;

/// <summary>
/// Provides assertion methods for domain operations, including entity existence and ownership validation.
/// </summary>
public class DomainAsserts(UserManager<User> userManager) {
    private readonly UserManager<User> userManager = userManager;

    /// <summary>
    /// Verifies that an entity exists (is not null).
    /// </summary>
    /// <param name="entity">The entity to check.</param>
    /// <param name="errorMessage">The error message to include in the exception if the entity is null.</param>
    /// <exception cref="ArgumentNullException">Thrown when the entity is null.</exception>
    public void Exists(object entity, string errorMessage = "The resource cannot be found.") {
        if (entity is null) {
            throw new ArgumentNullException(errorMessage);
        }
    }

    /// <summary>
    /// Verifies that the specified entity is owned by the current user.
    /// </summary>
    /// <param name="entity">The entity to check ownership for. Must have an OwnerId property.</param>
    /// <param name="user">The current user's claims principal.</param>
    /// <param name="errorMessage">The error message to include in the exception if the user doesn't own the entity.</param>
    /// <exception cref="UnauthorizedAccessException">Thrown when the entity doesn't have an OwnerId property or the user doesn't own the entity.</exception>
    public void IsOwnedByCurrentUser(object entity, ClaimsPrincipal user, string errorMessage = "You must own the resource.") {
        var userId = userManager.GetUserId(user);

        var ownerIdProp = entity.GetType().GetProperty("OwnerId");

        if (ownerIdProp is null) {
            throw new UnauthorizedAccessException(errorMessage);
        }

        var ownerIdValue = ownerIdProp.GetValue(entity);
        
        // Compare as Guid if the OwnerId is a Guid, otherwise compare as strings
        bool isOwner = ownerIdValue switch {
            Guid guidValue when Guid.TryParse(userId, out var userGuid) => guidValue == userGuid,
            _ => Equals(ownerIdValue?.ToString(), userId)
        };

        if (!isOwner) {
            throw new UnauthorizedAccessException(errorMessage);
        }
    }
}