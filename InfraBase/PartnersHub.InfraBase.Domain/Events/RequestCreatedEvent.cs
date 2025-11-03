using PartnersHub.InfraBase.Domain.Common;

namespace PartnersHub.InfraBase.Domain.Events;

public class RequestCreatedEvent : DomainEvent {
    public Guid RequestId { get; }
    public string RequestCode { get; }
    public string ProjectName { get; }
    public Guid CreatedBy { get; }

    public RequestCreatedEvent(Guid requestId, string requestCode, string projectName, Guid createdBy) {
        RequestId = requestId;
        RequestCode = requestCode;
        ProjectName = projectName;
        CreatedBy = createdBy;
    }
}