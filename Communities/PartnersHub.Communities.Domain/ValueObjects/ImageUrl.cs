
using PartnersHub.Communities.Domain.Common;

namespace PartnersHub.Communities.Domain.ValueObjects;

public sealed class ImageUrl : ValueObject
{
    public string Value { get; }

    private ImageUrl(string value)
    {
        Value = value;
    }

    public static ImageUrl Create(string url)
    {
        if (url?.Length > 500)
            throw new ArgumentException("Image URL cannot exceed 500 characters", nameof(url));

        if (!string.IsNullOrEmpty(url) && !Uri.IsWellFormedUriString(url, UriKind.Absolute))
            throw new ArgumentException("Invalid image URL format", nameof(url));

        return new ImageUrl(url?.Trim() ?? string.Empty);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(ImageUrl url) => url.Value;
}
