using PartnersHub.InfraBase.Domain.Common;

namespace PartnersHub.InfraBase.Domain.Events;

public class RequestSubmittedEvent : DomainEvent {
    public Guid RequestId { get; }
    public string RequestCode { get; }
    public string Status { get; }
    public Guid SubmittedBy { get; }

    public RequestSubmittedEvent(Guid requestId, string requestCode, string status, Guid submittedBy) {
        RequestId = requestId;
        RequestCode = requestCode;
        Status = status;
        SubmittedBy = submittedBy;
    }
}