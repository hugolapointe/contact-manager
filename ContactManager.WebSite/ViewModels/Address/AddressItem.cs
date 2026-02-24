using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace ContactManager.WebSite.ViewModels.Address;

public class AddressItem {
    [HiddenInput(DisplayValue = false)]
    [ScaffoldColumn(false)]
    public Guid Id { get; set; }

    [Display(Name = "Street Number")]
    public int StreetNumber { get; set; }

    [Display(Name = "Street Name")]
    public string StreetName { get; set; } = string.Empty;

    [Display(Name = "City")]
    public string City { get; set; } = string.Empty;

    [Display(Name = "Postal Code")]
    public string PostalCode { get; set; } = string.Empty;
}
