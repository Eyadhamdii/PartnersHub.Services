using PartnersHub.Communities.Domain.Common;

namespace PartnersHub.Communities.Domain.Events;

public class CommunityCreatedEvent : DomainEvent
{
    public CommunityCreatedEvent(Guid communityId)
    {
        CommunityId = communityId;
    }

    public Guid CommunityId { get; }
}

public class CommunityUpdatedEvent : DomainEvent
{
    public CommunityUpdatedEvent(Guid communityId)
    {
        CommunityId = communityId;
    }

    public Guid CommunityId { get; }
}

public class CommunityDeactivatedEvent : DomainEvent
{
    public CommunityDeactivatedEvent(Guid communityId)
    {
        CommunityId = communityId;
    }

    public Guid CommunityId { get; }
}

public class CommunityActivatedEvent : DomainEvent
{
    public CommunityActivatedEvent(Guid communityId)
    {
        CommunityId = communityId;
    }

    public Guid CommunityId { get; }
}

public class CommunityFollowedEvent : DomainEvent
{
    public CommunityFollowedEvent(Guid communityId, Guid userId)
    {
        CommunityId = communityId;
        UserId = userId;
    }

    public Guid CommunityId { get; }
    public Guid UserId { get; }
}

public class CommunityUnfollowedEvent : DomainEvent
{
    public CommunityUnfollowedEvent(Guid communityId, Guid userId)
    {
        CommunityId = communityId;
        UserId = userId;
    }

    public Guid CommunityId { get; }
    public Guid UserId { get; }
}

public class CommunityPostAddedEvent : DomainEvent
{
    public CommunityPostAddedEvent(Guid communityId, Guid postId)
    {
        CommunityId = communityId;
        PostId = postId;
    }

    public Guid CommunityId { get; }
    public Guid PostId { get; }
}