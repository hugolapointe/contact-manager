namespace ContactManager.Core.Domain.Entities;

public abstract class BaseEntity {
    public Guid Id { get; protected set; }
}
