using PartnersHub.ConfigurationHub.Domain.Common;

namespace PartnersHub.ConfigurationHub.Domain.Aggregates.Lookups;

/// <summary>
/// Represents a sector (Energy, Transportation, Real Estate)
/// </summary>
public class Sector : AggregateRoot {
    public string Code { get; private set; } = null!;
    public string NameAr { get; private set; } = null!;
    public string NameEn { get; private set; } = null!;
    public string? DescriptionAr { get; private set; }
    public string? DescriptionEn { get; private set; }
    public int DisplayOrder { get; private set; }
    public bool IsActive { get; private set; }

    private Sector() { }

    private Sector(
        string code, string nameAr, string nameEn, string? descriptionAr, string? descriptionEn, int displayOrder, Guid createdBy) {
        Code = code;
        NameAr = nameAr;
        NameEn = nameEn;
        DescriptionAr = descriptionAr;
        DescriptionEn = descriptionEn;
        DisplayOrder = displayOrder;
        IsActive = true;
        MarkAsCreated(createdBy);
    }

    public static Result<Sector> Create(
        string code, string nameAr, string nameEn, string? descriptionAr, string? descriptionEn, int displayOrder,
        Guid createdBy) {
        if (string.IsNullOrWhiteSpace(code))
            return Result<Sector>.Failure("Sector code is required");

        if (string.IsNullOrWhiteSpace(nameAr))
            return Result<Sector>.Failure("Arabic name is required");

        if (string.IsNullOrWhiteSpace(nameEn))
            return Result<Sector>.Failure("English name is required");

        if (code.Length > 50)
            return Result<Sector>.Failure("Code cannot exceed 50 characters");

        if (displayOrder < 0)
            return Result<Sector>.Failure("Display order must be non-negative");

        var sector = new Sector(
            code.Trim().ToUpperInvariant(),
            nameAr.Trim(),
            nameEn.Trim(),
            descriptionAr?.Trim(),
            descriptionEn?.Trim(),
            displayOrder,
            createdBy);

        return Result<Sector>.Success(sector);
    }

    public Result Update(
        string nameAr,
        string nameEn,
        string? descriptionAr,
        string? descriptionEn,
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
        DescriptionAr = descriptionAr?.Trim();
        DescriptionEn = descriptionEn?.Trim();
        DisplayOrder = displayOrder;
        MarkAsUpdated(updatedBy);

        return Result.Success();
    }

    public Result Activate(Guid updatedBy) {
        if (IsActive)
            return Result.Failure("Sector is already active");

        IsActive = true;
        MarkAsUpdated(updatedBy);
        return Result.Success();
    }

    public Result Deactivate(Guid updatedBy) {
        if (!IsActive)
            return Result.Failure("Sector is already inactive");

        IsActive = false;
        MarkAsUpdated(updatedBy);
        return Result.Success();
    }
}