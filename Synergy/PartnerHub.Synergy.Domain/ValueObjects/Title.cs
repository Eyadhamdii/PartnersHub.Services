using PartnersHub.Synergy.Domain.Common;

namespace PartnersHub.Synergy.Domain.ValueObjects;

/// <summary>
/// Represents a title with validation
/// </summary>
public class Title : ValueObject
{
    public const int MaxLength = 300;
    public const int MinLength = 3;

    public string Value { get; private set; }

    private Title(string value)
    {
        Value = value;
    }

    public static Result<Title> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result<Title>.Failure("Title is required");
        }

        var trimmedValue = value.Trim();

        if (trimmedValue.Length < MinLength)
        {
            return Result<Title>.Failure($"Title must be at least {MinLength} characters");
        }

        if (trimmedValue.Length > MaxLength)
        {
            return Result<Title>.Failure($"Title cannot exceed {MaxLength} characters");
        }

        return Result<Title>.Success(new Title(trimmedValue));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
