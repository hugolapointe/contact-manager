using System.ComponentModel.DataAnnotations;

namespace ContactManager.WebSite.ViewModels.Contact;

public class ContactItem {
    [Display(Name = "Id")]
    public Guid Id { get; set; }

    [Display(Name = "First Name")]
    public string FirstName { get; set; }

    [Display(Name = "Last Name")]
    public string LastName { get; set; }

    [Display(Name = "Age")]
    public int Age { get; set; }
}
