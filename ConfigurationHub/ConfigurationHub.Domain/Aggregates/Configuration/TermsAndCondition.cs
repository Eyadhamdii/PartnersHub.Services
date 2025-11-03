using PartnersHub.ConfigurationHub.Domain.Common;
using PartnersHub.ConfigurationHub.Domain.Enums;

namespace PartnersHub.ConfigurationHub.Domain.Aggregates.Configuration;

/// <summary>
/// Represents terms and conditions that can be used across all microservices
/// </summary>
public class TermsAndCondition : AggregateRoot {
    public string Version { get; private set; } = null!;
    public TermsType Type { get; private set; }
    public string TitleAr { get; private set; } = null!;
    public string TitleEn { get; private set; } = null!;
    public string ContentAr { get; private set; } = null!;
    public string ContentEn { get; private set; } = null!;
    public TermsStatus Status { get; private set; }
    public DateTime EffectiveDate { get; private set; }
    public DateTime? ExpiryDate { get; private set; }
    public bool RequiresAcceptance { get; private set; }

    // EF Core constructor
    private TermsAndCondition() { }

    private TermsAndCondition(
        string version,
        TermsType type,
        string titleAr,
        string titleEn,
        string contentAr,
        string contentEn,
        DateTime effectiveDate,
        bool requiresAcceptance,
        Guid createdBy) {
        Version = version;
        Type = type;
        TitleAr = titleAr;
        TitleEn = titleEn;
        ContentAr = contentAr;
        ContentEn = contentEn;
        EffectiveDate = effectiveDate;
        RequiresAcceptance = requiresAcceptance;
        Status = TermsStatus.Draft;
        MarkAsCreated(createdBy);
    }

    public static Result<TermsAndCondition> Create(
        string version,
        TermsType type,
        string titleAr,
        string titleEn,
        string contentAr,
        string contentEn,
        DateTime effectiveDate,
        bool requiresAcceptance,
        Guid createdBy) {
        if (string.IsNullOrWhiteSpace(version))
            return Result<TermsAndCondition>.Failure("Version is required");

        if (string.IsNullOrWhiteSpace(titleAr))
            return Result<TermsAndCondition>.Failure("Arabic title is required");

        if (string.IsNullOrWhiteSpace(titleEn))
            return Result<TermsAndCondition>.Failure("English title is required");

        if (string.IsNullOrWhiteSpace(contentAr))
            return Result<TermsAndCondition>.Failure("Arabic content is required");

        if (string.IsNullOrWhiteSpace(contentEn))
            return Result<TermsAndCondition>.Failure("English content is required");

        if (titleAr.Length > 200)
            return Result<TermsAndCondition>.Failure("Arabic title cannot exceed 200 characters");

        if (titleEn.Length > 200)
            return Result<TermsAndCondition>.Failure("English title cannot exceed 200 characters");

        var termsAndCondition = new TermsAndCondition(
            version.Trim(),
            type,
            titleAr.Trim(),
            titleEn.Trim(),
            contentAr.Trim(),
            contentEn.Trim(),
            effectiveDate,
            requiresAcceptance,
            createdBy);

        return Result<TermsAndCondition>.Success(termsAndCondition);
    }

    public Result UpdateContent(
        string titleAr,
        string titleEn,
        string contentAr,
        string contentEn,
        Guid updatedBy) {
        if (Status != TermsStatus.Draft)
            return Result.Failure("Only draft terms can be updated");

        if (string.IsNullOrWhiteSpace(titleAr))
            return Result.Failure("Arabic title is required");

        if (string.IsNullOrWhiteSpace(titleEn))
            return Result.Failure("English title is required");

        if (string.IsNullOrWhiteSpace(contentAr))
            return Result.Failure("Arabic content is required");

        if (string.IsNullOrWhiteSpace(contentEn))
            return Result.Failure("English content is required");

        TitleAr = titleAr.Trim();
        TitleEn = titleEn.Trim();
        ContentAr = contentAr.Trim();
        ContentEn = contentEn.Trim();
        MarkAsUpdated(updatedBy);

        return Result.Success();
    }

    public Result Publish(Guid updatedBy) {
        if (Status != TermsStatus.Draft)
            return Result.Failure("Only draft terms can be published");

        if (EffectiveDate < DateTime.UtcNow.Date)
            return Result.Failure("Effective date must be today or in the future");

        Status = TermsStatus.Active;
        MarkAsUpdated(updatedBy);

        return Result.Success();
    }

    public Result Supersede(Guid updatedBy) {
        if (Status != TermsStatus.Active)
            return Result.Failure("Only active terms can be superseded");

        Status = TermsStatus.Superseded;
        ExpiryDate = DateTime.UtcNow;
        MarkAsUpdated(updatedBy);

        return Result.Success();
    }

    public Result Archive(Guid updatedBy) {
        if (Status == TermsStatus.Draft)
            return Result.Failure("Draft terms should be deleted, not archived");

        Status = TermsStatus.Archived;
        if (!ExpiryDate.HasValue)
            ExpiryDate = DateTime.UtcNow;

        MarkAsUpdated(updatedBy);

        return Result.Success();
    }

    public bool IsActive() {
        return Status == TermsStatus.Active &&
               EffectiveDate <= DateTime.UtcNow &&
               (!ExpiryDate.HasValue || ExpiryDate.Value > DateTime.UtcNow);
    }
}