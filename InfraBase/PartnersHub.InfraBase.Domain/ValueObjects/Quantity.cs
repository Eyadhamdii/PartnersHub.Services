using PartnersHub.InfraBase.Domain.Common;

namespace PartnersHub.InfraBase.Domain.ValueObjects;

public class Quantity : ValueObject {
    public const int MaxDigits = 5;

    public decimal Value { get; private set; }

    private Quantity(decimal value) {
        Value = value;
    }

    public static Result<Quantity> Create(decimal value) {
        if (value <= 0) {
            return Result<Quantity>.Failure("Quantity must be greater than zero");
        }

        // Check if the number exceeds max digits (including decimal places)
        var valueString = value.ToString("G29");
        var digitsCount = valueString.Replace(".", "").Replace("-", "").Length;

        if (digitsCount > MaxDigits) {
            return Result<Quantity>.Failure($"Quantity cannot exceed {MaxDigits} digits");
        }

        return Result<Quantity>.Success(new Quantity(value));
    }

    protected override IEnumerable<object?> GetEqualityComponents() {
        yield return Value;
    }

    public override string ToString() => Value.ToString();
}