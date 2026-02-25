using Microsoft.AspNetCore.Mvc;

namespace ContactManager.WebSite.Authorization;

// Vérifie que l'utilisateur courant est propriétaire de la ressource.
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class ContactOwnerAttribute : TypeFilterAttribute {

    // routeParameter permet de réutiliser le filtre pour id/contactId sans logique supplémentaire.
    public ContactOwnerAttribute(string routeParameter = "id"): base(typeof(ContactOwnerFilter)) {
        Arguments = [routeParameter];
    }
}
