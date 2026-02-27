using ContactManager.Core.Domain.Validators;

using FluentValidation;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace ContactManager.WebSite.ViewModels.Address;

public class AddressEdit {
    // Utilisé par la vue pour générer les liens et l'action du formulaire.
    // Les valeurs proviennent de la route, pas du formulaire (voir [FromRoute] dans le controller).
    public Guid AddressId { get; set; }
    public Guid ContactId { get; set; }

    [Display(Name = "Street Number")]
    public int? StreetNumber { get; set; }

    [Display(Name = "Street Name")]
    public string? StreetName { get; set; }

    [Display(Name = "City")]
    public string? CityName { get; set; }

    [Display(Name = "Postal Code")]
    public string? PostalCode { get; set; }

    public class Validator : AbstractValidator<AddressEdit> {
        public Validator() {
            RuleFor(vm => vm.StreetNumber)
                .SetValidator(new StreetNumberValidator());

            RuleFor(vm => vm.StreetName)
                .SetValidator(new StreetNameValidator());

            RuleFor(vm => vm.CityName)
                .SetValidator(new CityNameValidator());

            RuleFor(vm => vm.PostalCode)
                .SetValidator(new PostalCodeValidator());
        }
    }
}
