using System.ComponentModel.DataAnnotations.Schema;

namespace ContactManager.Core.Domain.Entities;

public class Contact {
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }

    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required DateTime DateOfBirth { get; set; }

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
    public string FullName => $"{FirstName} {LastName}";

    // Navigation Properties
    [ForeignKey(nameof(OwnerId))]
    public virtual User? Owner { get; set; }
    public List<Address> Addresses { get; } = new();
}
