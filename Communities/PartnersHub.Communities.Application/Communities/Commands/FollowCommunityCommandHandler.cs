
using MediatR;
using PartnersHub.Communities.Application.Common.Interfaces;
using PartnersHub.Communities.Application.Common.Interfaces.Rpository;

namespace PartnersHub.Communities.Application.Communities.Commands;

public class FollowCommunityCommandHandler : IRequestHandler<FollowCommunityCommand, bool>
{
    private readonly ICommunitiesRepository _communitiesRepository;
    private readonly IUnitOfWork _unitOfWork;

    public FollowCommunityCommandHandler(
        ICommunitiesRepository communitiesRepository,
        IUnitOfWork unitOfWork)
    {
        _communitiesRepository = communitiesRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(FollowCommunityCommand request, CancellationToken cancellationToken)
    {
        var community = await _communitiesRepository.GetByIdAsync(request.CommunityId);
        
        if (community == null)
            throw new InvalidOperationException("Community not found");

        if (!community.IsActive)
            throw new InvalidOperationException("Community is not active");

        var isFollowing = await _communitiesRepository.IsFollowingAsync(request.CommunityId, request.UserId);
        if (isFollowing)
            throw new InvalidOperationException("User already follows this community");

        community.AddFollower(request.UserId);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
