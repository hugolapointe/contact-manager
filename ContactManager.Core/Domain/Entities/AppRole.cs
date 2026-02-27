using Microsoft.AspNetCore.Identity;

namespace ContactManager.Core.Domain.Entities;

public sealed class AppRole : IdentityRole<Guid> {
    public const string AdministratorName = "Administrator";
    public const string UserName = "User";

    public AppRole() : base() { }

    public static bool IsSupported(string? roleName) => roleName is AdministratorName or UserName;
}
