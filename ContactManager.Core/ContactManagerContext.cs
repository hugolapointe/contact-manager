using ContactManager.Core.Domain.Entities;

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
        // Optimise la recherche de contacts d'un utilisateur par ordre alphabétique
        modelBuilder.Entity<Contact>()
            .HasIndex(contact => new { contact.OwnerId, contact.FirstName, contact.LastName });

        base.OnModelCreating(modelBuilder);
    }
}
