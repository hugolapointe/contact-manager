namespace ContactManager.Core.Domain.Entities;

public interface IOwnedEntity {
    Guid OwnerId { get; }
}
