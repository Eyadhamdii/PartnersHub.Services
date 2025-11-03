using PartnersHub.ConfigurationHub.Domain.Common;

namespace PartnersHub.ConfigurationHub.Domain.Aggregates.Lookups;

/// <summary>
/// Represents an asset type (e.g., Building, Infrastructure, Equipment)
/// </summary>
public class AssetType : AggregateRoot {
    public string Code { get; private set; } = null!;
    public string NameAr { get; private set; } = null!;
    public string NameEn { get; private set; } = null!;
    public string? DescriptionAr { get; private set; }
    public string? DescriptionEn { get; private set; }
    public int DisplayOrder { get; private set; }
    public bool IsActive { get; private set; }

    // EF Core constructor
    private AssetType() { }

    private AssetType(
        string code,
        string nameAr,
        string nameEn,
        string? descriptionAr,
        string? descriptionEn,
        int displayOrder,
        Guid createdBy) {
        Code = code;
        NameAr = nameAr;
        NameEn = nameEn;
        DescriptionAr = descriptionAr;
        DescriptionEn = descriptionEn;
        DisplayOrder = displayOrder;
        IsActive = true;
        MarkAsCreated(createdBy);
    }

    public static Result<AssetType> Create(
        string code,
        string nameAr,
        string nameEn,
        string? descriptionAr,
        string? descriptionEn,
        int displayOrder,
        Guid createdBy) {
        if (string.IsNullOrWhiteSpace(code))
            return Result<AssetType>.Failure("Asset type code is required");

        if (string.IsNullOrWhiteSpace(nameAr))
            return Result<AssetType>.Failure("Arabic name is required");

        if (string.IsNullOrWhiteSpace(nameEn))
            return Result<AssetType>.Failure("English name is required");

        if (code.Length > 50)
            return Result<AssetType>.Failure("Code cannot exceed 50 characters");

        if (displayOrder < 0)
            return Result<AssetType>.Failure("Display order must be non-negative");

        var assetType = new AssetType(
            code.Trim().ToUpperInvariant(),
            nameAr.Trim(),
            nameEn.Trim(),
            descriptionAr?.Trim(),
            descriptionEn?.Trim(),
            displayOrder,
            createdBy);

        return Result<AssetType>.Success(assetType);
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
            return Result.Failure("Asset type is already active");

        IsActive = true;
        MarkAsUpdated(updatedBy);
        return Result.Success();
    }

    public Result Deactivate(Guid updatedBy) {
        if (!IsActive)
            return Result.Failure("Asset type is already inactive");

        IsActive = false;
        MarkAsUpdated(updatedBy);
        return Result.Success();
    }
}