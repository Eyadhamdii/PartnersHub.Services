using PartnersHub.InfraBase.Domain.Common;

namespace PartnersHub.InfraBase.Domain.Events;

public class RequestAcceptedByPcAdminEvent : DomainEvent {
    public Guid RequestId { get; }
    public string RequestCode { get; }
    public Guid AcceptedBy { get; }

    public RequestAcceptedByPcAdminEvent(Guid requestId, string requestCode, Guid acceptedBy) {
        RequestId = requestId;
        RequestCode = requestCode;
        AcceptedBy = acceptedBy;
    }
}