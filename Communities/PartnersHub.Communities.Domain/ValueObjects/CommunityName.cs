
using PartnersHub.Communities.Domain.Common;

namespace PartnersHub.Communities.Domain.ValueObjects;

public sealed class CommunityName : ValueObject
{
    public string Value { get; }

    private CommunityName(string value)
    {
        Value = value;
    }

    public static CommunityName Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Community name cannot be empty", nameof(name));

        if (name.Length > 200)
            throw new ArgumentException("Community name cannot exceed 200 characters", nameof(name));

        return new CommunityName(name.Trim());
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(CommunityName name) => name.Value;
}
