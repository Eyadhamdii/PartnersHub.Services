using MediatR;
using PartnersHub.Synergy.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartnersHub.Synergy.Application.Opportunities.Commands
{
    public class ApproveOpportunityByAssestManagerCommand :IRequest<Result>
    {
        public Guid OpportunityId { get; set; }
    }
}
