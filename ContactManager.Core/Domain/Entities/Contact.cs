using System.ComponentModel.DataAnnotations.Schema;

namespace ContactManager.Core.Domain.Entities;

public class Contact : BaseEntity, IOwned {
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
        ArgumentOutOfRangeException.ThrowIfEqual(ownerId, Guid.Empty);
        ArgumentException.ThrowIfNullOrWhiteSpace(firstName);
        ArgumentException.ThrowIfNullOrWhiteSpace(lastName);
        ArgumentOutOfRangeException.ThrowIfEqual(dateOfBirth, default);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(dateOfBirth.Date, DateTime.Today, nameof(dateOfBirth));

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
        ArgumentOutOfRangeException.ThrowIfEqual(dateOfBirth, default);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(dateOfBirth.Date, DateTime.Today, nameof(dateOfBirth));

        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
        
        Update();
    }
}
