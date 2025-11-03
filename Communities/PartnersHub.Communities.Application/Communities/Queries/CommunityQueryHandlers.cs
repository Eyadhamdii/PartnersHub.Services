using MediatR;
using PartnersHub.Communities.Application.Common.Interfaces.Rpository;
using PartnersHub.Communities.Application.Common.Interfaces.Service;
using PartnersHub.Communities.Domain.Aggregates.Community;
using PartnersHub.Communities.Domain.DbModel.Community;

namespace PartnersHub.Communities.Application.Communities.Queries;

public class GetCommunitiesQueryHandler : IRequestHandler<GetCommunitiesQuery, List<Community>>
{
    private readonly ICommunitiesRepository _communitiesRepository;

    public GetCommunitiesQueryHandler(ICommunitiesRepository communitiesRepository)
    {
        _communitiesRepository = communitiesRepository;
    }

    public async Task<List<Community>> Handle(GetCommunitiesQuery request, CancellationToken cancellationToken)
    {
        var communities = await _communitiesRepository.GetAllAsync();
        return communities.ToList();
    }
}

public class GetFollowedCommunitiesQueryHandler : IRequestHandler<GetFollowedCommunitiesQuery, List<Community>>
{
    private readonly ICommunitiesRepository _communitiesRepository;

    public GetFollowedCommunitiesQueryHandler(ICommunitiesRepository communitiesRepository)
    {
        _communitiesRepository = communitiesRepository;
    }

    public async Task<List<Community>> Handle(GetFollowedCommunitiesQuery request, CancellationToken cancellationToken)
    {
        var communities = await _communitiesRepository.GetFollowedCommunitiesAsync(request.UserId);
        return communities.ToList();
    }
}

public class GetCommunityByIdQueryHandler : IRequestHandler<GetCommunityByIdQuery, GetCommunityById?>
{
    private readonly ICommunityService _communitiesService;

    public GetCommunityByIdQueryHandler(ICommunityService communitiesService)
    {
        _communitiesService = communitiesService;
    }

    public async Task<GetCommunityById?> Handle(GetCommunityByIdQuery request, CancellationToken cancellationToken)
    {
        return await _communitiesService.GetCommunityById(request.CommunityId, cancellationToken);
    }
}

public class GetCommunityPostsQueryHandler : IRequestHandler<GetCommunityPostsQuery, List<CommunityPost>>
{
    private readonly ICommunitiesRepository _communitiesRepository;

    public GetCommunityPostsQueryHandler(ICommunitiesRepository communitiesRepository)
    {
        _communitiesRepository = communitiesRepository;
    }

    public async Task<List<CommunityPost>> Handle(GetCommunityPostsQuery request, CancellationToken cancellationToken)
    {
        var posts = await _communitiesRepository.GetCommunityPostsAsync(request.CommunityId);
        return posts.ToList();
    }
}

public class GetFollowedCommunitiesPostsQueryHandler : IRequestHandler<GetFollowedCommunitiesPostsQuery, List<CommunityPost>>
{
    private readonly ICommunitiesRepository _communitiesRepository;

    public GetFollowedCommunitiesPostsQueryHandler(ICommunitiesRepository communitiesRepository)
    {
        _communitiesRepository = communitiesRepository;
    }

    public async Task<List<CommunityPost>> Handle(GetFollowedCommunitiesPostsQuery request, CancellationToken cancellationToken)
    {
        var posts = await _communitiesRepository.GetFollowedCommunitiesPostsAsync(request.UserId);
        return posts.ToList();
    }
}
