# Plan d'amélioration architecturale : Migration vers DDD authentique

## Objectifs

**Problème actuel :**
- `DomainAsserts` utilise la réflexion et viole les principes DDD
- Logique d'autorisation dans la couche présentation
- Répétition dans chaque action de contrôleur

**Solution proposée : Approche hybride**
1. **Exceptions de domaine** - Communication claire des violations
2. **Méthodes d'agrégat** - Validation explicite pour opérations d'écriture
3. **Filtres d'autorisation** - Autorisation déclarative pour opérations de lecture

**Intention :**
- Respecter les principes DDD (agrégats protègent leurs invariants)
- Améliorer la testabilité (pas de réflexion)
- Séparer les préoccupations (domaine vs présentation)
- Valeur éducative pour les étudiants

---

## Phase 1 : Exceptions de domaine

**Intention :** Créer un vocabulaire de domaine pour communiquer les violations de règles métier.

**Tâches :**
- [ ] Créer `/ContactManager.Core/Domain/Exceptions/`
- [ ] Créer `DomainException` (classe de base abstraite)
- [ ] Créer `EntityNotFoundException` (entité manquante)
- [ ] Créer `UnauthorizedResourceAccessException` (violation de propriété)

**Exemple d'utilisation :**
```csharp
throw new UnauthorizedResourceAccessException(nameof(Contact), Id, attemptedUserId);
```

---

## Phase 2 : Comportement d'agrégat

**Intention :** Les agrégats valident leurs propres invariants, incluant la propriété (ownership).

**Tâches :**
- [ ] Ajouter méthode `ValidateOwnership(Guid userId)` à `Contact`
- [ ] Ajouter méthode `UpdatePersonalInfo(Guid userId, ...)` à `Contact`
- [ ] Ajouter méthode `AddAddress(Guid userId, Address address)` à `Contact`
- [ ] Ajouter méthode `RemoveAddress(Guid userId, Guid addressId)` à `Contact`

**Fichier à modifier :**
- `/ContactManager.Core/Domain/Entities/Contact.cs`

**Exemple d'utilisation :**
```csharp
// Dans le contrôleur
contact.UpdatePersonalInfo(currentUserId, firstName, lastName, dateOfBirth);
// Lance UnauthorizedResourceAccessException si userId != OwnerId
```

