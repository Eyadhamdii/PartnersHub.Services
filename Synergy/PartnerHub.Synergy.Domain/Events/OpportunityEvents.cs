using PartnersHub.Synergy.Domain.Common;

namespace PartnersHub.Synergy.Domain.Events;

public class OpportunityCreatedEvent : DomainEvent
{
    public Guid OpportunityId { get; }
    public Guid CompanyId { get; }
    public string Title { get; }
    public Guid CreatedBy { get; }

    public OpportunityCreatedEvent(Guid opportunityId, Guid companyId, string title, Guid createdBy)
    {
        OpportunityId = opportunityId;
        CompanyId = companyId;
        Title = title;
        CreatedBy = createdBy;
    }
}

public class OpportunitySubmittedEvent : DomainEvent
{
    public Guid OpportunityId { get; }
    public Guid CompanyId { get; }
    public Guid SubmittedBy { get; }

    public OpportunitySubmittedEvent(Guid opportunityId, Guid companyId, Guid submittedBy)
    {
        OpportunityId = opportunityId;
        CompanyId = companyId;
        SubmittedBy = submittedBy;
    }
}

public class OpportunityApprovedEvent : DomainEvent
{
    public Guid OpportunityId { get; }
    public Guid CompanyId { get; }
    public OpportunityStatus NewStatus { get; }
    public Guid ApprovedBy { get; }

    public OpportunityApprovedEvent(Guid opportunityId, Guid companyId, OpportunityStatus newStatus, Guid approvedBy)
    {
        OpportunityId = opportunityId;
        CompanyId = companyId;
        NewStatus = newStatus;
        ApprovedBy = approvedBy;
    }
}

public class OpportunityRejectedEvent : DomainEvent
{
    public Guid OpportunityId { get; }
    public Guid CompanyId { get; }
    public OpportunityStatus NewStatus { get; }
    public string RejectionReason { get; }
    public Guid RejectedBy { get; }

    public OpportunityRejectedEvent(Guid opportunityId, Guid companyId, OpportunityStatus newStatus, string rejectionReason, Guid rejectedBy)
    {
        OpportunityId = opportunityId;
        CompanyId = companyId;
        NewStatus = newStatus;
        RejectionReason = rejectionReason;
        RejectedBy = rejectedBy;
    }
}

public class OpportunityPublishedEvent : DomainEvent
{
    public Guid OpportunityId { get; }
    public Guid CompanyId { get; }
    public Guid PublishedBy { get; }

    public OpportunityPublishedEvent(Guid opportunityId, Guid companyId, Guid publishedBy)
    {
        OpportunityId = opportunityId;
        CompanyId = companyId;
        PublishedBy = publishedBy;
    }
}
