using Microsoft.AspNetCore.Identity;

namespace ContactManager.Core.Domain.Entities;

public class AppUser : IdentityUser<Guid> {
    // ===== Propriétés de navigation =====
    public virtual ICollection<Contact> Contacts { get; } = [];

    // ===== Constructeurs (EF Core) =====
    protected AppUser() : base() { }

    protected AppUser(string userName) : base(userName) { }

    // ===== Méthodes métier =====
    public static AppUser CreateForUserName(string userName) {
        ArgumentException.ThrowIfNullOrWhiteSpace(userName);
        return new AppUser(userName);
    }
}
