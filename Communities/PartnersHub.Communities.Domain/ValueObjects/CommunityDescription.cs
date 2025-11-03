
using PartnersHub.Communities.Domain.Common;

namespace PartnersHub.Communities.Domain.ValueObjects;

public sealed class CommunityDescription : ValueObject
{
    public string Value { get; }

    private CommunityDescription(string value)
    {
        Value = value;
    }

    public static CommunityDescription Create(string description)
    {
        if (description?.Length > 1000)
            throw new ArgumentException("Community description cannot exceed 1000 characters", nameof(description));

        return new CommunityDescription(description?.Trim() ?? string.Empty);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(CommunityDescription description) => description.Value;
}
