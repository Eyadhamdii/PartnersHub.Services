using PartnersHub.InfraBase.Domain.Common;

namespace PartnersHub.InfraBase.Domain.ValueObjects;

public class UnitPrice : ValueObject {
    public const int MaxDigits = 10;

    public decimal Value { get; private set; }

    private UnitPrice(decimal value) {
        Value = value;
    }

    public static Result<UnitPrice> Create(decimal value) {
        if (value <= 0) {
            return Result<UnitPrice>.Failure("Unit price must be greater than zero");
        }

        var valueString = value.ToString("G29");
        var digitsCount = valueString.Replace(".", "").Replace("-", "").Length;

        if (digitsCount > MaxDigits) {
            return Result<UnitPrice>.Failure($"Unit price cannot exceed {MaxDigits} digits");
        }

        return Result<UnitPrice>.Success(new UnitPrice(value));
    }

    protected override IEnumerable<object?> GetEqualityComponents() {
        yield return Value;
    }

    public override string ToString() => Value.ToString();
}