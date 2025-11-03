using PartnersHub.Synergy.Domain.Common;

namespace PartnersHub.Synergy.Domain.ValueObjects;

/// <summary>
/// Represents a description with validation
/// </summary>
public class Description : ValueObject
{
    public const int MaxLength = 5000;

    public string? Value { get; private set; }

    private Description(string? value)
    {
        Value = value;
    }

    public static Result<Description> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result<Description>.Success(new Description(null));
        }

        var trimmedValue = value.Trim();

        if (trimmedValue.Length > MaxLength)
        {
            return Result<Description>.Failure($"Description cannot exceed {MaxLength} characters");
        }

        return Result<Description>.Success(new Description(trimmedValue));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value ?? string.Empty;
}
