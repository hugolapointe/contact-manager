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
  - `Domain/Entities/`: entités métier (`AppUser`, `Contact`, `Address`)
  - `Domain/Guards/` et `Domain/Validators/`: invariants et règles de validation
  - `Data/SeedExtension.cs`: données de démarrage (rôles, utilisateur, contact/adresse)
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

## Décisions conceptuelles importantes

- **Autorisation personnalisée de propriété de ressource**: un composant dédié (`ResourceOwnerFilter`) est branché dans le pipeline MVC via l’attribut `ResourceOwner`, charge la ressource, valide l’ownership, puis la stocke dans `HttpContext`.
- **Séparation des responsabilités (Core vs WebSite)**: le domaine et la persistance sont isolés de la couche présentation.
- **Invariants métier au niveau domaine**: règles critiques appliquées dans les entités/guards pour éviter des états invalides.
- **Validation d’entrée au niveau MVC**: FluentValidation est utilisé pour les ViewModels.
- **Intégration Identity native**: authentification/autorisation standard via ASP.NET Core Identity avec `AppUser`.
- **Propriétés calculées non persistées**: `Age` et `FullName` sont explicitement exclues du mapping EF Core (`[NotMapped]`).

## Compte de test

Données seedées dans `ContactManager.Core/Data/SeedExtension.cs`:
- **Nom d’utilisateur**: `hlapointe`
- **Mot de passe**: `Admin123!`
- **Rôle**: `Administrator`
