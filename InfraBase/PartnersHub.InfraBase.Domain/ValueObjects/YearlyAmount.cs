using PartnersHub.InfraBase.Domain.Common;

namespace PartnersHub.InfraBase.Domain.ValueObjects;

public class YearlyAmount : ValueObject {
    public const int MaxDigits = 11;

    public decimal Value { get; private set; }

    private YearlyAmount(decimal value) {
        Value = value;
    }

    public static Result<YearlyAmount> Create(decimal value) {
        if (value <= 0) {
            return Result<YearlyAmount>.Failure("Amount must be greater than zero");
        }

        var valueString = value.ToString("G29");
        var digitsCount = valueString.Replace(".", "").Replace("-", "").Length;

        if (digitsCount > MaxDigits) {
            return Result<YearlyAmount>.Failure($"Amount cannot exceed {MaxDigits} digits");
        }

        return Result<YearlyAmount>.Success(new YearlyAmount(value));
    }

    protected override IEnumerable<object?> GetEqualityComponents() {
        yield return Value;
    }

    public override string ToString() => Value.ToString();
}