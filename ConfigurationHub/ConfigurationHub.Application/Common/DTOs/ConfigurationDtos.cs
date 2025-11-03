namespace PartnersHub.ConfigurationHub.Application.Common.DTOs;

/// <summary>
/// DTO for WhiteListIP
/// </summary>
public record WhiteListIPDto {
    public Guid Id { get; init; }
    public string IPAddress { get; init; } = string.Empty;
    public DateTime ExpiryDate { get; init; }
    public bool IsActive { get; init; }
    public string? Description { get; init; }
    public bool IsExpired { get; init; }
    public bool IsValid { get; init; }
    public Guid CreatedBy { get; init; }
    public DateTime CreatedAt { get; init; }
    public Guid? UpdatedBy { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

/// <summary>
/// DTO for TermsAndCondition
/// </summary>
public record TermsAndConditionDto {
    public Guid Id { get; init; }
    public string Version { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public string TitleAr { get; init; } = string.Empty;
    public string TitleEn { get; init; } = string.Empty;
    public string ContentAr { get; init; } = string.Empty;
    public string ContentEn { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime EffectiveDate { get; init; }
    public DateTime? ExpiryDate { get; init; }
    public bool RequiresAcceptance { get; init; }
    public bool IsActive { get; init; }
    public Guid CreatedBy { get; init; }
    public DateTime CreatedAt { get; init; }
    public Guid? UpdatedBy { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

/// <summary>
/// DTO for Sector
/// </summary>
public record SectorDto {
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string NameAr { get; init; } = string.Empty;
    public string NameEn { get; init; } = string.Empty;
    public string? DescriptionAr { get; init; }
    public string? DescriptionEn { get; init; }
    public int DisplayOrder { get; init; }
    public bool IsActive { get; init; }
}

/// <summary>
/// DTO for SubSector
/// </summary>
public record SubSectorDto {
    public Guid Id { get; init; }
    public Guid SectorId { get; init; }
    public string Code { get; init; } = string.Empty;
    public string NameAr { get; init; } = string.Empty;
    public string NameEn { get; init; } = string.Empty;
    public string? DescriptionAr { get; init; }
    public string? DescriptionEn { get; init; }
    public int DisplayOrder { get; init; }
    public bool IsActive { get; init; }
}

/// <summary>
/// DTO for AssetType
/// </summary>
public record AssetTypeDto {
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string NameAr { get; init; } = string.Empty;
    public string NameEn { get; init; } = string.Empty;
    public string? DescriptionAr { get; init; }
    public string? DescriptionEn { get; init; }
    public int DisplayOrder { get; init; }
    public bool IsActive { get; init; }
}

/// <summary>
/// DTO for UnitOfMeasurement
/// </summary>
public record UnitOfMeasurementDto {
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string NameAr { get; init; } = string.Empty;
    public string NameEn { get; init; } = string.Empty;
    public string? Symbol { get; init; }
    public int DisplayOrder { get; init; }
    public bool IsActive { get; init; }
}