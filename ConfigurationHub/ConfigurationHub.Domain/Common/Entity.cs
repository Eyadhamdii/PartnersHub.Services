namespace PartnersHub.ConfigurationHub.Domain.Common;

public abstract class Entity {
    public Guid Id { get; protected set; }

    protected Entity() {
        Id = Guid.NewGuid();
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