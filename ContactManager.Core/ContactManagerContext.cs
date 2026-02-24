using ContactManager.Core.Data;
using ContactManager.Core.Domain.Entities;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ContactManager.Core;

public class ContactManagerContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid> {

    public DbSet<Contact> Contacts { get; set; }
    public DbSet<Address> Addresses { get; set; }

    public ContactManagerContext(
        DbContextOptions<ContactManagerContext> options) :
        base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<AppUser>()
            .HasMany(user => user.Contacts)
            .WithOne(contact => contact.Owner)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Contact>()
            .HasMany(contact => contact.Addresses)
            .WithOne(address => address.Contact)
            .OnDelete(DeleteBehavior.Cascade);

        base.OnModelCreating(modelBuilder);
        modelBuilder.Seed();
    }
}
