namespace ContactManager.Core.Domain.Entities;

public interface IAudible {
    DateTime CreatedAt { get; }
    DateTime UpdatedAt { get; }
}
