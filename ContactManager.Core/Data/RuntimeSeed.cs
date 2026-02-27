using ContactManager.Core.Domain.Entities;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ContactManager.Core.Data;

public static class RuntimeSeed {
    private static readonly IReadOnlyList<SeedUser> SeedUsers = [
        new(
            UserName: "hlapointe",
            Password: "Admin123!",
            RoleName: AppRole.AdministratorName,
            Contacts: [
                new("Sébastien", "Pouliot", new DateTime(1980, 02, 06), [
                    new(3000, "Boulevard Boullé", "Saint-Hyacinthe", "J2S 1H9"),
                    new(12, "Rue Girouard Ouest", "Saint-Hyacinthe", "J2S 2Y5")
                ]),
                new("Émile", "Bouchard", new DateTime(1991, 09, 21), [
                    new(401, "Rue des Cascades", "Saint-Hyacinthe", "J2S 3G7")
                ]),
                new("Camille", "Dion", new DateTime(1988, 04, 13), [
                    new(77, "Avenue Sainte-Anne", "Saint-Hyacinthe", "J2S 5M1"),
                    new(910, "Boulevard Laframboise", "Saint-Hyacinthe", "J2S 4W7"),
                    new(8, "Rue Saint-Antoine", "Saint-Hyacinthe", "J2S 3K8")
                ]),
                new("Noah", "Tremblay", new DateTime(1995, 11, 02), [
                    new(255, "Rue Dessaulles", "Saint-Hyacinthe", "J2S 2T4")
                ])
            ]),
        new(
            UserName: "mbouchard",
            Password: "User123!A",
            RoleName: AppRole.UserName,
            Contacts: [
                new("Léa", "Gauthier", new DateTime(1992, 07, 17), [
                    new(18, "Rue Cartier", "Longueuil", "J4K 2V9"),
                    new(245, "Chemin Chambly", "Longueuil", "J4H 3L4")
                ]),
                new("Thomas", "Paradis", new DateTime(1987, 01, 28), [
                    new(990, "Boulevard Curé-Poirier", "Longueuil", "J4J 4Y5")
                ]),
                new("Ariane", "Leduc", new DateTime(1999, 03, 09), [
                    new(33, "Rue Saint-Charles", "Longueuil", "J4H 1C2"),
                    new(120, "Rue Green", "Longueuil", "J4K 3N7")
                ]),
                new("Félix", "Cyr", new DateTime(1990, 06, 12), [
                    new(501, "Rue Joliette", "Longueuil", "J4H 2G9")
                ])
            ]),
        new(
            UserName: "jsimard",
            Password: "User123!A",
            RoleName: AppRole.UserName,
            Contacts: [
                new("Élodie", "Martin", new DateTime(1985, 12, 01), [
                    new(74, "Rue Racine", "Québec", "G2B 1E3")
                ]),
                new("Gabriel", "Roy", new DateTime(1993, 10, 18), [
                    new(415, "Boulevard René-Lévesque", "Québec", "G1R 2B6"),
                    new(10, "Rue Saint-Jean", "Québec", "G1R 1N7")
                ]),
                new("Rosalie", "Dubé", new DateTime(1997, 05, 24), [
                    new(208, "Avenue Cartier", "Québec", "G1R 2S8"),
                    new(960, "Chemin Sainte-Foy", "Québec", "G1S 2L9")
                ]),
                new("Victor", "Nadeau", new DateTime(1989, 08, 30), [
                    new(301, "Rue Saint-Paul", "Québec", "G1K 3W2"),
                    new(44, "Rue Couillard", "Québec", "G1R 3T5"),
                    new(777, "Boulevard Laurier", "Québec", "G1V 4M6")
                ])
            ]),
        new(
            UserName: "agrenier",
            Password: "User123!A",
            RoleName: AppRole.UserName,
            Contacts: [
                new("Maude", "Pelletier", new DateTime(1994, 02, 14), [
                    new(55, "Rue Wellington", "Sherbrooke", "J1H 5E1")
                ]),
                new("Nathan", "Lavoie", new DateTime(1986, 09, 05), [
                    new(622, "Boulevard Bourque", "Sherbrooke", "J1N 1H3"),
                    new(9, "Rue King Ouest", "Sherbrooke", "J1H 1P1")
                ]),
                new("Clara", "Bernier", new DateTime(1998, 04, 26), [
                    new(410, "Rue Belvédère Sud", "Sherbrooke", "J1H 4C7"),
                    new(17, "Rue Marquette", "Sherbrooke", "J1H 1L5")
                ]),
                new("Julien", "Morin", new DateTime(1991, 11, 19), [
                    new(1001, "Boulevard de Portland", "Sherbrooke", "J1H 5H9")
                ])
            ])
    ];

