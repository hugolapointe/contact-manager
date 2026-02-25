namespace ContactManager.Core.Domain.Entities;

public interface IOwned {
    Guid OwnerId { get; }
}
