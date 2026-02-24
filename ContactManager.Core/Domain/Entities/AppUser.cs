using Microsoft.AspNetCore.Identity;

namespace ContactManager.Core.Domain.Entities;

public class AppUser : IdentityUser<Guid> {
    public virtual ICollection<Contact> Contacts { get; } = [];

    protected AppUser() : base() { }

    protected AppUser(string userName) : base(userName) { }

    public static AppUser Create(string userName) {
        ArgumentException.ThrowIfNullOrWhiteSpace(userName);
        return new AppUser(userName);
    }
}
