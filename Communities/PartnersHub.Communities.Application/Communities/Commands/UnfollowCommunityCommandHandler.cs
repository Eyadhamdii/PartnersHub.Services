
using MediatR;
using PartnersHub.Communities.Application.Common.Interfaces;
using PartnersHub.Communities.Application.Common.Interfaces.Rpository;

namespace PartnersHub.Communities.Application.Communities.Commands;

public class UnfollowCommunityCommandHandler : IRequestHandler<UnfollowCommunityCommand, bool>
{
    private readonly ICommunitiesRepository _communitiesRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UnfollowCommunityCommandHandler(
        ICommunitiesRepository communitiesRepository,
        IUnitOfWork unitOfWork)
    {
        _communitiesRepository = communitiesRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UnfollowCommunityCommand request, CancellationToken cancellationToken)
    {
        var community = await _communitiesRepository.GetByIdAsync(request.CommunityId);
        
        if (community == null)
            throw new InvalidOperationException("Community not found");

        var isFollowing = await _communitiesRepository.IsFollowingAsync(request.CommunityId, request.UserId);
        if (!isFollowing)
            throw new InvalidOperationException("User does not follow this community");

        community.RemoveFollower(request.UserId);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
