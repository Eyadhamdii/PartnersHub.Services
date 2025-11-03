using MediatR;
using Microsoft.AspNetCore.Mvc;
using PartnersHub.Synergy.Apis.Controllers.Base;
using PartnersHub.Synergy.Application.Models;
using PartnersHub.Synergy.Application.Opportunities.Commands;
using PartnersHub.Synergy.Application.Opportunities.DTOs;
using PartnersHub.Synergy.Application.Opportunities.Queries;
using PartnersHub.Synergy.Application.Opportunity.Queries;
using PartnersHub.Synergy.Application.SynergyCompany.Queries;
using PartnersHub.Synergy.Domain.Common;
using System.Net;
using System.Runtime.CompilerServices;

namespace PartnersHub.Synergy.Apis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OpportunityController : ApiBaseController<OpportunityController>
    {
        private readonly IMediator _mediator;

        public OpportunityController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Search opportunities with pagination, search, and filtering
        /// </summary>
        [HttpGet("search")]
        [ProducesResponseType(typeof(Result<SearchOpportunitiesResultDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<Result<SearchOpportunitiesResultDto>>> SearchOpportunities(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 12,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? sectorIds = null,
            [FromQuery] string? opportunityTypeIds = null,
            [FromQuery] string? thematicAreaIds = null,
            [FromQuery] string? collaborationRequirementIds = null,
            [FromQuery] string? expectedOutcomeIds = null,
            [FromQuery] string? status = null,
            [FromQuery] string? sortBy = "CreatedAt",
            [FromQuery] bool sortDescending = true)
        {
            var query = new SearchOpportunitiesQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SearchTerm = searchTerm,
                SectorIds = ParseGuids(sectorIds),
                OpportunityTypeIds = ParseInts(opportunityTypeIds),
                ThematicAreaIds = ParseInts(thematicAreaIds),
                CollaborationRequirementIds = ParseInts(collaborationRequirementIds),
                ExpectedOutcomeIds = ParseInts(expectedOutcomeIds),
                Status = status,
                SortBy = sortBy,
                SortDescending = sortDescending
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<Guid>>> SaveRequestAsDraft([FromBody] CreateOpportunityCommand command)
        {
            var requestId = await _mediator.Send(command);
            return Ok(requestId);
        }

        [HttpGet("details/{Id}")]
        public async Task<ActionResult<Result<OpportunityResponseDto>>> GetOpportunityById(Guid Id)
        {
            var query = new GetOpportunityDetailsQuery { Id = Id };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("companies/{companyId}")]
        public async Task<ActionResult<Result<List<OpportunityResponseDto>>>> GetOpportunityByCompanyId(Guid companyId)
        {
            var query = new GetOpportunitiesByCompanyIdQuery { CompanyId = companyId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPut("approve-by-assest-manager/{opportunityId}")]
        public async Task<ActionResult<Result>> ApproveOpportunityByAssestManager(Guid opportunityId)
        {
            var command = new ApproveOpportunityByAssestManagerCommand()
            {
                OpportunityId = opportunityId
            };
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("reject-by-assest-manager/{opportunityId}")]
        public async Task<ActionResult<Result>> RejectOpportunityByAssestManager(RejectOpportunityByAssestManagerCommand request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpPut("approve-by-synergy-admin/{opportunityId}")]
        public async Task<ActionResult<Result>> ApproveOpportunityBySynergyAdmin(Guid opportunityId)
        {
            var command = new ApproveOpportunityBySynergyAdminCommand()
            {
                OpportunityId = opportunityId
            };
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("reject-by-synergy-admin/{opportunityId}")]
        public async Task<ActionResult<Result>> RejectOpportunityBySynergyAdmin(RejectOpportunityBySynergyAdminCommand request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        #region Helper Methods

        private List<Guid>? ParseGuids(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            return value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(s => Guid.TryParse(s, out var guid) ? guid : Guid.Empty)
                .Where(g => g != Guid.Empty)
                .ToList();
        }

        private List<int>? ParseInts(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            return value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(s => int.TryParse(s, out var num) ? num : (int?)null)
                .Where(n => n.HasValue)
                .Select(n => n!.Value)
                .ToList();
        }

        #endregion
    }
}
