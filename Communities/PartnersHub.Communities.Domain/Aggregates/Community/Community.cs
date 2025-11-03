
using PartnersHub.Communities.Domain.Common;
using PartnersHub.Communities.Domain.Events;
using PartnersHub.Communities.Domain.ValueObjects;

namespace PartnersHub.Communities.Domain.Aggregates.Community;

public class Community : AggregateRoot
{
    private readonly List<CommunityFollower> _communityFollowers;
    private readonly List<CommunityPost> _communityPosts;

    private Community()
    {
        _communityFollowers = new List<CommunityFollower>();
        _communityPosts = new List<CommunityPost>();
    }

    private Community(CommunityName name, CommunityDescription description, ImageUrl imageUrl) : this()
    {
        Name = name;
        Description = description;
        ImageUrl = imageUrl;
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
        
        AddDomainEvent(new CommunityCreatedEvent(Id));
    }

    public CommunityName Name { get; private set; }
    public CommunityDescription Description { get; private set; }
    public ImageUrl ImageUrl { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsActive { get; private set; }
    
    public IReadOnlyCollection<CommunityFollower> Followers => _communityFollowers.AsReadOnly();
    public IReadOnlyCollection<CommunityPost> Posts => _communityPosts.AsReadOnly();

    public static Community Create(string name, string description, string imageUrl)
    {
        var communityName = CommunityName.Create(name);
        var communityDescription = CommunityDescription.Create(description);
        var communityImageUrl = ImageUrl.Create(imageUrl);

        return new Community(communityName, communityDescription, communityImageUrl);
    }

    public void AddFollower(Guid userId)
    {
        if (_communityFollowers.Any(f => f.UserId == userId))
            throw new InvalidOperationException("User already follows this community");

        if (!IsActive)
            throw new InvalidOperationException("Cannot follow an inactive community");

        var follower = CommunityFollower.Create(this, userId);
        _communityFollowers.Add(follower);
        
        AddDomainEvent(new CommunityFollowedEvent(Id, userId));
    }

    public void RemoveFollower(Guid userId)
    {
        var follower = _communityFollowers.FirstOrDefault(f => f.UserId == userId);
        if (follower == null)
            throw new InvalidOperationException("User does not follow this community");

        _communityFollowers.Remove(follower);
        
        AddDomainEvent(new CommunityUnfollowedEvent(Id, userId));
    }

    public void AddPost(string content, Guid authorId)
    {
        if (!IsActive)
            throw new InvalidOperationException("Cannot add posts to an inactive community");

        var post = CommunityPost.Create(this, authorId, content);
        _communityPosts.Add(post);
        
        AddDomainEvent(new CommunityPostAddedEvent(Id, post.Id));
    }

    public void Update(string name, string description, string imageUrl)
    {
        if (!IsActive)
            throw new InvalidOperationException("Cannot update an inactive community");

        Name = CommunityName.Create(name);
        Description = CommunityDescription.Create(description);
        ImageUrl = ImageUrl.Create(imageUrl);
        
        AddDomainEvent(new CommunityUpdatedEvent(Id));
    }

    public void Deactivate()
    {
        if (!IsActive)
            return;

        IsActive = false;
        AddDomainEvent(new CommunityDeactivatedEvent(Id));
    }

    public void Activate()
    {
        if (IsActive)
            return;

        IsActive = true;
        AddDomainEvent(new CommunityActivatedEvent(Id));
    }
}
