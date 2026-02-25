namespace ContactManager.Core.Domain.Entities;

public class Address : BaseEntity {
    // ===== Propriétés métier =====
    public int StreetNumber { get; private set; }
    public string StreetName { get; private set; }
    public string CityName { get; private set; }
    public string PostalCode { get; private set; }

    // ===== Propriétés de navigation =====
    public Guid ContactId { get; private set; }

    public virtual Contact? Contact { get; private set; }

    // ===== Constructeurs (EF Core) =====
    protected Address() { }

    // ===== Méthodes métier =====
    public static Address Create(Guid contactId, int streetNumber, string streetName, string cityName, string postalCode) {
        ArgumentOutOfRangeException.ThrowIfEqual(contactId, Guid.Empty);
        ArgumentOutOfRangeException.ThrowIfLessThan(streetNumber, 1);
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

    public void Update(int streetNumber, string streetName, string cityName, string postalCode) {
        ArgumentOutOfRangeException.ThrowIfLessThan(streetNumber, 1);
        ArgumentException.ThrowIfNullOrWhiteSpace(streetName);
        ArgumentException.ThrowIfNullOrWhiteSpace(cityName);
        ArgumentException.ThrowIfNullOrWhiteSpace(postalCode);

        StreetNumber = streetNumber;
        StreetName = streetName;
        CityName = cityName;
        PostalCode = postalCode;

        Update();
    }
}
