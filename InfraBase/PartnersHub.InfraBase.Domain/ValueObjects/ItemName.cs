using PartnersHub.InfraBase.Domain.Common;

namespace PartnersHub.InfraBase.Domain.ValueObjects;

public class ItemName : ValueObject {
    public const int MaxLength = 500;

    public string Value { get; private set; }

    private ItemName(string value) {
        Value = value;
    }

    public static Result<ItemName> Create(string value) {
        if (string.IsNullOrWhiteSpace(value)) {
            return Result<ItemName>.Failure("Item name is required");
        }

        if (value.Length > MaxLength) {
            return Result<ItemName>.Failure($"Item name cannot exceed {MaxLength} characters");
        }

        return Result<ItemName>.Success(new ItemName(value.Trim()));
    }

    protected override IEnumerable<object?> GetEqualityComponents() {
        yield return Value;
    }

    public override string ToString() => Value;
}