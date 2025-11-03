using PartnersHub.Synergy.Domain.Common;

namespace PartnersHub.Synergy.Domain.ValueObjects;

/// <summary>
/// Represents a title with validation
/// </summary>
public class SubSector : ValueObject
{


    public string Value { get; private set; }
    public Guid Id { get; private set; }

    private SubSector(string value, Guid id)
    {
        Value = value;
        Id = id;
    }

    public static Result<SubSector> Create(string value, Guid id)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result<SubSector>.Failure("SubSector name is required");
        }

        var trimmedValue = value.Trim();



        return Result<SubSector>.Success(new SubSector(trimmedValue, id));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
