# Contact Manager - Gestionnaire de Contacts

Application web éducative ASP.NET Core MVC développée pour le cours **4DW - Développement d'application Web**.

## À propos du projet

Contact Manager est une application web de gestion de contacts et d'adresses qui démontre les concepts fondamentaux du développement web moderne avec ASP.NET Core. Ce projet sert d'exemple pédagogique pour illustrer l'architecture MVC, l'authentification, la persistance des données, la validation et les principes du Domain-Driven Design (DDD).

### Caractéristiques principales

- Gestion complète des contacts (CRUD)
- Gestion des adresses associées aux contacts
- Authentification et autorisation des utilisateurs
- Validation des données côté serveur avec FluentValidation
- Interface utilisateur responsive avec Bootstrap
- Base de données SQL Server avec Entity Framework Core

---

## Table des matières

1. [Technologies et concepts clés](#technologies-et-concepts-clés)
   - [a) ASP.NET Core MVC](#a-aspnet-core-mvc)
   - [b) ASP.NET Core Identity](#b-aspnet-core-identity)
   - [c) Entity Framework Core](#c-entity-framework-core)
   - [d) FluentValidation](#d-fluentvalidation)
   - [e) Middleware et Filters](#e-middleware-et-filters)
   - [f) Domain-Driven Design (DDD)](#f-domain-driven-design-ddd---introduction)
2. [Installation et exécution](#installation-et-exécution)
3. [Structure du projet](#structure-du-projet)
4. [Objectifs d'apprentissage](#objectifs-dapprentissage)
5. [Compte de test](#compte-de-test)
6. [Ressources additionnelles](#ressources-additionnelles)

---

## Technologies et concepts clés

### a) ASP.NET Core MVC

#### Qu'est-ce que c'est?

ASP.NET Core MVC est un framework pour construire des applications web basé sur le patron de conception **Model-View-Controller (MVC)**. Ce patron sépare l'application en trois composantes principales :

- **Model (Modèle)** : Représente les données et la logique métier
- **View (Vue)** : Gère l'affichage et la présentation des données
- **Controller (Contrôleur)** : Coordonne les interactions entre le modèle et la vue

Cette séparation des responsabilités facilite la maintenance, les tests et l'évolution de l'application.

#### Implémentation dans le projet

##### Exemple : ContactController

```csharp
// ContactManager.WebSite/Controllers/ContactController.cs

[Authorize]
public class ContactController(
    ContactManagerContext context,
    UserManager<User> userManager,
    DomainAsserts asserts) : Controller {

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ContactCreate vm) {
        if (!ModelState.IsValid) {
            return View(vm);
        }

        var toAdd = new Contact() {
            FirstName = vm.FirstName!,
            LastName = vm.LastName!,
            DateOfBirth = vm.DateOfBirth!.Value,
        };

        var user = await userManager.GetUserAsync(User);
        user!.Contacts.Add(toAdd);
        await context.SaveChangesAsync();

        return RedirectToAction(nameof(Manage));
    }
    // ... voir le fichier complet pour plus de détails
}
```

Le **routage** est configuré dans `Program.cs` pour mapper les URLs aux actions :
- `/Contact/Create` → appelle `ContactController.Create()`
- `/Contact/Edit/123` → appelle `ContactController.Edit(Guid id)`

Les **ViewModels** contiennent uniquement les données nécessaires pour une vue spécifique et incluent la validation.

#### Fichiers concernés

- `/ContactManager.WebSite/Controllers/` - Contrôleurs (Contact, Account, Home)
- `/ContactManager.WebSite/ViewModels/` - ViewModels pour chaque entité
- `/ContactManager.WebSite/Views/` - Vues Razor
- `/ContactManager.WebSite/Program.cs` - Configuration du routage

#### Pourquoi c'est important?

- **Séparer les responsabilités** : chaque composant a un rôle clair
- **Faciliter les tests** : logique testable indépendamment de l'interface
- **Améliorer la maintenabilité** : modifications localisées
- **Réutiliser le code** : un même modèle peut être affiché de différentes manières

---

### b) ASP.NET Core Identity

#### Qu'est-ce que c'est?

ASP.NET Core Identity est un système complet de gestion d'identité qui fournit :
- **Authentification** : vérifier qui est l'utilisateur (login/logout)
- **Autorisation** : déterminer ce que l'utilisateur peut faire
- Gestion des mots de passe (hachage, validation)
- Gestion des rôles et des permissions

#### Implémentation dans le projet

##### Configuration et utilisation

```csharp
// Program.cs - Configuration d'Identity
builder.Services.AddIdentity<User, IdentityRole<Guid>>()
    .AddEntityFrameworkStores<ContactManagerContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

// Plus loin dans le pipeline...
app.UseAuthentication();
app.UseAuthorization();
```

```csharp
// AccountController.cs - Exemple d'inscription
[HttpPost]
[AllowAnonymous]
public async Task<IActionResult> Register(Register vm) {
    if (!ModelState.IsValid) {
        return View(vm);
    }

    var newUser = new User(vm.UserName);
    var result = await userManager.CreateAsync(newUser, vm.Password);

    if (!result.Succeeded) {
        ModelState.AddModelError(string.Empty, "Unable to register.");
        return View(vm);
    }

    await signInManager.SignInAsync(newUser, isPersistent: true);
    return RedirectToAction("Manage", "Contact");
    // ... voir le fichier complet pour plus de détails
}
```

L'attribut **[Authorize]** protège les actions :
```csharp
[Authorize]  // Toutes les actions nécessitent une authentification
public class ContactController : Controller {
    // ...
}
```

#### Fichiers concernés

- `/ContactManager.Core/Domain/Entities/User.cs` - Entité utilisateur personnalisée
- `/ContactManager.WebSite/Controllers/AccountController.cs` - Gestion de l'authentification
- `/ContactManager.WebSite/ViewModels/Account/` - ViewModels (LogIn, Register)
- `/ContactManager.WebSite/Program.cs` - Configuration d'Identity

#### Pourquoi c'est important?

- **Sécuriser l'application** : protéger les données sensibles
- **Gérer les sessions** : maintenir l'état de connexion
- **Personnaliser l'expérience** : données spécifiques à chaque utilisateur
- **Respecter les bonnes pratiques** : hachage sécurisé, protection CSRF

---

### c) Entity Framework Core

#### Qu'est-ce que c'est?

Entity Framework Core (EF Core) est un **ORM (Object-Relational Mapper)** qui permet de :
- Travailler avec une base de données en utilisant des objets .NET
- Éviter d'écrire du SQL brut pour les opérations courantes
- Gérer automatiquement les relations entre entités
- Créer et mettre à jour le schéma de base de données via les migrations

#### Implémentation dans le projet

##### DbContext et configuration des relations

```csharp
// ContactManagerContext.cs
public class ContactManagerContext : IdentityDbContext<User, IdentityRole<Guid>, Guid> {

    public DbSet<Contact> Contacts { get; set; }
    public DbSet<Address> Addresses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        // Un User peut avoir plusieurs Contacts (One-to-Many)
        modelBuilder.Entity<User>()
            .HasMany(user => user.Contacts)
            .WithOne(contact => contact.Owner)
            .OnDelete(DeleteBehavior.Cascade);

        // Un Contact peut avoir plusieurs Addresses (One-to-Many)
        modelBuilder.Entity<Contact>()
            .HasMany(contact => contact.Addresses)
            .WithOne(address => address.Contact)
            .OnDelete(DeleteBehavior.Cascade);

        base.OnModelCreating(modelBuilder);
        modelBuilder.Seed();  // Données de test
    }
}
```

##### Entités avec relations

```csharp
// Contact.cs
public class Contact {
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }

    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required DateTime DateOfBirth { get; set; }

    // Propriété calculée (non stockée en BD)
    public int Age {
        get {
            var today = DateTime.Today;
            var age = today.Year - DateOfBirth.Year;
            if (DateOfBirth.Date > today.AddYears(-age)) age--;
            return age;
        }
    }

    // Propriétés de navigation
    [ForeignKey(nameof(OwnerId))]
    public virtual User? Owner { get; set; }
    public List<Address> Addresses { get; } = new();
}
```

##### Opérations CRUD

```csharp
// CREATE
var toAdd = new Contact() { FirstName = "John", LastName = "Doe", DateOfBirth = new DateTime(1990, 1, 1) };
user.Contacts.Add(toAdd);
await context.SaveChangesAsync();

// READ
var contact = await context.Contacts.FindAsync(id);

// UPDATE
toEdit.FirstName = "Jane";
await context.SaveChangesAsync();

// DELETE
context.Contacts.Remove(toRemove);
await context.SaveChangesAsync();
```

##### Migrations

```bash
# Créer une migration
dotnet ef migrations add NomDeLaMigration --project ContactManager.Core --startup-project ContactManager.WebSite

# Appliquer les migrations
dotnet ef database update --project ContactManager.Core --startup-project ContactManager.WebSite
```

#### Fichiers concernés

- `/ContactManager.Core/ContactManagerContext.cs` - DbContext principal
- `/ContactManager.Core/Domain/Entities/` - Entités (User, Contact, Address)
- `/ContactManager.Core/Data/SeedExtension.cs` - Initialisation des données
- `/ContactManager.Core/Migrations/` - Migrations
- `/ContactManager.WebSite/appsettings.json` - Chaîne de connexion

#### Pourquoi c'est important?

- **Travailler avec des objets** : utiliser C# au lieu de SQL
- **Gérer les relations** : navigation automatique entre entités
- **Versionner la base de données** : migrations pour suivre l'évolution
- **Protéger contre les injections SQL** : requêtes paramétrées automatiques

---

### d) FluentValidation

#### Qu'est-ce que c'est?

FluentValidation est une bibliothèque de validation qui permet de :
- Définir des règles de validation de manière fluide et lisible
- Séparer la logique de validation de la logique métier
- Réutiliser les validateurs entre différents ViewModels
- Créer des règles de validation complexes et composables

#### Implémentation dans le projet

##### Configuration

```csharp
// Program.cs
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
```

##### Exemple de validateur réutilisable

```csharp
// ContactPropertyValidators.cs
public class FirstNameValidator : AbstractValidator<string?> {
    private const int FIRST_NAME_LENGTH_MIN = 2;
    private const int FIRST_NAME_LENGTH_MAX = 30;

    public FirstNameValidator() {
        Transform(firstName => firstName, firstName => firstName!.Trim())
            .NotEmpty()
            .WithMessage("Please provide a First Name.")
            .Length(FIRST_NAME_LENGTH_MIN, FIRST_NAME_LENGTH_MAX)
            .WithMessage($"Please provide a First Name between {FIRST_NAME_LENGTH_MIN} and {FIRST_NAME_LENGTH_MAX} characters.")
            .IsValidName()
            .WithMessage("Please provide a First Name that contains only letters.");
    }
}
// ... voir le fichier complet pour LastNameValidator, BirthDateValidator, etc.
```

##### Composition dans un ViewModel

```csharp
// ContactCreate.cs
public class ContactCreate {
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime? DateOfBirth { get; set; }

    public class Validator : AbstractValidator<ContactCreate> {
        public Validator() {
            RuleFor(vm => vm.FirstName)
                .NotNull()
                .SetValidator(new FirstNameValidator());

            RuleFor(vm => vm.LastName)
                .NotNull()
                .SetValidator(new LastNameValidator());

            RuleFor(vm => vm.DateOfBirth)
                .NotNull()
                .SetValidator(new BirthDateValidator());
            // ... voir le fichier complet pour plus de détails
        }
    }
}
```

L'utilisation dans les contrôleurs est automatique via `ModelState.IsValid`.

#### Fichiers concernés

- `/ContactManager.Core/Domain/Validators/` - Validateurs de propriétés
- `/ContactManager.WebSite/ViewModels/` - Validateurs dans les ViewModels
- `/ContactManager.WebSite/Program.cs` - Configuration

#### Pourquoi c'est important?

- **Centraliser la validation** : règles réutilisables et testables
- **Améliorer la lisibilité** : syntaxe fluide et expressive
- **Séparer les responsabilités** : validation découplée des entités
- **Sécuriser l'application** : validation côté serveur robuste

---

### e) Middleware et Filters

#### Qu'est-ce que c'est?

Le **middleware** est un composant qui intercepte les requêtes HTTP et peut examiner, modifier ou terminer la requête. Les middlewares forment un **pipeline** où chaque requête passe à travers une série de composants dans un ordre spécifique.

Les **filtres** sont des attributs qui s'exécutent à des moments précis du cycle de vie d'une action (avant/après exécution, autorisation, gestion d'exceptions).

#### Implémentation dans le projet

##### Pipeline de Middleware

```csharp
// Program.cs

var app = builder.Build();

// Ordre d'exécution critique
if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();       // 1. Redirection HTTPS
app.UseStaticFiles();            // 2. Fichiers statiques (CSS, JS)
app.UseRouting();                // 3. Déterminer quelle action appeler
app.UseAuthentication();         // 4. Identifier l'utilisateur
app.UseAuthorization();          // 5. Vérifier les permissions

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
```

**Ordre d'exécution critique** :
- `UseRouting()` DOIT être avant `UseAuthentication()` et `UseAuthorization()`
- `UseAuthentication()` DOIT être avant `UseAuthorization()`

##### Filters importants

```csharp
// Authorization Filter
[Authorize]  // Protège toutes les actions du contrôleur
public class ContactController : Controller {
    [AllowAnonymous]  // Exception pour cette action
    public IActionResult PublicAction() { }
}

// Anti-Forgery Filter (protection CSRF)
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(ContactCreate vm) {
    // Vérifie la présence d'un token anti-forgery valide
}
```

#### Fichiers concernés

- `/ContactManager.WebSite/Program.cs` - Configuration du pipeline
- Tous les contrôleurs - Utilisation des filters ([Authorize], [ValidateAntiForgeryToken])

#### Pourquoi c'est important?

- **Centraliser la logique transversale** : authentification, logging, gestion d'erreurs
- **Sécuriser l'application** : protection CSRF, autorisation, HTTPS
- **Améliorer la maintenabilité** : comportements réutilisables
- **Respecter le principe DRY** : éviter de répéter la logique de sécurité

---

### f) Domain-Driven Design (DDD) - Introduction

#### Qu'est-ce que c'est?

Domain-Driven Design (DDD) est une approche de conception logicielle qui met l'accent sur :
- Le **domaine métier** comme centre de l'application
- La **logique métier** dans les entités plutôt que dans les services
- La **séparation en couches** (domaine, infrastructure, présentation)
- Les **agrégats** pour regrouper les entités liées

#### Implémentation dans le projet

##### Entités riches avec comportement

```csharp
// Contact.cs - Entité avec logique métier
public class Contact {
    public Guid Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required DateTime DateOfBirth { get; set; }

    // Logique métier centralisée dans l'entité
    public int Age {
        get {
            var today = DateTime.Today;
            var age = today.Year - DateOfBirth.Year;
            if (DateOfBirth.Date > today.AddYears(-age)) age--;
            return age;
        }
    }

    public string FullName => $"{FirstName} {LastName}";

    public virtual User? Owner { get; set; }
    public List<Address> Addresses { get; } = new();
}
```

##### Agrégats (Aggregate Roots)

```
User (Racine d'agrégat)
  └── Contacts (Collection)
        └── Addresses (Collection)
```

Les suppressions sont en cascade : supprimer un User supprime ses Contacts et leurs Addresses.

##### DomainAsserts - Règles métier

```csharp
// DomainAsserts.cs
public class DomainAsserts(UserManager<User> userManager) {

    public void Exists(object entity, string errorMessage = "The resource cannot be found.") {
        if (entity is null) {
            throw new ArgumentNullException(errorMessage);
        }
    }

    public void IsOwnedByCurrentUser(object entity, ClaimsPrincipal user,
        string errorMessage = "You must own the resource.") {
        var userId = userManager.GetUserId(user);
        var ownerIdProp = entity.GetType().GetProperty("OwnerId");
        // ... vérification de propriété (voir fichier complet)
    }
}
```

Utilisation :
```csharp
var toEdit = await context.Contacts.FindAsync(id);
asserts.Exists(toEdit, "Contact not found.");
asserts.IsOwnedByCurrentUser(toEdit, User);
```

##### Séparation en couches

```
ContactManager.Core (Domaine)
  ├── Domain/Entities/     - Entités métier
  ├── Domain/Validators/   - Règles de validation
  └── Data/                - Accès aux données

ContactManager.WebSite (Présentation)
  ├── Controllers/         - Logique de présentation
  ├── ViewModels/         - DTOs pour les vues
  └── Views/              - Interface utilisateur
```

Le domaine (Core) ne dépend pas de la présentation (WebSite), ce qui permet de réutiliser le domaine dans d'autres projets (API, Console, etc.).

#### Fichiers concernés

- `/ContactManager.Core/Domain/` - Entités et validateurs métier
- `/ContactManager.WebSite/Utilities/DomainAsserts.cs` - Assertions métier
- Structure de projets Core vs WebSite - Séparation en couches

#### Pourquoi c'est important?

- **Modéliser le métier** : le code reflète les concepts métier
- **Centraliser la logique** : éviter la duplication dans les contrôleurs
- **Faciliter l'évolution** : règles métier isolées et testables
- **Réutiliser le code** : domaine indépendant de l'interface

---

## Installation et exécution

### Prérequis

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) ou supérieur
- [SQL Server LocalDB](https://learn.microsoft.com/sql/database-engine/configure-windows/sql-server-express-localdb) (inclus avec Visual Studio)
- [Git](https://git-scm.com/downloads) pour cloner le dépôt
- Un éditeur de code ([Visual Studio](https://visualstudio.microsoft.com/), [Visual Studio Code](https://code.visualstudio.com/), ou [Rider](https://www.jetbrains.com/rider/))

### Instructions d'installation

1. **Cloner le dépôt**

```bash
git clone https://gitea.hlapointe.ca/hlapointe/contact-manager.git
cd contact-manager
```

2. **Restaurer les packages NuGet**

```bash
dotnet restore
```

3. **Appliquer les migrations à la base de données**

```bash
dotnet ef database update --project ContactManager.Core --startup-project ContactManager.WebSite
```

Cette commande crée la base de données `ContactManagerDb` et applique toutes les migrations. Les données de test (seed) sont automatiquement insérées.

4. **Exécuter l'application**

```bash
dotnet run --project ContactManager.WebSite
```

L'application sera disponible sur :
- `https://localhost:5001` (HTTPS)
- `http://localhost:5000` (HTTP)

### Commandes utiles

#### Entity Framework Core

```bash
# Créer une nouvelle migration
dotnet ef migrations add NomDeLaMigration --project ContactManager.Core --startup-project ContactManager.WebSite

# Voir l'état des migrations
dotnet ef migrations list --project ContactManager.Core --startup-project ContactManager.WebSite

# Supprimer et recréer la base de données
dotnet ef database drop --project ContactManager.Core --startup-project ContactManager.WebSite
dotnet ef database update --project ContactManager.Core --startup-project ContactManager.WebSite
```

#### Build et exécution

```bash
# Compiler le projet
dotnet build

# Nettoyer les fichiers compilés
dotnet clean

# Exécuter les tests (si présents)
dotnet test

# Publier pour la production
dotnet publish --configuration Release
```

---

## Structure du projet

### Organisation en deux projets

```
contact-manager/
│
├── ContactManager.Core/              # Projet de bibliothèque (Domain Layer)
│   ├── Domain/
│   │   ├── Entities/
│   │   │   ├── User.cs               # Entité utilisateur
│   │   │   ├── Contact.cs            # Entité contact
│   │   │   └── Address.cs            # Entité adresse
│   │   └── Validators/
│   │       ├── ContactPropertyValidators.cs
│   │       ├── AddressPropertyValidators.cs
│   │       ├── IdentityValidators.cs
│   │       └── CommonValidationRules.cs
│   ├── Data/
│   │   └── SeedExtension.cs          # Initialisation des données
│   ├── Migrations/                   # Migrations EF Core
│   └── ContactManagerContext.cs      # DbContext principal
│
├── ContactManager.WebSite/           # Projet web (Presentation Layer)
│   ├── Controllers/
│   │   ├── AccountController.cs      # Authentification
│   │   ├── ContactController.cs      # Gestion des contacts
│   │   ├── AddressController.cs      # Gestion des adresses
│   │   └── HomeController.cs         # Page d'accueil
│   ├── ViewModels/
│   │   ├── Account/
│   │   ├── Contact/
│   │   ├── Address/
│   │   └── User/
│   ├── Views/
│   │   ├── Account/
│   │   ├── Contact/
│   │   ├── Address/
│   │   ├── Home/
│   │   └── Shared/
│   ├── Utilities/
│   │   ├── DomainAsserts.cs          # Assertions métier
│   │   └── PasswordGenerator.cs
│   ├── wwwroot/                      # Fichiers statiques
│   ├── appsettings.json              # Configuration
│   └── Program.cs                    # Point d'entrée
│
└── contact-manager.sln               # Solution Visual Studio
```

### Responsabilités des projets

#### ContactManager.Core (Couche Domaine)

- Définir les entités métier (User, Contact, Address)
- Implémenter la logique métier (calcul de l'âge, nom complet)
- Définir les règles de validation du domaine
- Gérer la persistance avec Entity Framework Core

**Avantages** : Réutilisable dans différents types de projets (Web, API, Console), testable indépendamment.

#### ContactManager.WebSite (Couche Présentation)

- Gérer les requêtes HTTP (Contrôleurs)
- Afficher les données (Vues Razor)
- Transformer les données pour l'affichage (ViewModels)
- Configurer le pipeline ASP.NET Core

**Dépend de** : ContactManager.Core pour accéder aux entités et au DbContext.

---

## Objectifs d'apprentissage

À la fin de ce projet, vous devriez être capable de :

### ASP.NET Core MVC

- [ ] Comprendre le patron MVC et ses avantages
- [ ] Créer des contrôleurs avec des actions GET et POST
- [ ] Utiliser le routage pour mapper les URLs aux actions
- [ ] Créer des ViewModels pour transférer des données aux vues

### ASP.NET Core Identity

- [ ] Configurer Identity dans une application ASP.NET Core
- [ ] Implémenter l'inscription et la connexion d'utilisateurs
- [ ] Utiliser l'attribut `[Authorize]` pour protéger des actions
- [ ] Comprendre le hachage de mots de passe et la sécurité

### Entity Framework Core

- [ ] Créer un DbContext pour gérer l'accès aux données
- [ ] Définir des entités avec des propriétés de navigation
- [ ] Configurer des relations One-to-Many avec Fluent API
- [ ] Utiliser les migrations pour gérer l'évolution du schéma
- [ ] Effectuer des opérations CRUD

### FluentValidation

- [ ] Créer des validateurs réutilisables pour des propriétés
- [ ] Composer des validateurs complexes à partir de validateurs simples
- [ ] Créer des règles de validation personnalisées
- [ ] Intégrer FluentValidation avec ASP.NET Core MVC

### Middleware et Filters

- [ ] Comprendre le pipeline de middleware ASP.NET Core
- [ ] Connaître l'ordre d'exécution des middlewares
- [ ] Utiliser les filters pour implémenter des fonctionnalités transversales
- [ ] Protéger contre les attaques CSRF avec `[ValidateAntiForgeryToken]`

### Domain-Driven Design

- [ ] Créer des entités riches avec comportement métier
- [ ] Organiser le code en couches (Domaine, Présentation)
- [ ] Implémenter le concept d'agrégat et de racine d'agrégat
- [ ] Centraliser les règles métier dans le domaine

### Compétences générales

- [ ] Structurer une solution .NET avec plusieurs projets
- [ ] Utiliser l'injection de dépendances
- [ ] Appliquer les principes SOLID
- [ ] Gérer la configuration avec appsettings.json

---

## Compte de test

Pour tester l'application, utilisez le compte suivant créé automatiquement lors de l'initialisation de la base de données :

- **Nom d'utilisateur** : `hlapointe`
- **Mot de passe** : `Admin123!`
- **Rôle** : Administrateur

Ce compte contient un contact de test (Sébastien Pouliot) avec une adresse au Cégep de Saint-Hyacinthe.

### Créer votre propre compte

Vous pouvez également créer un nouveau compte en utilisant la page d'inscription :

1. Cliquez sur "Register" dans le menu
2. Entrez un nom d'utilisateur (minimum 3 caractères)
3. Créez un mot de passe sécurisé
4. Confirmez le mot de passe
5. Cliquez sur "Register"

Vous serez automatiquement connecté après l'inscription.

---

## Ressources additionnelles

### Documentation officielle Microsoft

#### ASP.NET Core
- [Vue d'ensemble d'ASP.NET Core](https://learn.microsoft.com/aspnet/core/)
- [Tutoriel ASP.NET Core MVC](https://learn.microsoft.com/aspnet/core/tutorials/first-mvc-app/start-mvc)
- [Architecture MVC](https://learn.microsoft.com/aspnet/core/mvc/overview)

#### ASP.NET Core Identity
- [Introduction à Identity](https://learn.microsoft.com/aspnet/core/security/authentication/identity)
- [Configuration d'Identity](https://learn.microsoft.com/aspnet/core/security/authentication/identity-configuration)
- [Autorisation dans ASP.NET Core](https://learn.microsoft.com/aspnet/core/security/authorization/introduction)

#### Entity Framework Core
- [Vue d'ensemble d'EF Core](https://learn.microsoft.com/ef/core/)
- [Tutoriel EF Core](https://learn.microsoft.com/ef/core/get-started/overview/first-app)
- [Migrations](https://learn.microsoft.com/ef/core/managing-schemas/migrations/)
- [Relations](https://learn.microsoft.com/ef/core/modeling/relationships)

#### Middleware et Filters
- [Middleware ASP.NET Core](https://learn.microsoft.com/aspnet/core/fundamentals/middleware/)
- [Filters dans ASP.NET Core](https://learn.microsoft.com/aspnet/core/mvc/controllers/filters)
- [Prévention CSRF](https://learn.microsoft.com/aspnet/core/security/anti-request-forgery)

### FluentValidation
- [Documentation officielle FluentValidation](https://docs.fluentvalidation.net/)
- [Intégration avec ASP.NET Core](https://docs.fluentvalidation.net/en/latest/aspnet.html)
- [Built-in Validators](https://docs.fluentvalidation.net/en/latest/built-in-validators.html)

### Domain-Driven Design

#### Livres recommandés
- **Domain-Driven Design: Tackling Complexity in the Heart of Software** par Eric Evans
- **Implementing Domain-Driven Design** par Vaughn Vernon
- **Domain-Driven Design Distilled** par Vaughn Vernon (version condensée)

#### Articles et tutoriels
- [DDD by Example (Martin Fowler)](https://martinfowler.com/tags/domain%20driven%20design.html)
- [Microsoft Architecture Guides - DDD](https://learn.microsoft.com/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/ddd-oriented-microservice)

### C# et .NET
- [Tour du langage C#](https://learn.microsoft.com/dotnet/csharp/tour-of-csharp/)
- [Guide de programmation C#](https://learn.microsoft.com/dotnet/csharp/programming-guide/)
- [Asynchronous programming (async/await)](https://learn.microsoft.com/dotnet/csharp/programming-guide/concepts/async/)

### Vidéos et cours en ligne
- [Microsoft Learn - ASP.NET Core](https://learn.microsoft.com/training/browse/?products=aspnet-core)
- [YouTube - dotNET channel](https://www.youtube.com/c/dotNET)

### Communautés et forums
- [Stack Overflow - ASP.NET Core](https://stackoverflow.com/questions/tagged/asp.net-core)
- [Reddit - r/dotnet](https://www.reddit.com/r/dotnet/)
- [.NET Discord](https://discord.gg/dotnet)

---

## Licence

Ce projet est un exemple éducatif créé pour le cours **4DW - Développement d'application Web**. Il est fourni à des fins d'apprentissage uniquement.

---

## Auteur

**Hugo Lapointe**
Enseignant - Développement d'application Web (4DW)
Contact : [https://gitea.hlapointe.ca/hlapointe](https://gitea.hlapointe.ca/hlapointe)

---

## Support

Pour toute question concernant ce projet :

1. Consultez la documentation officielle mentionnée dans les ressources
2. Posez vos questions en classe ou durant les périodes de laboratoire
3. Utilisez le système de gestion des issues du dépôt Git (si activé)

Bon apprentissage!
