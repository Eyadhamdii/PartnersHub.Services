namespace PartnersHub.InnovationHub.Domain.Common;

/// <summary>
/// Base class for all entities in the domain.
/// Entities have a unique identifier and identity equality.
/// </summary>
public abstract class Entity : IEquatable<Entity>
{
    /// <summary>
    /// Gets the unique identifier for this entity.
    /// </summary>
    public Guid Id { get; protected set; }

    /// <summary>
    /// Initializes a new instance with a new Guid.
    /// </summary>
    protected Entity()
    {
        Id = Guid.NewGuid();
    }

    /// <summary>
    /// Initializes a new instance with a specific Guid (for reconstitution from storage).
    /// </summary>
    /// <param name="id">The entity identifier.</param>
    protected Entity(Guid id)
    {
        Id = id;
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current entity.
    /// Entities are equal if they have the same ID and are of the same type.
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (obj is null || obj is not Entity other || GetType() != obj.GetType())
            return false;

        return Id == other.Id;
    }

    /// <summary>
    /// Determines whether the specified entity is equal to the current entity.
    /// </summary>
    public bool Equals(Entity? other)
    {
        if (other is null || GetType() != other.GetType())
            return false;

        return Id == other.Id;
    }

    /// <summary>
    /// Returns the hash code for this entity based on its ID.
    /// </summary>
    public override int GetHashCode() => Id.GetHashCode();

    /// <summary>
    /// Determines whether two entities are equal.
    /// </summary>
    public static bool operator ==(Entity? left, Entity? right)
    {
        if (left is null && right is null)
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    /// <summary>
    /// Determines whether two entities are not equal.
    /// </summary>
    public static bool operator !=(Entity? left, Entity? right) => !(left == right);
}
