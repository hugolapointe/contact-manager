using System.ComponentModel.DataAnnotations;

namespace ContactManager.WebSite.ViewModels.Address;

public class AddressList {
    public Guid ContactId { get; set; }

    [Display(Name = "Contact")]
    public string ContactFullName { get; set; } = string.Empty;

    public IEnumerable<AddressItem> Addresses { get; set; } = [];
}
