using PartnersHub.ConfigurationHub.Domain.Common;

namespace PartnersHub.ConfigurationHub.Domain.Aggregates.Lookups;

/// <summary>
/// Represents a unit of measurement (e.g., Square Meter, Kilometer, Unit)
/// </summary>
public class UnitOfMeasurement : AggregateRoot {
    public string Code { get; private set; } = null!;
    public string NameAr { get; private set; } = null!;
    public string NameEn { get; private set; } = null!;
    public string? Symbol { get; private set; }
    public int DisplayOrder { get; private set; }
    public bool IsActive { get; private set; }

    // EF Core constructor
    private UnitOfMeasurement() { }

    private UnitOfMeasurement(
        string code,
        string nameAr,
        string nameEn,
        string? symbol,
        int displayOrder,
        Guid createdBy) {
        Code = code;
        NameAr = nameAr;
        NameEn = nameEn;
        Symbol = symbol;
        DisplayOrder = displayOrder;
        IsActive = true;
        MarkAsCreated(createdBy);
    }

    public static Result<UnitOfMeasurement> Create(
        string code,
        string nameAr,
        string nameEn,
        string? symbol,
        int displayOrder,
        Guid createdBy) {
        if (string.IsNullOrWhiteSpace(code))
            return Result<UnitOfMeasurement>.Failure("UOM code is required");

        if (string.IsNullOrWhiteSpace(nameAr))
            return Result<UnitOfMeasurement>.Failure("Arabic name is required");

        if (string.IsNullOrWhiteSpace(nameEn))
            return Result<UnitOfMeasurement>.Failure("English name is required");

        if (code.Length > 20)
            return Result<UnitOfMeasurement>.Failure("Code cannot exceed 20 characters");

        if (displayOrder < 0)
            return Result<UnitOfMeasurement>.Failure("Display order must be non-negative");

        var uom = new UnitOfMeasurement(
            code.Trim().ToUpperInvariant(),
            nameAr.Trim(),
            nameEn.Trim(),
            symbol?.Trim(),
            displayOrder,
            createdBy);

        return Result<UnitOfMeasurement>.Success(uom);
    }

    public Result Update(
        string nameAr,
        string nameEn,
        string? symbol,
        int displayOrder,
        Guid updatedBy) {
        if (string.IsNullOrWhiteSpace(nameAr))
            return Result.Failure("Arabic name is required");

        if (string.IsNullOrWhiteSpace(nameEn))
            return Result.Failure("English name is required");

        if (displayOrder < 0)
            return Result.Failure("Display order must be non-negative");

        NameAr = nameAr.Trim();
        NameEn = nameEn.Trim();
        Symbol = symbol?.Trim();
        DisplayOrder = displayOrder;
        MarkAsUpdated(updatedBy);

        return Result.Success();
    }

    public Result Activate(Guid updatedBy) {
        if (IsActive)
            return Result.Failure("UOM is already active");

        IsActive = true;
        MarkAsUpdated(updatedBy);
        return Result.Success();
    }

    public Result Deactivate(Guid updatedBy) {
        if (!IsActive)
            return Result.Failure("UOM is already inactive");

        IsActive = false;
        MarkAsUpdated(updatedBy);
        return Result.Success();
    }
}