**Pourquoi :**
- Validation explicite et type-safe
- Testable sans dépendances ASP.NET Core
- Montre DDD en action (comportement dans l'agrégat)

---

## Phase 3 : Filtre d'autorisation

**Intention :** Autorisation déclarative pour réduire le code boilerplate dans les contrôleurs.

**Tâches :**
- [ ] Créer `/ContactManager.WebSite/Filters/`
- [ ] Créer `RequireContactOwnershipAttribute` (attribut)
- [ ] Créer `ContactOwnershipFilter` (implémentation IAsyncActionFilter)
- [ ] Le filtre valide la propriété avant l'action et stocke le contact validé dans `HttpContext.Items`

**Fichier à créer :**
- `/ContactManager.WebSite/Filters/ResourceAuthorizationFilter.cs`

**Exemple d'utilisation :**
```csharp
[RequireContactOwnership]
public async Task<IActionResult> Edit(Guid id) {
    var contact = HttpContext.Items["ValidatedContact"] as Contact;
    // Le filtre a déjà validé la propriété
}
```

**Pourquoi :**
- Réduit la duplication dans les actions de lecture
- Pattern standard ASP.NET Core
- Moins verbeux que la validation manuelle

---

## Phase 4 : Middleware de gestion d'exceptions

**Intention :** Convertir automatiquement les exceptions de domaine en réponses HTTP appropriées.

**Tâches :**
- [ ] Créer `/ContactManager.WebSite/Middleware/`
- [ ] Créer `DomainExceptionMiddleware`
  - `EntityNotFoundException` → 404 Not Found
  - `UnauthorizedResourceAccessException` → 403 Forbidden
  - `DomainException` → 400 Bad Request
- [ ] Enregistrer dans `Program.cs` après `UseRouting()`, avant `UseAuthentication()`

**Fichier à créer :**
- `/ContactManager.WebSite/Middleware/DomainExceptionMiddleware.cs`

**Fichier à modifier :**
- `/ContactManager.WebSite/Program.cs`

**Pourquoi :**
- Gestion centralisée des erreurs
- Évite les try/catch répétés dans les contrôleurs
- Réponses HTTP cohérentes

---

## Phase 5 : Migration des contrôleurs

**Intention :** Remplacer progressivement `DomainAsserts` par la nouvelle approche.

### 5.1 UserController (Simple - uniquement vérifications d'existence)

**Tâches :**
- [ ] Remplacer `asserts.Exists()` par vérifications null
- [ ] Lancer `EntityNotFoundException` si null
- [ ] Supprimer la dépendance à `DomainAsserts`

**Fichier à modifier :**
- `/ContactManager.WebSite/Controllers/UserController.cs`

**Pourquoi :** Contrôleur le plus simple, bon point de départ.

### 5.2 ContactController (Mixte - filtres + méthodes d'agrégat)

**Tâches :**
- [ ] **Actions GET (Edit, Remove GET)** : Ajouter `[RequireContactOwnership]`
- [ ] **Actions POST (Create, Edit POST)** : Utiliser méthodes d'agrégat (`UpdatePersonalInfo()`)
- [ ] **Remove POST** : Vérifier propriété puis supprimer
- [ ] Supprimer la dépendance à `DomainAsserts`

**Fichier à modifier :**
- `/ContactManager.WebSite/Controllers/ContactController.cs`

**Exemple avant :**
```csharp
public async Task<IActionResult> Edit(Guid id, ContactEdit vm) {
    var contact = await context.Contacts.FindAsync(id);
    asserts.Exists(contact, "Contact not found.");
    asserts.IsOwnedByCurrentUser(contact, User);

    contact.FirstName = vm.FirstName!;
    // ...
}
```

**Exemple après :**
```csharp
public async Task<IActionResult> Edit(Guid id, ContactEdit vm) {
    var contact = await context.Contacts.FindAsync(id);
    if (contact is null) throw new EntityNotFoundException(nameof(Contact), id);

    var currentUser = await userManager.GetUserAsync(User);
    contact.UpdatePersonalInfo(currentUser!.Id, vm.FirstName!, vm.LastName!, vm.DateOfBirth!.Value);
    // Lance UnauthorizedResourceAccessException si pas propriétaire
}
```

**Pourquoi :**
- Montre les deux approches (filtres pour GET, agrégat pour POST)
- Valeur éducative maximale

### 5.3 AddressController (Complexe - validation via Contact parent)

**Tâches :**
- [ ] Charger le `Contact` parent via navigation
- [ ] Valider la propriété via `contact.ValidateOwnership(userId)`
- [ ] Supprimer la dépendance à `DomainAsserts`

**Fichier à modifier :**
- `/ContactManager.WebSite/Controllers/AddressController.cs`

**Exemple :**
```csharp
public async Task<IActionResult> Edit(Guid id, AddressEdit vm) {
    var address = await context.Addresses.FindAsync(id);
    if (address is null) throw new EntityNotFoundException(nameof(Address), id);

    // Charger le Contact parent
    await context.Entry(address).Reference(a => a.Contact).LoadAsync();
    if (address.Contact is null) throw new EntityNotFoundException(nameof(Contact), address.ContactId);

    var currentUser = await userManager.GetUserAsync(User);
    address.Contact.ValidateOwnership(currentUser!.Id);

    // Mettre à jour l'adresse
    address.StreetNumber = vm.StreetNumber!.Value;
    // ...
}
```

**Pourquoi :** Démontre la validation d'ownership transitif via l'agrégat parent.

---

## Phase 6 : Nettoyage

**Intention :** Supprimer l'ancien code et finaliser la documentation.

**Tâches :**
- [ ] Supprimer `/ContactManager.WebSite/Utilities/DomainAsserts.cs`
- [ ] Supprimer `builder.Services.AddScoped<DomainAsserts>();` de `Program.cs`
- [ ] Mettre à jour `README.md` - Ajouter section "Authorization Pattern"
- [ ] Mettre à jour `CLAUDE.md` - Documenter la nouvelle approche
- [ ] Vérifier que tous les tests passent
- [ ] Tester manuellement les scénarios d'autorisation

---

## Ordre d'exécution recommandé

1. **Phase 1** (Exceptions) - Fondation, pas de changements cassants
2. **Phase 2** (Agrégat) - Ajoute comportement, pas de changements cassants
3. **Phase 4** (Middleware) - Avant Phase 3 pour capturer les exceptions du filtre
4. **Phase 3** (Filtre) - Infrastructure prête mais pas encore utilisée
5. **Phase 5** (Migration) - UserController → ContactController → AddressController
6. **Phase 6** (Nettoyage) - Quand tout fonctionne

---

## Vérification rapide

**Comment savoir si c'est réussi :**
- [ ] Aucune utilisation de `DomainAsserts` dans le code
- [ ] Aucune réflexion pour la validation de propriété
- [ ] Les contrôleurs sont plus lisibles
- [ ] Les tests sont plus faciles à écrire
- [ ] Les étudiants comprennent le DDD
- [ ] Tous les tests passent
- [ ] Les codes HTTP sont corrects (401, 403, 404)

---

## Fichiers critiques

**À créer :**
- `/ContactManager.Core/Domain/Exceptions/*.cs` (3 fichiers)
- `/ContactManager.WebSite/Filters/ResourceAuthorizationFilter.cs`
- `/ContactManager.WebSite/Middleware/DomainExceptionMiddleware.cs`

**À modifier :**
- `/ContactManager.Core/Domain/Entities/Contact.cs`
- `/ContactManager.WebSite/Controllers/UserController.cs`
- `/ContactManager.WebSite/Controllers/ContactController.cs`
- `/ContactManager.WebSite/Controllers/AddressController.cs`
- `/ContactManager.WebSite/Program.cs`
- `/README.md`
- `/CLAUDE.md`

**À supprimer :**
- `/ContactManager.WebSite/Utilities/DomainAsserts.cs`

---

## Approche hybride - Quand utiliser quoi

| Scénario | Approche | Raison |
|----------|----------|--------|
| Action GET (lecture) | Filtre `[RequireContactOwnership]` | Déclaratif, réduit boilerplate |
| Action POST (création) | Méthode d'agrégat | Validation explicite, testable |
| Action POST (modification) | Méthode d'agrégat | Encapsule logique métier |
| Action POST (suppression) | Vérification manuelle + agrégat | Besoin de supprimer après validation |
| Validation d'existence | `EntityNotFoundException` | Communication claire |
| Validation de propriété | Méthode d'agrégat ou filtre | Selon GET vs POST |

---

## Notes pour l'implémentation

**Important :**
- Tester après chaque phase
- Commiter fréquemment (chaque tâche = 1 commit)
- Garder l'ancien code jusqu'à ce que le nouveau soit testé
- Documenter les changements au fur et à mesure

**Pour les étudiants :**
- Expliquer pourquoi on change (DDD, testabilité)
- Montrer les exemples avant/après
- Comparer les approches dans le README
- Mettre en évidence les avantages de chaque pattern
