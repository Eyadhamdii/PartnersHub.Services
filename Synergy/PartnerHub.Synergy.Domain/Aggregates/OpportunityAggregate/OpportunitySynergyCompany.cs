using PartnersHub.Synergy.Domain.Common;

namespace PartnersHub.Synergy.Domain.Aggregates.OpportunityAggregate;

public class OpportunitySynergyCompany : Entity
{
    public Guid OpportunityId { get; private set; }
    public Guid SynergyCompanyId { get; private set; }
    public DateTime CollaborationDate { get; private set; }

    private OpportunitySynergyCompany() { }

    public OpportunitySynergyCompany(Guid opportunityId, Guid synergyCompanyId)
    {
        if (opportunityId == Guid.Empty)
            throw new ArgumentException("Opportunity ID is required", nameof(opportunityId));

        if (synergyCompanyId == Guid.Empty)
            throw new ArgumentException("Synergy Company ID is required", nameof(synergyCompanyId));

        OpportunityId = opportunityId;
        SynergyCompanyId = synergyCompanyId;
        CollaborationDate = DateTime.UtcNow;
    }
}
