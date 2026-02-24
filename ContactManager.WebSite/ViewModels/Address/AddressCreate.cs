using ContactManager.Core.Domain.Validators;

using FluentValidation;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace ContactManager.WebSite.ViewModels.Address;

public class AddressCreate : IAddressInput {
    [HiddenInput(DisplayValue = false)]
    [Editable(false)]
    public Guid ContactId { get; set; }

    [Display(Name = "Street Number")]
    public int? StreetNumber { get; set; }

    [Display(Name = "Street Name")]
    public string? StreetName { get; set; }

    [Display(Name = "City")]
    public string? CityName { get; set; }

    [Display(Name = "Postal Code")]
    public string? PostalCode { get; set; }

    public class Validator : AbstractValidator<AddressCreate> {
        public Validator() {
            this.ApplyAddressRules();
        }
    }
}
