using MediatR;
using PartnersHub.Synergy.Application.Interfaces.Common;
using PartnersHub.Synergy.Application.Interfaces.Repository;
using PartnersHub.Synergy.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartnersHub.Synergy.Application.Opportunities.Commands
{
    public class ApproveOpportunityBySynergyAdminCommand : IRequest<Result>
    {
        public Guid OpportunityId { get; set; } 
    }
}
