using PartnersHub.InfraBase.Domain.Common;

namespace PartnersHub.InfraBase.Domain.ValueObjects;

public class RejectionReason : ValueObject {
    public const int MaxLength = 3000;

    public string Value { get; private set; }

    private RejectionReason(string value) {
        Value = value;
    }

    public static Result<RejectionReason> Create(string value) {
        if (string.IsNullOrWhiteSpace(value)) {
            return Result<RejectionReason>.Failure("Rejection reason is required");
        }

        if (value.Length > MaxLength) {
            return Result<RejectionReason>.Failure($"Rejection reason cannot exceed {MaxLength} characters");
        }

        return Result<RejectionReason>.Success(new RejectionReason(value.Trim()));
    }

    protected override IEnumerable<object?> GetEqualityComponents() {
        yield return Value;
    }

    public override string ToString() => Value;
}