    public static async Task SeedAsync(IServiceProvider services, CancellationToken cancellationToken = default) {
        var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
        var userManager = services.GetRequiredService<UserManager<AppUser>>();
        var context = services.GetRequiredService<ContactManagerContext>();

        await ensureRolesAsync(roleManager);

        foreach (var seedUser in SeedUsers) {
            var user = await ensureUserAsync(userManager, seedUser.UserName, seedUser.Password);
            await ensureRoleMembershipAsync(userManager, user, seedUser.RoleName);
            await ensureContactsAsync(context, user.Id, seedUser.Contacts, cancellationToken);
        }
    }

    private static async Task ensureRolesAsync(RoleManager<AppRole> roleManager) {
        await ensureRoleAsync(roleManager, AppRole.AdministratorName);
        await ensureRoleAsync(roleManager, AppRole.UserName);
    }

    private static async Task ensureRoleAsync(RoleManager<AppRole> roleManager, string roleName) {
        if (await roleManager.FindByNameAsync(roleName) is not null) {
            return;
        }

        var createResult = await roleManager.CreateAsync(new AppRole {
            Name = roleName,
        });

        if (!createResult.Succeeded) {
            throw new InvalidOperationException($"Unable to seed role '{roleName}': {string.Join("; ", createResult.Errors.Select(error => error.Description))}");
        }
    }

    private static async Task<AppUser> ensureUserAsync(UserManager<AppUser> userManager, string userName, string password) {
        var existingUser = await userManager.FindByNameAsync(userName);
        if (existingUser is not null) {
            return existingUser;
        }

        var userToCreate = AppUser.Create(userName);
        var createResult = await userManager.CreateAsync(userToCreate, password);

        if (!createResult.Succeeded) {
            throw new InvalidOperationException($"Unable to seed user '{userName}': {formatErrors(createResult.Errors)}");
        }

        return userToCreate;
    }

    private static async Task ensureRoleMembershipAsync(UserManager<AppUser> userManager, AppUser user, string roleName) {
        if (await userManager.IsInRoleAsync(user, roleName)) {
            return;
        }

        var addResult = await userManager.AddToRoleAsync(user, roleName);
        if (!addResult.Succeeded) {
            throw new InvalidOperationException($"Unable to assign role '{roleName}' to user '{user.UserName}': {formatErrors(addResult.Errors)}");
        }
    }

    private static async Task ensureContactsAsync(
        ContactManagerContext context,
        Guid ownerId,
        IReadOnlyList<SeedContact> contacts,
        CancellationToken cancellationToken) {
        var existingContacts = await context.Contacts
            .Include(contact => contact.Addresses)
            .Where(contact => contact.OwnerId == ownerId)
            .ToListAsync(cancellationToken);

        foreach (var contactSeed in contacts) {
            var contact = existingContacts.FirstOrDefault(existingContact =>
                existingContact.FirstName == contactSeed.FirstName
                && existingContact.LastName == contactSeed.LastName
                && existingContact.DateOfBirth.Date == contactSeed.DateOfBirth.Date);

            if (contact is null) {
                contact = Contact.Create(ownerId, contactSeed.FirstName, contactSeed.LastName, contactSeed.DateOfBirth);

                foreach (var addressSeed in contactSeed.Addresses) {
                    contact.Addresses.Add(Address.Create(
                        contact.Id,
                        addressSeed.StreetNumber,
                        addressSeed.StreetName,
                        addressSeed.CityName,
                        addressSeed.PostalCode));
                }

                context.Contacts.Add(contact);
                existingContacts.Add(contact);
                continue;
            }

            foreach (var addressSeed in contactSeed.Addresses) {
                var addressExists = contact.Addresses.Any(existingAddress =>
                    existingAddress.StreetNumber == addressSeed.StreetNumber
                    && existingAddress.StreetName == addressSeed.StreetName
                    && existingAddress.CityName == addressSeed.CityName
                    && existingAddress.PostalCode == addressSeed.PostalCode);

                if (addressExists) {
                    continue;
                }

                context.Addresses.Add(Address.Create(
                    contact.Id,
                    addressSeed.StreetNumber,
                    addressSeed.StreetName,
                    addressSeed.CityName,
                    addressSeed.PostalCode));
            }
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    private static string formatErrors(IEnumerable<IdentityError> errors) =>
        string.Join("; ", errors.Select(error => error.Description));

    private sealed record SeedUser(
        string UserName,
        string Password,
        string RoleName,
        IReadOnlyList<SeedContact> Contacts);

    private sealed record SeedContact(
        string FirstName,
        string LastName,
        DateTime DateOfBirth,
        IReadOnlyList<SeedAddress> Addresses);

    private sealed record SeedAddress(
        int StreetNumber,
        string StreetName,
        string CityName,
        string PostalCode);
}