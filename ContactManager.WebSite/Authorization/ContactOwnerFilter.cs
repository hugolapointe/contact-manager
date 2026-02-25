using ContactManager.Core;
using ContactManager.Core.Domain.Entities;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace ContactManager.WebSite.Authorization;

// Charge la ressource, valide l'ownership, puis la stocke dans HttpContext.
public class ContactOwnerFilter(string routeParameter, ContactManagerContext context) 
: IAsyncActionFilter {

    public async Task OnActionExecutionAsync(ActionExecutingContext filterContext, ActionExecutionDelegate next) {
        // 1) On lit l'identifiant de route (id/contactId selon l'action).
        if (!filterContext.ActionArguments.TryGetValue(routeParameter, out var value) || value is not Guid entityId) {
            filterContext.Result = new BadRequestResult();
            return;
        }

        // 2) On charge explicitement Contact: c'est la seule ressource d'ownership du cours.
        var contact = await context.Contacts.FindAsync(entityId);
        if (contact is null) {
            filterContext.Result = new NotFoundResult();
            return;
        }

        // 3) On compare le propriétaire de la ressource et l'utilisateur connecté.
        var userId = filterContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userId, out var currentUserId) || contact.OwnerId != currentUserId) {
            // Non propriétaire => interdit (même comportement qu'avant).
            filterContext.Result = new ForbidResult();
            return;
        }

        filterContext.HttpContext.SetResourceOwned(contact);
        await next();
    }
}

public static class ResourceOwnedExtensions {
    private const string Key = "ResourceOwned";

    internal static void SetResourceOwned(this HttpContext context, IOwned entity)
        => context.Items[Key] = entity;

    public static T GetResourceOwned<T>(this HttpContext context) where T : class, IOwned
        => (T)context.Items[Key]!;
}
