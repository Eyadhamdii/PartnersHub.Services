using PartnersHub.Synergy.Domain.Common;

namespace PartnersHub.Synergy.Domain.Events;

public class CompanyCreatedEvent : DomainEvent
{
    public Guid CompanyId { get; }
    public string CompanyName { get; }
    public Guid CreatedBy { get; }

    public CompanyCreatedEvent(Guid companyId, string companyName, Guid createdBy)
    {
        CompanyId = companyId;
        CompanyName = companyName;
        CreatedBy = createdBy;
    }
}

public class CompanyUpdatedEvent : DomainEvent
{
    public Guid CompanyId { get; }
    public Guid UpdatedBy { get; }

    public CompanyUpdatedEvent(Guid companyId, Guid updatedBy)
    {
        CompanyId = companyId;
        UpdatedBy = updatedBy;
    }
}
