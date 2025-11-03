using PartnersHub.InfraBase.Domain.Common;

namespace PartnersHub.InfraBase.Domain.ValueObjects;

public class ProjectName : ValueObject {
    public const int MaxLength = 500;

    public string Value { get; private set; }

    private ProjectName(string value) {
        Value = value;
    }

    public static Result<ProjectName> Create(string value) {
        if (string.IsNullOrWhiteSpace(value)) {
            return Result<ProjectName>.Failure("Project name is required");
        }

        if (value.Length > MaxLength) {
            return Result<ProjectName>.Failure($"Project name cannot exceed {MaxLength} characters");
        }

        return Result<ProjectName>.Success(new ProjectName(value.Trim()));
    }

    protected override IEnumerable<object?> GetEqualityComponents() {
        yield return Value;
    }

    public override string ToString() => Value;
}