# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

ContactManager is an ASP.NET Core 8.0 MVC web application for managing contacts and their addresses, built with Entity Framework Core and ASP.NET Identity for authentication.

## Build and Run Commands

### Build
```bash
dotnet build
```

### Run the application
```bash
dotnet run --project ContactManager.WebSite
```

### Database Migrations
```bash
# Create a new migration
dotnet ef migrations add <MigrationName> --project ContactManager.Core --startup-project ContactManager.WebSite

# Apply migrations to database
dotnet ef database update --project ContactManager.Core --startup-project ContactManager.WebSite

# Drop database
dotnet ef database drop --project ContactManager.Core --startup-project ContactManager.WebSite
```

### Run with hot reload
```bash
dotnet watch --project ContactManager.WebSite
```

## Architecture

### Project Structure

**ContactManager.Core** - Domain layer containing:
- Domain entities (User, Contact, Address)
- DbContext (ContactManagerContext)
- FluentValidation validators for domain properties
- Data seeding logic

**ContactManager.WebSite** - Presentation layer containing:
- MVC Controllers
- Razor Views
- ViewModels with nested FluentValidation validators
- Utilities (DomainAsserts, PasswordGenerator)

### Domain Model Hierarchy

The application uses a three-tier entity hierarchy with cascade delete:
- **User** (ASP.NET Identity user with Guid ID)
  - Has many **Contacts** (OwnerId foreign key)
    - Each Contact has many **Addresses** (ContactId foreign key)

All entities use `Guid` as primary keys. When a User is deleted, all their Contacts cascade delete. When a Contact is deleted, all its Addresses cascade delete.

### Validation Pattern

The codebase uses a two-layer validation approach with FluentValidation:

1. **Property Validators** (`ContactManager.Core/Domain/Validators/`) - Reusable validators for individual properties (e.g., `FirstNameValidator`, `LastNameValidator`, `StreetNumberValidator`)

2. **ViewModel Validators** - Each ViewModel contains a nested `Validator` class that composes property validators:
```csharp
public class ContactCreate {
    public string? FirstName { get; set; }

    public class Validator : AbstractValidator<ContactCreate> {
        public Validator() {
            RuleFor(vm => vm.FirstName)
                .NotNull()
                .SetValidator(new FirstNameValidator());
        }
    }
}
```

Property validators are defined in `ContactManager.Core/Domain/Validators/` and reused across multiple ViewModels.

### Authorization Pattern

Controllers use the `DomainAsserts` utility class (registered as scoped service) for ownership validation:

```csharp
var contact = await context.Contacts.FindAsync(id);
asserts.Exists(contact, "Contact not found.");
asserts.IsOwnedByCurrentUser(contact, User);
```

`DomainAsserts` uses reflection to check the `OwnerId` property and compares it with the current user's ID. It handles both Guid and string comparison.

### Database Connection

Uses SQL Server LocalDB by default: `Server=(localdb)\\mssqllocaldb;Database=ContactManagerDb`

Connection string is in `appsettings.json` and can be overridden in `appsettings.Development.json`.

### ViewModels and Entity Mapping

ViewModels are separate from entities and require manual mapping in controllers. There is no AutoMapper or similar library. When creating/editing:
1. Controllers receive ViewModels from POST requests
2. Manual property mapping creates/updates entities
3. Entity relationships are established explicitly (e.g., `user.Contacts.Add(toAdd)`)

### Key Dependencies

- **FluentValidation.AspNetCore** (11.3.1) - Automatic validation integration with MVC model binding
- **Microsoft.AspNetCore.Identity.EntityFrameworkCore** (8.0.21) - User authentication/authorization
- **Microsoft.EntityFrameworkCore.SqlServer** (8.0.21) - Database access
- **Swashbuckle.AspNetCore** (6.9.0) - API documentation (if applicable)

## Important Patterns

### Controller Constructor Injection
Controllers use primary constructors (C# 12 feature) with dependency injection:
```csharp
public class ContactController(
    ContactManagerContext context,
    UserManager<User> userManager,
    DomainAsserts asserts) : Controller {
```

### Navigation Property Loading
Entity relationships must be explicitly loaded:
```csharp
context.Entry(user!).Collection(u => u.Contacts).Load();
```

### Required Properties
Entities use C# 11+ `required` keyword for non-nullable reference properties that must be set during initialization.

### Calculated Properties
Domain entities include computed properties (e.g., `Contact.Age`, `Contact.FullName`) that derive values from other properties.
