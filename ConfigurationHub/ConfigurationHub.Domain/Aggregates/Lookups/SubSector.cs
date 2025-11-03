using PartnersHub.ConfigurationHub.Domain.Common;

namespace PartnersHub.ConfigurationHub.Domain.Aggregates.Lookups;

/// <summary>
/// Represents a sub-sector under a parent sector
/// </summary>
public class SubSector : AggregateRoot {
    public Guid SectorId { get; private set; }
    public string Code { get; private set; } = null!;
    public string NameAr { get; private set; } = null!;
    public string NameEn { get; private set; } = null!;
    public string? DescriptionAr { get; private set; }
    public string? DescriptionEn { get; private set; }
    public int DisplayOrder { get; private set; }
    public bool IsActive { get; private set; }

    // EF Core constructor
    private SubSector() { }

    private SubSector(
        Guid sectorId,
        string code,
        string nameAr,
        string nameEn,
        string? descriptionAr,
        string? descriptionEn,
        int displayOrder,
        Guid createdBy) {
        SectorId = sectorId;
        Code = code;
        NameAr = nameAr;
        NameEn = nameEn;
        DescriptionAr = descriptionAr;
        DescriptionEn = descriptionEn;
        DisplayOrder = displayOrder;
        IsActive = true;
        MarkAsCreated(createdBy);
    }

    public static Result<SubSector> Create(
        Guid sectorId,
        string code,
        string nameAr,
        string nameEn,
        string? descriptionAr,
        string? descriptionEn,
        int displayOrder,
        Guid createdBy) {
        if (sectorId == Guid.Empty)
            return Result<SubSector>.Failure("Sector is required");

        if (string.IsNullOrWhiteSpace(code))
            return Result<SubSector>.Failure("Sub-sector code is required");

        if (string.IsNullOrWhiteSpace(nameAr))
            return Result<SubSector>.Failure("Arabic name is required");

        if (string.IsNullOrWhiteSpace(nameEn))
            return Result<SubSector>.Failure("English name is required");

        if (code.Length > 50)
            return Result<SubSector>.Failure("Code cannot exceed 50 characters");

        if (displayOrder < 0)
            return Result<SubSector>.Failure("Display order must be non-negative");

        var subSector = new SubSector(
            sectorId,
            code.Trim().ToUpperInvariant(),
            nameAr.Trim(),
            nameEn.Trim(),
            descriptionAr?.Trim(),
            descriptionEn?.Trim(),
            displayOrder,
            createdBy);

        return Result<SubSector>.Success(subSector);
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
            return Result.Failure("Sub-sector is already active");

        IsActive = true;
        MarkAsUpdated(updatedBy);
        return Result.Success();
    }

    public Result Deactivate(Guid updatedBy) {
        if (!IsActive)
            return Result.Failure("Sub-sector is already inactive");

        IsActive = false;
        MarkAsUpdated(updatedBy);
        return Result.Success();
    }
}