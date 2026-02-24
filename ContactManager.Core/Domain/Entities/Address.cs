namespace ContactManager.Core.Domain.Entities;

public class Address : BaseEntity {
    public int StreetNumber { get; private set; }
    public string StreetName { get; private set; }
    public string CityName { get; private set; }
    public string PostalCode { get; private set; }

    public Guid ContactId { get; private set; }

    public virtual Contact? Contact { get; private set; }

    protected Address() { }

    public static Address CreateForContact(Guid contactId, int streetNumber, string streetName, string cityName, string postalCode) {
        if (contactId == Guid.Empty) throw new ArgumentException("Contact ID cannot be empty.", nameof(contactId));
        if (streetNumber <= 0) throw new ArgumentOutOfRangeException(nameof(streetNumber));
        ArgumentException.ThrowIfNullOrWhiteSpace(streetName);
        ArgumentException.ThrowIfNullOrWhiteSpace(cityName);
        ArgumentException.ThrowIfNullOrWhiteSpace(postalCode);

        return new Address {
            Id = Guid.NewGuid(),
            ContactId = contactId,
            StreetNumber = streetNumber,
            StreetName = streetName,
            CityName = cityName,
            PostalCode = postalCode
        };
    }

    public static Address CreateDefault(int streetNumber, string streetName, string cityName, string postalCode) {
        if (streetNumber <= 0) throw new ArgumentOutOfRangeException(nameof(streetNumber));
        ArgumentException.ThrowIfNullOrWhiteSpace(streetName);
        ArgumentException.ThrowIfNullOrWhiteSpace(cityName);
        ArgumentException.ThrowIfNullOrWhiteSpace(postalCode);

        return new Address {
            Id = Guid.NewGuid(),
            StreetNumber = streetNumber,
            StreetName = streetName,
            CityName = cityName,
            PostalCode = postalCode
        };
    }

    public void Update(int streetNumber, string streetName, string cityName, string postalCode) {
        if (streetNumber <= 0) throw new ArgumentOutOfRangeException(nameof(streetNumber));
        ArgumentException.ThrowIfNullOrWhiteSpace(streetName);
        ArgumentException.ThrowIfNullOrWhiteSpace(cityName);
        ArgumentException.ThrowIfNullOrWhiteSpace(postalCode);

        StreetNumber = streetNumber;
        StreetName = streetName;
        CityName = cityName;
        PostalCode = postalCode;
    }
}
