using PartnersHub.InfraBase.Domain.Common;

namespace PartnersHub.InfraBase.Domain.Events;

public class RequestRejectedByPcAdminEvent : DomainEvent {
    public Guid RequestId { get; }
    public string RequestCode { get; }
    public string RejectionReason { get; }
    public Guid RejectedBy { get; }

    public RequestRejectedByPcAdminEvent(Guid requestId, string requestCode, string rejectionReason, Guid rejectedBy) {
        RequestId = requestId;
        RequestCode = requestCode;
        RejectionReason = rejectionReason;
        RejectedBy = rejectedBy;
    }
}