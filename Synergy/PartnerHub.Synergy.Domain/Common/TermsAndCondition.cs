using PartnersHub.Synergy.Domain.Common;

namespace PartnersHub.Synergy.Domain.Common;

/// <summary>
/// Represents a version of terms and conditions (Lookup/Reference Table)
/// </summary>
public class TermsAndCondition : Entity
{
    public string Version { get; private set; } = null!;
    public TermsAndConditionType Type { get; private set; }
    public string Content { get; private set; } = null!;
    public bool IsActive { get; private set; }
    public DateTime EffectiveDate { get; private set; }
    public DateTime? ExpiryDate { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // EF Core constructor
    private TermsAndCondition() { }

    private TermsAndCondition(
        string version, 
        TermsAndConditionType type, 
        string content,
        DateTime effectiveDate)
    {
        Version = version;
        Type = type;
        Content = content;
        EffectiveDate = effectiveDate;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public static Result<TermsAndCondition> Create(
        string version, 
        TermsAndConditionType type, 
        string content,
        DateTime effectiveDate)
    {
        if (string.IsNullOrWhiteSpace(version))
        {
            return Result<TermsAndCondition>.Failure("Terms and conditions version is required");
        }

        if (string.IsNullOrWhiteSpace(content))
        {
            return Result<TermsAndCondition>.Failure("Terms and conditions content is required");
        }

        if (effectiveDate == default)
        {
            return Result<TermsAndCondition>.Failure("Effective date is required");
        }

        return Result<TermsAndCondition>.Success(
            new TermsAndCondition(version.Trim(), type, content.Trim(), effectiveDate));
    }

    public void Deactivate(DateTime? expiryDate = null)
    {
        IsActive = false;
        ExpiryDate = expiryDate ?? DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        ExpiryDate = null;
    }
}
