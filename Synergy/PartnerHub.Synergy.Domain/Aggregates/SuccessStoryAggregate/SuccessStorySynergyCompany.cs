using PartnersHub.Synergy.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartnersHub.Synergy.Domain.Aggregates.SuccessStoryAggregate
{
    public class SuccessStorySynergyCompany : Entity
    {
        public Guid SynergyCompanyId { get; private set; }
        public Guid SuccessStoryId { get; private set; }

        private SuccessStorySynergyCompany() { }

        public SuccessStorySynergyCompany(Guid successStoryId, Guid synergyComapanyId)
        {
            if (synergyComapanyId == Guid.Empty)
                throw new ArgumentException("Synergy Company ID is required", nameof(SynergyCompanyId));

            if (successStoryId == Guid.Empty)
                throw new ArgumentException("Success Story Id ID is required", nameof(successStoryId));

            SynergyCompanyId = synergyComapanyId;
            SuccessStoryId = successStoryId;
        }
    }
}
