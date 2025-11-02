using Microsoft.AspNetCore.Identity;

namespace ContactManager.Core.Domain.Entities;

/// <summary>
/// Represents a user in the contact management system.
/// </summary>
public class User : IdentityUser<Guid> {

    /// <summary>
    /// Gets the collection of contacts owned by this user.
    /// </summary>
    public List<Contact> Contacts { get; } = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="User"/> class.
    /// </summary>
    /// <param name="userName">The username for the user.</param>
    public User(string userName) :
        base(userName) { }
}
