using Microsoft.AspNetCore.Mvc;

using System.Security.Claims;

namespace ContactManager.WebSite.Utilities;

public enum NotificationType { Success, Error, Info, Warning }

public static class ControllerBaseExtensions {

    // Utiliser quand une action exige un utilisateur authentifié avec un NameIdentifier valide.
    // Cette méthode est utile pour simplifier les actions qui ont besoin d'un Guid utilisateur
    // et éviter de répéter le parsing/validation dans chaque contrôleur.
    public static Guid GetRequiredUserId(this ControllerBase controller) {
        var userIdRaw = controller.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdRaw, out var userId)) {
            throw new InvalidOperationException("Authenticated user does not contain a valid user identifier claim.");
        }

        return userId;
    }


    // Ajoute une ou plusieurs erreurs personnalisées au ModelState.
    public static void AddModelErrors(this Controller controller, IEnumerable<string> errors) {
        foreach (var error in errors.Where(error => !string.IsNullOrWhiteSpace(error))) {
            controller.ModelState.AddModelError(string.Empty, error);
        }
    }

    // Variante pratique pour une seule erreur personnalisée.
    public static void AddModelError(this Controller controller, string error) {
        if (!string.IsNullOrWhiteSpace(error)) {
            controller.ModelState.AddModelError(string.Empty, error);
        }
    }

    // Ajoute une notification générique (succès, erreur, info, warning) à TempData
    public static void AddNotification(this Controller controller, string message, NotificationType type) {
        controller.TempData[$"Notification.{type}"] = message;
    }
}
