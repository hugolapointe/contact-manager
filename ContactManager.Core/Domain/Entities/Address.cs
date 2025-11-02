using System.ComponentModel.DataAnnotations.Schema;

namespace ContactManager.Core.Domain.Entities;

/// <summary>
/// Represents a physical address for a contact.
/// </summary>
public class Address {

    public Guid Id { get; set; }
    public Guid ContactId { get; set; }

    public required int StreetNumber { get; set; }
    public required string StreetName { get; set; }
    public required string CityName { get; set; }
    public required string PostalCode { get; set; }


    // Navigation Properties
    [ForeignKey(nameof(ContactId))]
    public virtual Contact? Contact { get; set; }
}