using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace ContactManager.WebSite.ViewModels.User;

public class UserItem {
    [HiddenInput(DisplayValue = false)]
    [ScaffoldColumn(false)]
    public Guid Id { get; set; }

    [Display(Name = "Username")]
    public string UserName { get; set; } = string.Empty;

    [Display(Name = "Role")]
    public string RoleName { get; set; } = string.Empty;
}
