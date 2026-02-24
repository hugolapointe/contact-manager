using Microsoft.AspNetCore.Mvc;

namespace ContactManager.WebSite.Authorization;

// Vérifie que l'utilisateur courant est propriétaire de la ressource.
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class ResourceOwnerAttribute : TypeFilterAttribute {
    // routeParameter permet de réutiliser le filtre pour id/contactId sans logique supplémentaire.
    public ResourceOwnerAttribute(string routeParameter = "id")
        : base(typeof(ResourceOwnerFilter)) {
        Arguments = [routeParameter];
    }
}
