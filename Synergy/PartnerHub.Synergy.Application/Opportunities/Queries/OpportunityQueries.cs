using MediatR;
using Microsoft.AspNetCore.Http;
using PartnersHub.Synergy.Application.Models;
using PartnersHub.Synergy.Application.Opportunities.DTOs;
using PartnersHub.Synergy.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartnersHub.Synergy.Application.Opportunity.Queries
{
    public class GetOpportunityDetailsQuery : IRequest<Result<OpportunityResponseDto>>
    {
        public Guid Id { get; set; }

    }
    public class GetPaginatedOpportunityDetailsQuery : IRequest<Result<PaginatedList<OpportunityResponseDto>>>
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
       
    }
    public class GetOpportunitiesByCompanyIdQuery : IRequest<Result<List<OpportunityResponseDto>>>
    {
        public Guid CompanyId { get; set; }
    }
}
