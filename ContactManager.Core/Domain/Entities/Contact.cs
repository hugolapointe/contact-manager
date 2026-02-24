using ContactManager.Core.Domain.Guards;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContactManager.Core.Domain.Entities;

public class Contact : BaseEntity, IOwnedEntity {
    // ===== Propriétés métier =====
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public DateTime DateOfBirth { get; private set; }

    // ===== Propriétés calculées =====
    [NotMapped]
    public int Age {
        get {
            var today = DateTime.Today;
            var years = today.Year - DateOfBirth.Year;
            return years - (today < DateOfBirth.AddYears(years) ? 1 : 0);
        }
    }

    [NotMapped]
    public string FullName => $"{FirstName} {LastName}";

    // ===== Propriétés de navigation =====
    public Guid OwnerId { get; private set; }
    public virtual AppUser? Owner { get; private set; }

    public virtual ICollection<Address> Addresses { get; } = [];

    // ===== Constructeurs (EF Core) =====
    protected Contact() { }

    // ===== Méthodes métier =====
    public static Contact Create(Guid ownerId, string firstName, string lastName, DateTime dateOfBirth) {
        Guard.AgainstEmptyGuid(ownerId, nameof(ownerId), "Owner ID cannot be empty.");
        ArgumentException.ThrowIfNullOrWhiteSpace(firstName);
        ArgumentException.ThrowIfNullOrWhiteSpace(lastName);
        Guard.AgainstInvalidBirthDate(dateOfBirth, nameof(dateOfBirth));

        return new Contact {
            Id = Guid.NewGuid(),
            OwnerId = ownerId,
            FirstName = firstName,
            LastName = lastName,
            DateOfBirth = dateOfBirth
        };
    }

    public void Update(string firstName, string lastName, DateTime dateOfBirth) {
        ArgumentException.ThrowIfNullOrWhiteSpace(firstName);
        ArgumentException.ThrowIfNullOrWhiteSpace(lastName);
        Guard.AgainstInvalidBirthDate(dateOfBirth, nameof(dateOfBirth));

        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
    }
}
