using PartnersHub.InnovationHub.Domain.Common;


namespace PartnersHub.InnovationHub.Domain.Aggregates.ChallengeRequest;

public class ChallengeRequestRevisionComment : Entity
{
    private ChallengeRequestRevisionComment() { }

    public Guid ChallengeRequestId { get; private set; }
    public string Content { get; set; }
    public string Author { get; set; }
    public DateTime CommentedAt { get; set; }
    public bool IsCurrent { get; set; }

    public ChallengeRequestRevisionComment(
        Guid challengeRequestId, 
        string content,
        string author,
        DateTime commentedAt,
        bool isCurrent
        ) {
        ChallengeRequestId = challengeRequestId;
        Content = content;
        Author = author;
        CommentedAt = commentedAt;
        IsCurrent = isCurrent;

    }

    public static ChallengeRequestRevisionComment Create(Guid challengeRequestId,string content, string author, DateTime commentedAt, bool isCurrent)
    {
        return new ChallengeRequestRevisionComment(challengeRequestId, content, author, commentedAt, isCurrent);
    }

}

