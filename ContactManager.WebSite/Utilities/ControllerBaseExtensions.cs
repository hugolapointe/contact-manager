using Microsoft.AspNetCore.Mvc;

using System.Security.Claims;

namespace ContactManager.WebSite.Utilities;

public static class ControllerBaseExtensions {
    private const string FlashErrorKey = "Error";
    private const string FlashSuccessKey = "Success";

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

    // Utiliser dans un flux PRG (Post/Redirect/Get) pour afficher un message d'erreur ponctuel
    // après redirection, via la partial partagée de feedback.
    public static void SetErrorMessage(this Controller controller, string errorMessage) {
        controller.TempData[FlashErrorKey] = errorMessage;
    }

    // Utiliser dans un flux PRG pour afficher un message de succès ponctuel après redirection.
    public static void SetSuccessMessage(this Controller controller, string successMessage) {
        controller.TempData[FlashSuccessKey] = successMessage;
    }
}
