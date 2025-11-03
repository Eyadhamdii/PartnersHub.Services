
using PartnersHub.Communities.Domain.Common;

namespace PartnersHub.Communities.Domain.ValueObjects;

public sealed class PostContent : ValueObject
{
    public string Value { get; }

    private PostContent(string value)
    {
        Value = value;
    }

    public static PostContent Create(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Post content cannot be empty", nameof(content));

        if (content.Length > 5000)
            throw new ArgumentException("Post content cannot exceed 5000 characters", nameof(content));

        return new PostContent(content.Trim());
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(PostContent content) => content.Value;
}
