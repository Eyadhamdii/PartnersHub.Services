using PartnersHub.InfraBase.Domain.Common;

namespace PartnersHub.InfraBase.Domain.ValueObjects;

public class ProjectDescription : ValueObject {
    public const int MaxLength = 3000;

    public string? Value { get; private set; }

    private ProjectDescription(string? value) {
        Value = value;
    }

    public static Result<ProjectDescription> Create(string? value) {
        if (string.IsNullOrWhiteSpace(value)) {
            return Result<ProjectDescription>.Success(new ProjectDescription(null));
        }

        if (value.Length > MaxLength) {
            return Result<ProjectDescription>.Failure($"Project description cannot exceed {MaxLength} characters");
        }

        return Result<ProjectDescription>.Success(new ProjectDescription(value.Trim()));
    }

    protected override IEnumerable<object?> GetEqualityComponents() {
        yield return Value;
    }

    public override string ToString() => Value ?? string.Empty;
}