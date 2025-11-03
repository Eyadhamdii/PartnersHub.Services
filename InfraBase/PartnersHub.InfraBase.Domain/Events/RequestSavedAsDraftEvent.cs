using PartnersHub.InfraBase.Domain.Common;

namespace PartnersHub.InfraBase.Domain.Events;

public class RequestSavedAsDraftEvent : DomainEvent {
    public Guid RequestId { get; }
    public string ProjectName { get; }
    public Guid SavedBy { get; }

    public RequestSavedAsDraftEvent(Guid requestId, string projectName, Guid savedBy) {
        RequestId = requestId;
        ProjectName = projectName;
        SavedBy = savedBy;
    }
}