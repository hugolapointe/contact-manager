using System.Collections.Generic;

namespace ContactManager.WebSite.ViewModels.Contact;

public class ContactList
{
    public IEnumerable<ContactItem> Contacts { get; set; } = new List<ContactItem>();
}
