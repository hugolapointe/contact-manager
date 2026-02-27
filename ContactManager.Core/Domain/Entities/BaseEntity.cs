namespace ContactManager.Core.Domain.Entities;

public abstract class BaseEntity : IAudible {
    public Guid Id { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    public DateTime UpdatedAt { get; protected set; }

    protected BaseEntity() {
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    // Met à jour le timestamp de dernière modification.
    protected void Touch() {
        UpdatedAt = DateTime.UtcNow;
    }
}
