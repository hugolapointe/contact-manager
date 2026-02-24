namespace ContactManager.Core.Domain.Entities;

public class Contact : BaseEntity, IOwnedEntity {
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public DateTime DateOfBirth { get; private set; }

    public int Age {
        get {
            var today = DateTime.Today;
            var age = today.Year - DateOfBirth.Year;
            if (DateOfBirth.Date > today.AddYears(-age))
                age--;
            return age;
        }
    }
    public string FullName => $"{FirstName} {LastName}";

    public Guid OwnerId { get; private set; }
    public virtual AppUser? Owner { get; private set; }

    public virtual ICollection<Address> Addresses { get; } = [];

    protected Contact() { }

    public static Contact CreateForOwner(Guid ownerId, string firstName, string lastName, DateTime dateOfBirth) {
        if (ownerId == Guid.Empty) throw new ArgumentException("Owner ID cannot be empty.", nameof(ownerId));
        ArgumentException.ThrowIfNullOrWhiteSpace(firstName);
        ArgumentException.ThrowIfNullOrWhiteSpace(lastName);

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

        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
    }
}
