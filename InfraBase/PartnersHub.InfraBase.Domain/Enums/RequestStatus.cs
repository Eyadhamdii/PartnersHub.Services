namespace PartnersHub.InfraBase.Domain.Enums;

/// <summary>
/// Represents the status of an infrastructure request throughout its lifecycle
/// </summary>
public enum RequestStatus : byte {
    /// <summary>
    /// Request is being drafted and not yet submitted
    /// </summary>
    Draft = 0,

    /// <summary>
    /// Request submitted by PC Contributor, pending PC Admin approval
    /// </summary>
    PendingPcAdminApproval = 1,

    /// <summary>
    /// Request approved by PC Admin, pending Infrabase Admin approval
    /// </summary>
    PendingPcInfrabaseApproval = 2,

    /// <summary>
    /// Request rejected by PC Admin
    /// </summary>
    RejectedByPcAdmin = 4,

    /// <summary>
    /// Infrabase Admin requested changes to the request
    /// </summary>
    ChangeRequested = 7,

    /// <summary>
    /// Request approved by Infrabase Admin (Final approval)
    /// </summary>
    InfrabaseApproved = 8,

    /// <summary>
    /// Request rejected by Infrabase Admin (Final rejection)
    /// </summary>
    InfrabaseRejected = 9
}