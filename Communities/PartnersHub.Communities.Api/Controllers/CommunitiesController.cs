using MediatR;
using Microsoft.AspNetCore.Mvc;
using PartnersHub.Communities.Application.Communities.Commands;
using PartnersHub.Communities.Application.Communities.Queries;
using PartnersHub.Communities.Domain.Aggregates.Community;

namespace PartnersHub.Communities.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommunitiesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CommunitiesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<Community>>> GetCommunities()
    {
        var communities = await _mediator.Send(new GetCommunitiesQuery());
        return Ok(communities);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Community>> GetCommunityById(Guid id)
    {
        var community = await _mediator.Send(new GetCommunityByIdQuery(id));
        if (community == null)
            return NotFound();

        return Ok(community);
    }

    [HttpGet("followed/{userId}")]
    public async Task<ActionResult<List<Community>>> GetFollowedCommunities(Guid userId)
    {
        var communities = await _mediator.Send(new GetFollowedCommunitiesQuery(userId));
        return Ok(communities);
    }

    [HttpGet("{id}/posts")]
    public async Task<ActionResult<List<CommunityPost>>> GetCommunityPosts(Guid id)
    {
        var posts = await _mediator.Send(new GetCommunityPostsQuery(id));
        return Ok(posts);
    }

    [HttpGet("followed/{userId}/posts")]
    public async Task<ActionResult<List<CommunityPost>>> GetFollowedCommunitiesPosts(Guid userId)
    {
        var posts = await _mediator.Send(new GetFollowedCommunitiesPostsQuery(userId));
        return Ok(posts);
    }

    [HttpPost("{id}/follow")]
    public async Task<ActionResult> FollowCommunity(Guid id, [FromBody] Guid userId)
    {
        try
        {
            await _mediator.Send(new FollowCommunityCommand { CommunityId = id, UserId = userId });
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id}/unfollow")]
    public async Task<ActionResult> UnfollowCommunity(Guid id, [FromBody] Guid userId)
    {
        try
        {
            await _mediator.Send(new UnfollowCommunityCommand { CommunityId = id, UserId = userId });
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}