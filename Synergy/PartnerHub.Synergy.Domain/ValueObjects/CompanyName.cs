using PartnersHub.Synergy.Domain.Common;

namespace PartnersHub.Synergy.Domain.ValueObjects;

/// <summary>
/// Represents a company name with validation
/// </summary>
public class CompanyName : ValueObject
{
    public const int MaxLength = 200;
    public const int MinLength = 2;

    public string Value { get; private set; }

    private CompanyName(string value)
    {
        Value = value;
    }

    public static Result<CompanyName> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result<CompanyName>.Failure("Company name is required");
        }

        var trimmedValue = value.Trim();

        if (trimmedValue.Length < MinLength)
        {
            return Result<CompanyName>.Failure($"Company name must be at least {MinLength} characters");
        }

        if (trimmedValue.Length > MaxLength)
        {
            return Result<CompanyName>.Failure($"Company name cannot exceed {MaxLength} characters");
        }

        return Result<CompanyName>.Success(new CompanyName(trimmedValue));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value.ToLowerInvariant();
    }

    public override string ToString() => Value;
}
