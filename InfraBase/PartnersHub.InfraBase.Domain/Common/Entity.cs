namespace PartnersHub.InfraBase.Domain.Common;

public abstract class Entity {
    public Guid Id { get; protected set; }

    protected Entity() {
        Id = Guid.NewGuid();
    }

    protected Entity(Guid id) {
        Id = id;
    }

    public override bool Equals(object? obj) {
        if (obj is null || obj is not Entity other)
            return false;

        return Id == other.Id;
    }

    public override int GetHashCode() {
        return Id.GetHashCode();
    }
}