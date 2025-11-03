using MediatR;

namespace PartnersHub.InfraBase.Domain.Common;

public abstract class DomainEvent : INotification {
    protected DomainEvent() {
        EventId = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
    }

    public Guid EventId { get; }
    public DateTime OccurredOn { get; }
}