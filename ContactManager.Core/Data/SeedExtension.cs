using ContactManager.Core.Domain.Entities;
using ContactManager.Core.Domain.Enums;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ContactManager.Core.Data;

public static class SeedExtension {
    private static readonly PasswordHasher<AppUser> passwordHasher = new();

    public static void Seed(this ModelBuilder builder) {
        var adminRole = addRole(Roles.Administrator);
        _ = addRole(Roles.User);
        var hugoUser = addUser("hlapointe", "Admin123!");
        addUserToRole(hugoUser, adminRole);
        addContactWithAddress(
            "Sébastien", "Pouliot", new DateTime(1980, 02, 06),
            3000, "Boulevard Boullé", "Saint-Hyacinthe", "J2S 1H9",
            hugoUser);

        IdentityRole<Guid> addRole(string name) {
            var newRole = new IdentityRole<Guid> {
                Id = Guid.NewGuid(),
                Name = name,
                NormalizedName = name.ToUpper()
            };
            builder.Entity<IdentityRole<Guid>>().HasData(newRole);

            return newRole;
        }

        void addUserToRole(AppUser user, IdentityRole<Guid> role) {
            builder.Entity<IdentityUserRole<Guid>>().HasData(new IdentityUserRole<Guid> {
                UserId = user.Id,
                RoleId = role.Id,
            });
        }

        AppUser addUser(string userName, string password) {
            var newUser = AppUser.Create(userName);
            newUser.Id = Guid.NewGuid();
            newUser.NormalizedUserName = userName.ToUpper();
            newUser.SecurityStamp = Guid.NewGuid().ToString();
            newUser.PasswordHash = passwordHasher.HashPassword(newUser, password);
            builder.Entity<AppUser>().HasData(newUser);

            return newUser;
        }

        void addContactWithAddress(
            string firstName, string lastName, DateTime dob,
            int streetNumber, string streetName, string city, string postalCode,
            AppUser user) {
            var contactId = Guid.NewGuid();

            builder.Entity<Contact>().HasData(new {
                Id = contactId,
                OwnerId = user.Id,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dob,
            });

            builder.Entity<Address>().HasData(new {
                Id = Guid.NewGuid(),
                ContactId = contactId,
                StreetNumber = streetNumber,
                StreetName = streetName,
                CityName = city,
                PostalCode = postalCode,
            });
        }
    }
}
