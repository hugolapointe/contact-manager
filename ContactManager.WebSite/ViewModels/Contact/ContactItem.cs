using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace ContactManager.WebSite.ViewModels.Contact;

public class ContactItem {
    [HiddenInput(DisplayValue = false)]
    [ScaffoldColumn(false)]
    public Guid Id { get; set; }

    [Display(Name = "First Name")]
    public string FirstName { get; set; } = string.Empty;

    [Display(Name = "Last Name")]
    public string LastName { get; set; } = string.Empty;

    [Display(Name = "Age")]
    public int Age { get; set; }
}
