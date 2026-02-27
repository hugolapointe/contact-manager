using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace ContactManager.WebSite.ViewModels.Contact;

public class ContactItem {
    [HiddenInput(DisplayValue = false)]
    [ScaffoldColumn(false)]
    public Guid Id { get; set; }

    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [Display(Name = "Age")]
    public int Age { get; set; }

    [Display(Name = "Created At")]
    public DateTime CreatedAt { get; set; }

    [Display(Name = "Updated At")]
    public DateTime UpdatedAt { get; set; }
}
