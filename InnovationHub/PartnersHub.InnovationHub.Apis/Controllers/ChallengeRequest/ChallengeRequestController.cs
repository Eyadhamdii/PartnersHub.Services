using MediatR;
using Microsoft.AspNetCore.Mvc;
using PartnersHub.InnovationHub.Apis.Controllers.Base;
using PartnersHub.InnovationHub.Application.Challenge.Commands.ChallengeRequest;
using PartnersHub.InnovationHub.Application.Challenge.Commands.LinkTechnologyToChallenge;
using PartnersHub.InnovationHub.Application.Challenge.Queries.ChallengeRequest;
using PartnersHub.InnovationHub.Application.Challenge.Queries.DTOs;

namespace PartnersHub.InnovationHub.Apis.Controllers.ChallengeRequest
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ChallengeRequestController : ApiBaseController<ChallengeRequestController>
    {

        [HttpPost]
        public Task<ActionResult<ApiResponse>> Create([FromBody] CreateChallengeRequestCommand command, CancellationToken cancellationToken)
             => Execute(command, cancellationToken);

        [HttpPost("link-to-technology")]
        public Task<ActionResult<ApiResponse>> LinkToTechnology(LinkTechnologyToChallengeCommand command, CancellationToken cancellationToken)
         => Execute(command, cancellationToken);

        [HttpPost("link-additional-technology")]
        public Task<ActionResult<ApiResponse>> LinkAdditionalTechnology(LinkAdditionalTechnologyToChallengeCommand command, CancellationToken cancellationToken)
            => Execute(command, cancellationToken);

        [HttpPost("{requestId}/Unarchive")]
        public Task<ActionResult<ApiResponse>> Unarchive(Guid requestId, CancellationToken cancellationToken)
             => Execute(new UnarchiveChallengeRequestCommand { RequestId = requestId }, cancellationToken);


        [HttpDelete("{requestId}/draft")]
        public Task<ActionResult<ApiResponse>> DeleteDraft(Guid requestId, CancellationToken cancellationToken)
              => Execute(new DeleteChallengeRequestDraftCommand { RequestId = requestId }, cancellationToken);


        [HttpPost("List")]
        public Task<ActionResult<ApiResponse>> List(ChallengeRequestListQuery challengeRequestListQuery, CancellationToken cancellationToken)
                => Execute(challengeRequestListQuery, cancellationToken);


        [HttpGet("{requestId}/details")]
        public Task<ActionResult<ApiResponse>> Details(Guid requestId, CancellationToken cancellationToken)
                     => Execute(new ChallengeDetailsQuery { ChallengeId = requestId }, cancellationToken);


        [HttpPost("Review")]
        public Task<ActionResult<ApiResponse>> Review([FromBody] ReviewChallengeRequestCommand reviewChallenge, CancellationToken cancellationToken)
             => Execute(reviewChallenge, cancellationToken);
    }
}
