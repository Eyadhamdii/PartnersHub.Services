using MediatR;
using Microsoft.AspNetCore.Mvc;
using PartnersHub.Communities.Apis.Controllers.Base;
using PartnersHub.Communities.Application.Communities.Commands;
using PartnersHub.Communities.Application.Communities.Queries;
using PartnersHub.Communities.Domain.Aggregates.Community;
using PartnersHub.Communities.Domain.DbModel.Community;
using System.Threading;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace PartnersHub.Communities.Apis.Controllers;


public class CommunitiesController : ApiBaseController<CommunitiesController>
{
    [HttpPost]
    public Task<ActionResult<ApiResponse>> Create([FromBody]CreateCommunityCommand command, CancellationToken cancellationToken)
        => Execute(command, cancellationToken);
    
    [HttpGet]
    public  Task<ActionResult<ApiResponse>> GetCommunities(CancellationToken cancellationToken)
        => Execute(new GetCommunitiesQuery(), cancellationToken);


    [HttpGet("{id}")]
    public  Task<ActionResult<ApiResponse>> GetCommunityById(Guid id, CancellationToken cancellationToken)
        => Execute(new GetCommunityByIdQuery(id), cancellationToken);


    [HttpGet("followed/{userId}")]
    public  Task<ActionResult<ApiResponse>> GetFollowedCommunities(Guid userId, CancellationToken cancellationToken)
          => Execute(new GetFollowedCommunitiesQuery(userId), cancellationToken);


    [HttpGet("{id}/posts")]
    public  Task<ActionResult<ApiResponse>> GetCommunityPosts(Guid id, CancellationToken cancellationToken)
         => Execute(new GetCommunityPostsQuery(id), cancellationToken);


    [HttpGet("followed/{userId}/posts")]
    public  Task<ActionResult<ApiResponse>> GetFollowedCommunitiesPosts(Guid userId, CancellationToken cancellationToken)
          => Execute(new GetFollowedCommunitiesPostsQuery(userId), cancellationToken);

    [HttpPost("{id}/follow")]
    public  Task<ActionResult<ApiResponse>> FollowCommunity(Guid id, [FromBody] Guid userId, CancellationToken cancellationToken)
        => Execute(new FollowCommunityCommand { CommunityId = id, UserId = userId }, cancellationToken);


    [HttpPost("{id}/unfollow")]
    public  Task<ActionResult<ApiResponse>> UnfollowCommunity(Guid id, [FromBody] Guid userId, CancellationToken cancellationToken)
          => Execute(new UnfollowCommunityCommand { CommunityId = id, UserId = userId }, cancellationToken);
}