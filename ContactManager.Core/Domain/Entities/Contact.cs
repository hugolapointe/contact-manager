using System.ComponentModel.DataAnnotations.Schema;

namespace ContactManager.Core.Domain.Entities;

/// <summary>
/// Represents a contact in the contact management system.
/// </summary>
public class Contact {
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }

    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required DateTime DateOfBirth { get; set; }

    /// <summary>
    /// Gets the calculated age of the contact based on their date of birth.
    /// </summary>
    public int Age {
        get {
            var today = DateTime.Today;
            var age = today.Year - DateOfBirth.Year;
            if (DateOfBirth.Date > today.AddYears(-age)) {
                age--;
            }
            return age;
        }
    }

    /// <summary>
    /// Gets the full name of the contact (FirstName + LastName).
    /// </summary>
    public string FullName => $"{FirstName} {LastName}";

    // Navigation Properties
    [ForeignKey(nameof(OwnerId))]
    public virtual User? Owner { get; set; }
    public List<Address> Addresses { get; } = new();
}
