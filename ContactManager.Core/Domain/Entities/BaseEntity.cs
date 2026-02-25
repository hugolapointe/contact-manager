namespace ContactManager.Core.Domain.Entities;

public abstract class BaseEntity : IAudible {
    public Guid Id { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    public DateTime UpdateAt { get; protected set; }

    protected BaseEntity() {
        CreatedAt = DateTime.UtcNow;

        Update();
    }

    protected virtual void Update() {
        UpdateAt = DateTime.UtcNow;
    }
}
