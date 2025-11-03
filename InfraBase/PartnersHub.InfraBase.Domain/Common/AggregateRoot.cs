namespace PartnersHub.InfraBase.Domain.Common;

/// <summary>
/// Base class for all aggregate roots with domain events and audit tracking
/// </summary>
public abstract class AggregateRoot : Entity {
    private readonly List<DomainEvent> _domainEvents = new();

    // Audit Properties - Common to all aggregates
    public Guid CreatedBy { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    public Guid? UpdatedBy { get; protected set; }
    public DateTime? UpdatedAt { get; protected set; }

    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(DomainEvent domainEvent) {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents() {
        _domainEvents.Clear();
    }
}