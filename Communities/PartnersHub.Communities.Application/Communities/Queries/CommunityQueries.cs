
using MediatR;
using PartnersHub.Communities.Domain.Aggregates.Community;
using PartnersHub.Communities.Domain.DbModel.Community;

namespace PartnersHub.Communities.Application.Communities.Queries;

public record GetCommunitiesQuery : IRequest<List<Community>>;

public record GetFollowedCommunitiesQuery(Guid UserId) : IRequest<List<Community>>;

public record GetCommunityByIdQuery(Guid CommunityId) : IRequest<GetCommunityById?>;

public record GetCommunityPostsQuery(Guid CommunityId) : IRequest<List<CommunityPost>>;

public record GetFollowedCommunitiesPostsQuery(Guid UserId) : IRequest<List<CommunityPost>>;
