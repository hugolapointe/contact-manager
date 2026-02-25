# Contact Manager

![.NET 8](https://img.shields.io/badge/.NET-8-512BD4?logo=dotnet)
![ASP.NET Core MVC](https://img.shields.io/badge/ASP.NET%20Core-MVC-5C2D91)
![EF Core](https://img.shields.io/badge/Entity%20Framework-Core-6DB33F)
![Identity](https://img.shields.io/badge/ASP.NET%20Core-Identity-0A66C2)
![FluentValidation](https://img.shields.io/badge/Validation-FluentValidation-1F7A8C)


Projet éducatif pour présenter les notions de base de **ASP.NET Core MVC**, **ASP.NET Core Identity** et **FluentValidation**, dans un contexte simple de gestion de contacts.

## Objectif pédagogique

Ce projet montre comment:
- structurer une application web MVC;
- authentifier et autoriser les utilisateurs avec Identity;
- valider les entrées avec FluentValidation;
- persister les données avec EF Core et SQL Server.

## Structure du projet

- `ContactManager.Core/`
  - `Domain/Entities/`: entités métier (`AppUser`, `Contact`, `Address`, `AppRole`)
  - `Domain/Validators/`: règles de validation
  - `BaseEntity` et `IAudible` : gestion automatique des dates de création et de modification
  - `Data/RuntimeSeed.cs`: données de démarrage injectées au runtime (rôles, utilisateur, contact/adresse)
  - `ContactManagerContext.cs`: configuration EF Core et relations
- `ContactManager.WebSite/`
  - `Controllers/`: cas d’utilisation MVC
  - `ViewModels/`: modèles d’entrée/sortie des vues + validation
  - `Views/`: UI Razor
  - `Authorization/`: filtre de contrôle de propriété des ressources
  - `Program.cs`: configuration DI, Identity, EF Core et pipeline HTTP


## Domaine

| Entité | Rôle | Relations principales |
|---|---|---|
| `AppUser` | Utilisateur de l’application (Identity) | 1 utilisateur possède plusieurs `Contact` |
| `AppRole` | Rôle métier de l’application | Hérite d’IdentityRole, utilisé pour l’authentification |
| `Contact` | Contact appartenant à un utilisateur | appartient à `AppUser`, possède plusieurs `Address` |
| `Address` | Adresse d’un contact | appartient à `Contact` |

## MVC

Controllers définis dans `ContactManager.WebSite/Controllers/`:

| Controller | Rôle | Actions principales |
|---|---|---|
| `HomeController` | Navigation générale | `Index`, `Privacy`, `Error` |
| `AccountController` | Authentification | `Login`, `Register`, `LogOut` |
| `ContactController` | Gestion des contacts utilisateur | `Manage`, `Create`, `Edit`, `Remove` |
| `AddressController` | Gestion des adresses d’un contact | `Manage`, `Create`, `Edit`, `Remove` |
| `UserController` | Administration des utilisateurs (admin) | `Manage`, `Create`, `ResetPassword`, `Remove` |

## Fonctionnalités

Actions principales offertes par l’application.

### Visiteur (non connecté)

- Créer un compte.
- Se connecter à l’application.

### Utilisateur connecté

- Accéder à la gestion des contacts depuis l’accueil.
- Créer, modifier et supprimer ses contacts.
- Ajouter, modifier et supprimer les adresses de ses contacts.
- Se déconnecter.

### Administrateur

- Consulter la liste des utilisateurs.
- Créer un utilisateur et lui attribuer un rôle.
- Réinitialiser le mot de passe d’un utilisateur.
- Supprimer un utilisateur (avec protections sur l’auto-suppression et le dernier administrateur).

### Règle d’autorisation métier

- Modifier et supprimer uniquement ses propres contacts et adresses grâce au contrôle de propriété `ResourceOwner`.

## Comptes de test

Données seedées dynamiquement au runtime dans `ContactManager.Core/Data/RuntimeSeed.cs` :
- **Nom d’utilisateur**: `hlapointe` / **Mot de passe**: `Admin123!` / **Rôle**: `Administrator`
- **Nom d’utilisateur**: `mbouchard` / **Mot de passe**: `User123!A` / **Rôle**: `User`

## Guide de démarrage rapide

1. **Configurer la base de données** :
  - Vérifiez la chaîne de connexion dans `ContactManager.WebSite/appsettings.json` (clé `DefaultConnection`).
2. **Appliquer les migrations EF Core** :
  - Ouvrez un terminal à la racine de la solution.
  - Exécutez :
    ```
    dotnet ef database update -p ContactManager.Core/ContactManager.Core.csproj -s ContactManager.WebSite/ContactManager.WebSite.csproj -c ContactManagerContext
    ```
    (Le paramètre `-p` indique le projet contenant les migrations, `-s` le projet de démarrage, et `-c` le contexte.)
3. **Lancer l’application** :
  - Démarrez le projet `ContactManager.WebSite`.
  - Connectez-vous avec un des comptes de test ci-dessus.

---

## Notions et patterns démontrés

### Architecture et séparation des responsabilités
- **Séparation stricte** entre la couche domaine/persistance (`ContactManager.Core`) et la couche présentation MVC (`ContactManager.WebSite`).
- **Entités métier** (`AppUser`, `Contact`, `Address`, `AppRole`) : encapsulent la logique métier et les règles d’intégrité.
- **Seed runtime** : les données de test sont injectées dynamiquement au démarrage (voir `RuntimeSeed.cs`).

### Validation et ViewModels
- **Validation côté serveur** : chaque ViewModel possède un validateur FluentValidation dédié (ex : `ContactCreate.Validator`).
- **Affichage des erreurs** : les erreurs de validation sont affichées dans les vues Razor via le ModelState.

### Authentification et autorisation
- **ASP.NET Core Identity** : gestion des utilisateurs (`AppUser`) et des rôles personnalisés (`AppRole`).
- **Filtre d’autorisation personnalisé** : l’attribut `[ResourceOwner]` garantit qu’un utilisateur ne peut modifier que ses propres contacts/adresses (pattern d’ownership).

### Pattern POST/Redirect/GET (PRG) et feedback utilisateur
- **PRG** : après une action POST (création, édition, suppression), l’utilisateur est redirigé (RedirectToAction) pour éviter la double soumission de formulaire.
- **Messages flash** : les messages d’erreur ou de succès sont stockés dans `TempData` et affichés via la vue partielle `_RequestFeedbackAlerts.cshtml`.
  - Utilisation des méthodes d’extension `SetErrorMessage` et `SetSuccessMessage` dans les contrôleurs.

### Audit automatique
- **Timestamps** : toutes les entités héritent de `BaseEntity` qui gère automatiquement les dates de création (`CreatedAt`) et de modification (`UpdateAt`).

### Bonnes pratiques démontrées
- **Propriétés calculées non persistées** : propriétés comme `Age` et `FullName` sont décorées avec `[NotMapped]` pour ne pas être stockées en base.
- **Utilisation de partials Razor** : pour la réutilisation de l’affichage des messages de feedback.
