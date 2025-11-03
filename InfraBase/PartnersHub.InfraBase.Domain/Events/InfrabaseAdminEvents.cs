using PartnersHub.InfraBase.Domain.Common;

namespace PartnersHub.InfraBase.Domain.Events;

public class RequestApprovedByInfrabaseAdminEvent : DomainEvent {
    public Guid RequestId { get; }
    public string RequestCode { get; }
    public Guid ApprovedBy { get; }

    public RequestApprovedByInfrabaseAdminEvent(Guid requestId, string requestCode, Guid approvedBy) {
        RequestId = requestId;
        RequestCode = requestCode;
        ApprovedBy = approvedBy;
    }
}

public class RequestRejectedByInfrabaseAdminEvent : DomainEvent {
    public Guid RequestId { get; }
    public string RequestCode { get; }
    public string RejectionReason { get; }
    public Guid RejectedBy { get; }

    public RequestRejectedByInfrabaseAdminEvent(Guid requestId, string requestCode, string rejectionReason, Guid rejectedBy) {
        RequestId = requestId;
        RequestCode = requestCode;
        RejectionReason = rejectionReason;
        RejectedBy = rejectedBy;
    }
}

public class RequestChangesRequestedByInfrabaseAdminEvent : DomainEvent {
    public Guid RequestId { get; }
    public string RequestCode { get; }
    public string ChangeDescription { get; }
    public Guid RequestedBy { get; }

    public RequestChangesRequestedByInfrabaseAdminEvent(Guid requestId, string requestCode, string changeDescription, Guid requestedBy) {
        RequestId = requestId;
        RequestCode = requestCode;
        ChangeDescription = changeDescription;
        RequestedBy = requestedBy;
    }
}

public class RequestResubmittedEvent : DomainEvent {
    public Guid RequestId { get; }
    public string RequestCode { get; }
    public string PreviousStatus { get; }
    public string NewStatus { get; }
    public Guid ResubmittedBy { get; }

    public RequestResubmittedEvent(Guid requestId, string requestCode, string previousStatus, string newStatus, Guid resubmittedBy) {
        RequestId = requestId;
        RequestCode = requestCode;
        PreviousStatus = previousStatus;
        NewStatus = newStatus;
        ResubmittedBy = resubmittedBy;
    }
}