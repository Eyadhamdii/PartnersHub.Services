namespace PartnersHub.ConfigurationHub.Domain.Common;

/// <summary>
/// Base class for aggregate roots with domain events and audit tracking
/// </summary>
public abstract class AggregateRoot : Entity {
    private readonly List<DomainEvent> _domainEvents = new();

    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    // Audit fields - common to all aggregates
    public Guid CreatedBy { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    public Guid? UpdatedBy { get; protected set; }
    public DateTime? UpdatedAt { get; protected set; }

    protected AggregateRoot() {
        CreatedAt = DateTime.UtcNow;
    }

    protected void AddDomainEvent(DomainEvent domainEvent) {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents() {
        _domainEvents.Clear();
    }

    protected void MarkAsCreated(Guid userId) {
        CreatedBy = userId;
        CreatedAt = DateTime.UtcNow;
    }

    protected void MarkAsUpdated(Guid userId) {
        UpdatedBy = userId;
        UpdatedAt = DateTime.UtcNow;
    }
}