
using PartnersHub.Communities.Domain.Common;
using PartnersHub.Communities.Domain.ValueObjects;

namespace PartnersHub.Communities.Domain.Aggregates.Community;

public class CommunityPost : Entity
{
    private CommunityPost() { }
    
    private CommunityPost(Community community, Guid authorId, PostContent content)
    {
        CommunityId = community.Id;
        AuthorId = authorId;
        Content = content;
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
    }

    public Guid CommunityId { get; private set; }
    public Guid AuthorId { get; private set; }
    public PostContent Content { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsActive { get; private set; }

    public static CommunityPost Create(Community community, Guid authorId, string content)
    {
        if (community == null)
            throw new ArgumentNullException(nameof(community));

        if (authorId == Guid.Empty)
            throw new ArgumentException("Author ID cannot be empty", nameof(authorId));

        var postContent = PostContent.Create(content);

        return new CommunityPost(community, authorId, postContent);
    }

    public void Update(string content)
    {
        Content = PostContent.Create(content);
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}
