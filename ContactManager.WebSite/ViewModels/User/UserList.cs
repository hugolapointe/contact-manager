using System.Collections.Generic;

namespace ContactManager.WebSite.ViewModels.User;

public class UserList
{
    public IEnumerable<UserItem> Users { get; set; } = new List<UserItem>();
}
