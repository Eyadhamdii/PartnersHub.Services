using PartnersHub.Synergy.Domain.Aggregates.Synergy.Lookups;
using PartnersHub.Synergy.Domain.Common;

namespace PartnersHub.Synergy.Domain.ValueObjects;

/// <summary>
/// Represents a title with validation
/// </summary>
public class Sector : ValueObject
{


    public string Value { get; private set; }
    public Guid Id { get; private set; }

    private Sector(string value, Guid id)
    {
        Value = value;
        Id = id;
    }

    public static Result<Sector> Create(string value, Guid id)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result<Sector>.Failure("Sector name is required");
        }

        var trimmedValue = value.Trim();



        return Result<Sector>.Success(new Sector(trimmedValue,id));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
