using PartnersHub.Synergy.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartnersHub.Synergy.Domain.Aggregates.SuccessStoryAggregate
{
    public class SuccessStoryOpportunity : Entity
    {
        public Guid OpportunityId { get; private set; }
        public Guid SuccessStoryId { get; private set; }

        private SuccessStoryOpportunity() { }

        public SuccessStoryOpportunity(Guid successStoryId, Guid opportunityId)
        {
            if (opportunityId == Guid.Empty)
                throw new ArgumentException("Opportunity ID is required", nameof(opportunityId));

            if (successStoryId == Guid.Empty)
                throw new ArgumentException("successStoryId ID is required", nameof(successStoryId));

            OpportunityId = opportunityId;
            SuccessStoryId = successStoryId;
        }
    }
}
