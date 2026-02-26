using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace ContactManager.WebSite.ViewModels.Address;

public class AddressList {
    [HiddenInput(DisplayValue = false)]
    [Editable(false)]
    public Guid ContactId { get; set; }

    [Display(Name = "Contact")]
    public string ContactFullName { get; set; } = string.Empty;

    public IEnumerable<AddressItem> Addresses { get; set; } = [];
}