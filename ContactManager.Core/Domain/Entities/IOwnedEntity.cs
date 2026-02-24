namespace ContactManager.Core.Domain.Entities;

public interface IOwnedEntity {
    Guid Id { get; }
    Guid OwnerId { get; }
}
