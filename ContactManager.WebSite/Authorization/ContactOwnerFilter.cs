using ContactManager.Core;
using ContactManager.Core.Domain.Entities;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace ContactManager.WebSite.Authorization;

// Charge le contact, valide l'ownership, puis le stocke dans HttpContext.
public class ContactOwnerFilter(string routeParameter, ContactManagerContext context)
: IAsyncActionFilter {

    public async Task OnActionExecutionAsync(ActionExecutingContext filterContext, ActionExecutionDelegate next) {
        // 1) On lit l'identifiant de route (id/contactId selon l'action).
        if (!filterContext.ActionArguments.TryGetValue(routeParameter, out var value) || value is not Guid entityId) {
            filterContext.Result = new BadRequestResult();
            return;
        }

        // 2) On charge explicitement Contact : c'est la seule ressource d'ownership du cours.
        var contact = await context.Contacts.FindAsync(entityId);
        if (contact is null) {
            filterContext.Result = new NotFoundResult();
            return;
        }

        // 3) On compare le propriétaire de la ressource et l'utilisateur connecté.
        var userId = filterContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userId, out var currentUserId) || contact.OwnerId != currentUserId) {
            filterContext.Result = new ForbidResult();
            return;
        }

        filterContext.HttpContext.SetContactOwned(contact);
        await next();
    }
}

public static class ContactOwnedExtensions {
    private const string Key = "ContactOwned";

    internal static void SetContactOwned(this HttpContext context, Contact contact)
        => context.Items[Key] = contact;

    public static Contact GetContactOwned(this HttpContext context)
        => (Contact)context.Items[Key]!;
}
