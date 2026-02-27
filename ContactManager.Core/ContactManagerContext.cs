using ContactManager.Core.Domain.Entities;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ContactManager.Core;

public class ContactManagerContext : IdentityDbContext<AppUser, AppRole, Guid>
{

    public DbSet<Contact> Contacts { get; set; }
    public DbSet<Address> Addresses { get; set; }

    public ContactManagerContext(
        DbContextOptions<ContactManagerContext> options) :
        base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Optimise la recherche de contacts d'un utilisateur par ordre alphabétique
        modelBuilder.Entity<Contact>()
            .HasIndex(contact => new { contact.OwnerId, contact.FirstName, contact.LastName });

        // AppUser.Roles (ICollection<AppRole>) est une many-to-many via la table AspNetUserRoles.
        // Sans cette configuration explicite, EF Core crée un FK implicite AppUserId sur AspNetRoles.
        modelBuilder.Entity<AppUser>()
            .HasMany(user => user.Roles)
            .WithMany()
            .UsingEntity<IdentityUserRole<Guid>>(
                join => join.HasOne<AppRole>().WithMany().HasForeignKey(ur => ur.RoleId),
                join => join.HasOne<AppUser>().WithMany().HasForeignKey(ur => ur.UserId),
                join => {
                    join.HasKey(ur => new { ur.UserId, ur.RoleId });
                    join.ToTable("AspNetUserRoles");
                });
    }
}
