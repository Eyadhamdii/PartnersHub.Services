
using PartnersHub.Communities.Domain.Common;

namespace PartnersHub.Communities.Domain.Aggregates.Community;

public class CommunityFollower : Entity
{
    private CommunityFollower() { }
    
    private CommunityFollower(Community community, Guid userId)
    {
        CommunityId = community.Id;
        UserId = userId;
        FollowedAt = DateTime.UtcNow;
    }

    public Guid CommunityId { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime FollowedAt { get; private set; }

    public static CommunityFollower Create(Community community, Guid userId)
    {
        if (community == null)
            throw new ArgumentNullException(nameof(community));

        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        return new CommunityFollower(community, userId);
    }
}
