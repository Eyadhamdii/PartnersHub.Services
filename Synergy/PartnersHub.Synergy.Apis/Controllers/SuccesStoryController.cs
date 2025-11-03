using MediatR;
using Microsoft.AspNetCore.Mvc;
using PartnersHub.Synergy.Apis.Controllers.Base;
using PartnersHub.Synergy.Application.SuccessStories.Commands;
using PartnersHub.Synergy.Application.SuccessStories.DTOs;
using PartnersHub.Synergy.Application.SuccessStories.Queries;
using PartnersHub.Synergy.Domain.Common;

namespace PartnersHub.Synergy.Apis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuccesStoryController : ApiBaseController<SuccesStoryController>
    {
        private readonly IMediator _mediator;
        public SuccesStoryController(IMediator mediator)
        {
            _mediator = mediator;

        }
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<Guid>>> SaveRequestAsDraft([FromBody] CreateSuccessStoryCommand command)
        {
            var requestId = await _mediator.Send(command);



            return Ok(requestId);
        }
        [HttpGet("{Id}")]
        [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<Guid>>> GetById( Guid Id)
        {
            var query = new GetSuccessStoryByIdQuery { Id = Id };
            var result = await _mediator.Send(query);



            return Ok(result);
        }
        [HttpGet("search")]
        public async Task<ActionResult<Result<SuccessStorySearchResponseDto>>> SearchSuccessStories(
        [FromQuery] Guid? companyId = null,
        [FromQuery] int? collaborationType = null,
        [FromQuery] List<Guid>? sectorIds = null,
        [FromQuery] SuccessStoryStatus? status = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] string? partnerCompanyName = null,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = true,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
        {
            var query = new SearchSuccessStoriesQuery
            {
                CompanyId = companyId,
                CollaborationType = collaborationType,
                SectorIds = sectorIds,
                Status = status,
                StartDate = startDate,
                EndDate = endDate,
                PartnerCompanyName = partnerCompanyName,
                SearchTerm = searchTerm,
                SortBy = sortBy,
                SortDescending = sortDescending,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);

            return Ok(result);
        }
    }
}
