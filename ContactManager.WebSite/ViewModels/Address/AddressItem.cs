using System.ComponentModel.DataAnnotations;

namespace ContactManager.WebSite.ViewModels.Address;

public class AddressItem {
    [Display(Name = "Id")]
    public Guid Id { get; set; }

    [Display(Name = "Street Number")]
    public int StreetNumber { get; set; }

    [Display(Name = "Street Name")]
    public string StreetName { get; set; }

    [Display(Name = "City")]
    public string City { get; set; }

    [Display(Name = "Postal Code")]
    public string PostalCode { get; set; }
}
